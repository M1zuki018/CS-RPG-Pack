using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using iCON.Battle;
using iCON.Enums;
using UnityEngine;

namespace iCON.UI
{
    /// <summary>
    /// バトルシーンのCanvasManager
    /// </summary>
    public class BattleCanvasManager : SceneCanvasManagerBase
    {
        /// <summary>
        /// キャラクターアイコンのPrefab
        /// TODO: 管理方法は検討中
        /// </summary>
        [SerializeField]
        private CharacterIconContents _iconContents;
        private List<CharacterIconContents> _icons;
        
        /// <summary>
        /// ダメージテキストのオブジェクトプールを管理するクラス
        /// </summary>
        [SerializeField]
        private DamageTextPool _damageTextPool;
        
        /// <summary>
        /// 現在表示中のウィンドウを取得する
        /// </summary>
        public WindowBase CurrentCanvas => _canvasObjects[_currentCanvasIndex];

        /// <summary>
        /// キャンバスを切り替える
        /// </summary>
        public void ShowCanvas(BattleCanvasType canvasType)
        {
            base.ShowCanvas((int)canvasType);
        }
        
        /// <summary>
        /// キャラクターアイコンを生成する
        /// </summary>
        public void SetupIcons(IReadOnlyList<BattleUnit> unitData)
        {
            // リストを新規作成
            _icons = new List<CharacterIconContents>(unitData.Count);
            
            for (int i = 0; i < unitData.Count; i++)
            {
                // 自身の子オブジェクトとしてアイコンを生成
                var icon = Instantiate(_iconContents, transform);
                _icons.Add(icon);
                icon.Setup(_damageTextPool);
            }
            
            SubscribeToUnitEvents(unitData);
        }

        /// <summary>
        /// キャラクターのHP・SP変動アクションを購読する
        /// </summary>
        private void SubscribeToUnitEvents(IReadOnlyList<BattleUnit> unitData)
        {
            // 味方ユニットのイベント購読
            for (int i = 0; i < unitData.Count; i++)
            {
                var unit = unitData[i];
                var index = i; // ローカル変数でキャプチャ
                
                unit.OnHpChanged += (currentHp, maxHp, damage) => UpdatePlayer(index, currentHp, maxHp, damage);
                //unit.OnSpChanged += (newSp) => UpdatePlayer(index);
                //unit.OnDeath += () => UpdatePlayer(index);
            }
        }

        /// <summary>
        /// ダメージを受けた時のUI更新
        /// </summary>
        private void UpdatePlayer(int index, int value, int maxValue, int damage)
        {
            // HPバーを更新
            _icons[index].SetHpSlider(value, maxValue);
            
            // ダメージ量の表記を行う
            _icons[index].SetDamageText(damage).Forget();
        }
    }

}
