using System;
using System.Collections.Generic;
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

        private void Awake()
        {
            ServiceLocator.Resister(this, ServiceType.Local);
            InitializeStates();

            SetState(_currentState);
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
                _states.Add(BattleSystemState.ActionSelect, new ActionSelectState());
            }
            catch(Exception e)
            {
                LogUtility.Error($"State初期化中に例外が発生: {e.Message}", LogCategory.Gameplay);
            }
        }

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
    }
}
