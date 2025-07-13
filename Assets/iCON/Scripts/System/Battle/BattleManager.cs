using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using iCON.Enums;
using iCON.UI;
using iCON.Utility;
using UnityEngine;

namespace iCON.Battle
{
    /// <summary>
    /// バトルを管理するクラス
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        /// <summary>
        /// View
        /// </summary>
        [SerializeField, HighlightIfNull]
        private BattleCanvasManager _view;
        
        /// <summary>
        /// バトルの進行状態
        /// </summary>
        [SerializeField]
        private BattleSystemState _currentState;
        
        private BattleStateBase _currentStateHandler;
        private Dictionary<BattleSystemState, BattleStateBase> _states;

        /// <summary>
        /// バトルで使用する変数をまとめたクラス
        /// </summary>
        private BattleData _data;

        /// <summary>
        /// コマンドを記録しておくリスト
        /// </summary>
        private List<string> _commands = new List<string>();
        
        /// <summary>
        /// 現在コマンドを選んでいるキャラクターのIndex
        /// </summary>
        private int _currentCommandSelectIndex = 0;
        
        /// <summary>
        /// バトルで使用する変数をまとめたクラス
        /// </summary>
        public BattleData Data => _data;

        #region Life cycle

        private void Awake()
        {
            ServiceLocator.Resister(this, ServiceType.Local);
            
            // ステートマシンの初期化
            InitializeStates();
            SetState(_currentState);
            
            _data = new BattleData();
        }

        #endregion

        /// <summary>
        /// バトルの状態を変更する
        /// </summary>
        public void SetState(BattleSystemState state)
        {
            try
            {
                _currentStateHandler?.Exit();
                _currentState = state;
                _currentStateHandler = _states[state];
                _currentStateHandler.Enter(this, _view);
            }
            catch(Exception e)
            {
                LogUtility.Error($"バトル進行中に例外が発生しました {e.Message}", LogCategory.Gameplay);
            }
        }

        /// <summary>
        /// コマンドを記録する
        /// TODO: 仮実装
        /// </summary>
        public void RecordCommand(string command)
        {
            _commands.Add(command);
        }
        
        /// <summary>
        /// 次のキャラクターのコマンド選択に移る
        /// </summary>
        /// <returns>次に移れる場合はtrue</returns>
        public bool CheckNextCommandSelect()
        {
            _currentCommandSelectIndex++;
            return _currentCommandSelectIndex < _data.UnitCount;
        }

        /// <summary>
        /// バトル実行
        /// </summary>
        public UniTask ExecuteBattle()
        {
            // TODO: 実行処理を書く
            foreach (var command in _commands)
            {
                LogUtility.Info(command);
            }
            
            // 実行が終わったらコマンドリストをクリア
            _commands.Clear();
            
            return UniTask.CompletedTask;
        }
        
        /// <summary>
        /// StateMachineの初期化
        /// </summary>
        private void InitializeStates()
        {
            _states = new Dictionary<BattleSystemState, BattleStateBase>();
    
            try
            {
                _states.Add(BattleSystemState.FirstSelect, new FirstSelectState());
                _states.Add(BattleSystemState.TryEscape, new TryEscapeState());
                _states.Add(BattleSystemState.CommandSelect, new CommandSelectState());
                _states.Add(BattleSystemState.Execute, new ExecuteState());
            }
            catch(Exception e)
            {
                LogUtility.Error($"State初期化中に例外が発生: {e.Message}", LogCategory.Gameplay);
            }
        }
    }
}
