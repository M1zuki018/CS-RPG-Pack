using System;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using CryStar.Attribute;

namespace CryStar.Editor
{
    /// <summary>
    /// インスペクターにボタンを表示しメソッドを実行できるようにする（ContextMenuの代わりなどに）
    /// </summary>
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class MethodButtonInspectorDrawer : UnityEditor.Editor
    {
        // NOTE: 初回のみリフレクションを行い、以降はキャッシュを使用して重い検索を避けている
        private MethodInfo[] _cachedMethods;
        private Type _cachedType;
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector(); // デフォルトのプロパティを描画
            DrawButtonsForMethods(); // ボタンを描画
        }

        /// <summary>
        /// メソッドを探してボタンを描画
        /// </summary>
        private void DrawButtonsForMethods()
        {
            // 現在のターゲットオブジェクトを取得
            MonoBehaviour targetObject = (MonoBehaviour)target;

            // 型が変わった場合のみメソッドを再取得
            if (_cachedMethods == null || _cachedType != targetObject.GetType())
            {
                _cachedType = targetObject.GetType();
                _cachedMethods = _cachedType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }

            // [MethodButtonInspector] 属性が付いたメソッドを探す
            foreach (MethodInfo method in _cachedMethods)
            {
                MethodButtonInspectorAttribute methodButtonInspectorAttribute =
                    method.GetCustomAttribute<MethodButtonInspectorAttribute>();
                if (methodButtonInspectorAttribute != null)
                {
                    string buttonText = string.IsNullOrEmpty(methodButtonInspectorAttribute.Label)
                        ? method.Name
                        : methodButtonInspectorAttribute.Label;

                    // ボタンを描画
                    if (GUILayout.Button(buttonText))
                    {
                        try
                        {
                            if (method.GetParameters().Length > 0)
                            {
                                Debug.LogWarning($"複数のパラメーターがあるメソッドは Inspector Button からは呼び出せません！ '{method.Name}'");
                                continue;
                            }
                            
                            // メソッドを呼び出す
                            method.Invoke(targetObject, null);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Method '{method.Name}' execution failed: {e.Message}");
                        }
                    }
                }
            }
        }
    }
}