#if UNITY_EDITOR
using CryStar.Attribute;
using UnityEditor;
using UnityEngine;

namespace CryStar.Editor
{
    /// <summary>
    /// ScriptableObjectをInspector上で展開して編集できるようにするPropertyDrawer
    /// [ExpandableSO]属性が付いたScriptableObjectフィールドに対して、
    /// 参照先のオブジェクトの中身を直接Inspector上で編集可能にする
    /// </summary>
    [CustomPropertyDrawer(typeof(ExpandableSOAttribute))]
    public class ExpandableSODrawer : PropertyDrawer
    {
        /// <summary>外側の余白サイズ</summary>
        private const float MARGIN = 6f;
        
        /// <summary>プロパティ間のパディング</summary>
        private const float PADDING = 2f;
        
        /// <summary>ボックス内のパディング</summary>
        private const float BOX_PADDING = 4f;
        
        /// <summary>SerializedObjectのキャッシュ（パフォーマンス向上のため）</summary>
        private SerializedObject _cachedSerializedObject;
        
        /// <summary>最後にキャッシュしたターゲットオブジェクトの参照</summary>
        private Object _lastCachedTarget;
        
        /// <summary>
        /// プロパティのGUI描画処理
        /// 1. ObjectFieldを描画してScriptableObjectの参照を設定可能にする
        /// 2. 参照が設定されている場合、その中身を展開して表示する
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // プロパティの描画開始をマーク（Undo/Redoの適切な処理のため）
            EditorGUI.BeginProperty(position, label, property);
            
            try
            {
                // ScriptableObjectの参照フィールドを描画
                DrawObjectField(position, property, label);
                
                // 参照が設定されている場合のみ、展開されたコンテンツを描画
                if (property.objectReferenceValue != null)
                {
                    DrawExpandedContent(position, property, label);
                }
            }
            finally
            {
                // プロパティの描画終了をマーク
                EditorGUI.EndProperty();
            }
        }

        /// <summary>
        /// プロパティの表示に必要な高さを計算
        /// 参照が未設定の場合は1行分、設定済みの場合は展開された全体の高さを返す
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 参照が未設定の場合は、ObjectFieldの1行分のみ
            if (property.objectReferenceValue == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            // 参照が設定済みの場合は、展開されたコンテンツ全体の高さを計算
            return CalculateExpandedHeight(property);
        }

        #region 描画を行う処理
        
        /// <summary>
        /// ScriptableObjectの参照フィールドを描画
        /// ユーザーがScriptableObjectアセットをドラッグ&ドロップまたは選択できるフィールド
        /// </summary>
        private void DrawObjectField(Rect position, SerializedProperty property, GUIContent label)
        {
            // ObjectField用の矩形を計算（最上部の1行分）
            var objectFieldRect = new Rect(
                position.x, 
                position.y, 
                position.width, 
                EditorGUIUtility.singleLineHeight
            );
            
            // ObjectFieldを描画し、変更があれば自動的にpropertyに反映される
            property.objectReferenceValue = EditorGUI.ObjectField(
                objectFieldRect, 
                label, 
                property.objectReferenceValue, 
                fieldInfo.FieldType, // フィールドの型情報を使用してフィルタリング
                false // シーンオブジェクトは許可しない（アセットのみ）
            );
        }

        /// <summary>
        /// 展開されたScriptableObjectのコンテンツを描画
        /// 参照先オブジェクトの全プロパティを順次表示し、編集可能にする
        /// </summary>
        private void DrawExpandedContent(Rect position, SerializedProperty property, GUIContent label)
        {
            // SerializedObjectの取得に失敗した場合は何も描画しない
            if (!TryGetSerializedObject(property, out var serializedObject))
            {
                return;
            }

            // 編集前の状態を保存（変更検出のため）
            serializedObject.Update();

            // コンテンツ描画用の矩形を計算
            var contentRect = CalculateContentRect(position, property, label);
            
            // 背景のボックスを描画（視覚的な区別のため）
            DrawContentBackground(contentRect);
            
            // インデントレベルを増加（階層構造を視覚的に表現）
            EditorGUI.indentLevel++;
            try
            {
                // ScriptableObject内の各プロパティを順次描画
                DrawProperties(contentRect, serializedObject);
            }
            finally
            {
                // インデントレベルを元に戻す
                EditorGUI.indentLevel--;
                // 変更があった場合はアセットファイルに保存
                serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// コンテンツの背景ボックスを描画
        /// HelpBoxスタイルを使用して、展開エリアを視覚的に区別
        /// </summary>
        private void DrawContentBackground(Rect contentRect)
        {
            GUI.Box(contentRect, GUIContent.none, EditorStyles.helpBox);
        }

        /// <summary>
        /// ScriptableObjectのプロパティを順次描画
        /// スクリプト参照（m_Script）を除く全てのシリアライズされたフィールドを表示
        /// </summary>
        private void DrawProperties(Rect contentRect, SerializedObject serializedObject)
        {
            // プロパティのイテレータを取得
            var iterator = serializedObject.GetIterator();
            
            // 最初のプロパティ（通常はm_Script）をスキップ
            if (!iterator.NextVisible(true))
            {
                return; // プロパティが存在しない場合は何も描画しない
            }

            // 描画開始位置（背景ボックス内の上端から少し下）
            var yOffset = contentRect.y + MARGIN;
            
            // 残りの全てのプロパティを順次描画
            while (iterator.NextVisible(false))
            {
                // 現在のプロパティの必要な高さを取得
                var propHeight = EditorGUI.GetPropertyHeight(iterator, true);
                
                // プロパティ描画用の矩形を計算
                var propRect = new Rect(
                    contentRect.x + MARGIN, // 左側に余白
                    yOffset,
                    contentRect.width - MARGIN * 2, // 左右に余白
                    propHeight
                );
                
                // プロパティフィールドを描画（配列やカスタムクラスも含めて完全に展開）
                EditorGUI.PropertyField(propRect, iterator, true);
                
                // 次のプロパティの描画位置を計算
                yOffset += propHeight + PADDING;
            }
        }
        #endregion
        
        #region レイアウトの計算
        
        /// <summary>
        /// コンテンツ描画領域を計算
        /// ObjectFieldの下からボックスの内側までの矩形を返す
        /// </summary>
        private Rect CalculateContentRect(Rect position, SerializedProperty property, GUIContent label)
        {
            // コンテンツエリアの高さ（全体からObjectFieldとパディングを引いた値）
            var contentHeight = CalculateExpandedHeight(property) - EditorGUIUtility.singleLineHeight - BOX_PADDING;
            
            return new Rect(
                position.x,
                position.y + EditorGUIUtility.singleLineHeight + PADDING, // ObjectFieldの下から開始
                position.width,
                contentHeight
            );
        }

        /// <summary>
        /// 展開時の全体の高さを計算
        /// ObjectField + 全プロパティの高さ + 余白・パディングの合計
        /// </summary>
        private float CalculateExpandedHeight(SerializedProperty property)
        {
            // SerializedObjectの取得に失敗した場合は1行分のみ
            if (!TryGetSerializedObject(property, out var serializedObject))
            {
                return EditorGUIUtility.singleLineHeight;
            }

            serializedObject.Update();
            
            // 基本の高さ（ObjectField + ボックスのパディング）
            var height = EditorGUIUtility.singleLineHeight + BOX_PADDING;
            
            // プロパティのイテレータを取得
            var iterator = serializedObject.GetIterator();
            
            // 最初のプロパティ（m_Script）をスキップ
            if (!iterator.NextVisible(true))
            {
                return height + MARGIN; // プロパティがない場合
            }

            // 全プロパティの高さを累積
            while (iterator.NextVisible(false))
            {
                height += EditorGUI.GetPropertyHeight(iterator, true) + PADDING;
            }
            
            // 下部の余白を追加
            return height + MARGIN;
        }
        #endregion
        
        #region キャッシュ管理
        
        /// <summary>
        /// SerializedObjectを取得（キャッシュ機能付き）
        /// 同じオブジェクトに対する重複したSerializedObject作成を避けてパフォーマンスを向上
        /// </summary>
        private bool TryGetSerializedObject(SerializedProperty property, out SerializedObject serializedObject)
        {
            serializedObject = null;
            
            // 参照が未設定の場合は取得不可
            if (property.objectReferenceValue == null)
            {
                return false;
            }

            // キャッシュの検証：オブジェクトが変更された場合はキャッシュを更新
            if (_cachedSerializedObject?.targetObject != property.objectReferenceValue)
            {
                // 古いSerializedObjectを破棄してメモリリークを防止
                _cachedSerializedObject?.Dispose();
                
                // 新しいSerializedObjectを作成してキャッシュ
                _cachedSerializedObject = new SerializedObject(property.objectReferenceValue);
                _lastCachedTarget = property.objectReferenceValue;
            }

            serializedObject = _cachedSerializedObject;
            return true;
        }
        
        #endregion

        #region Cleanup

        /// <summary>
        /// リソースのクリーンアップ
        /// ファイナライザーでSerializedObjectを確実に破棄してメモリリークを防止
        /// （ガベージコレクション時に自動実行）
        /// </summary>
        ~ExpandableSODrawer()
        {
            _cachedSerializedObject?.Dispose();
        }
        
        #endregion
    }
}
#endif