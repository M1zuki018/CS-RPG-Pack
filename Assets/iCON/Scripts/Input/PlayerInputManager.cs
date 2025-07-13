using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using iCON.Constants;
using iCON.Enums;
using iCON.Utility;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace iCON.Input
{
    /// <summary>
    /// PlayerのInputActionを管理するクラス
    /// </summary>
    public class PlayerInputManager : ViewBase
    {
        /// <summary>
        /// InputActionAssetの参照
        /// </summary>
        [SerializeField, HighlightIfNull] private InputActionAsset _actionAsset;

        /// <summary>
        /// ゲームの状態と対応するActionMapの辞書
        /// </summary>
        private Dictionary<GameStateType, InputActionMap> _contextMaps = new Dictionary<GameStateType, InputActionMap>();

        private IPlayerInputReceiver _iPlayerInputReceiver;
        private CompositeDisposable _disposables = new CompositeDisposable();

        #region Life cycle

        /// <summary>
        /// Awake
        /// </summary>
        public override UniTask OnAwake()
        {
            _iPlayerInputReceiver = new PlayerInputProcessor();

            // ActionMapの初期化
            InitializeActionMaps();
            
            // ステートに合わせて入力制限をかけたり解除したりする処理
            GameManagerServiceLocator.Instance.CurrentGameStateProp
                .Subscribe(SwitchInputContext)
                .AddTo(_disposables);

            // 入力アクションに各メソッドを登録
            RegisterInputActions();
            
            return base.OnAwake();
        }

        /// <summary>
        /// Destroy
        /// </summary>
        private void OnDestroy()
        {
            UnregisterInputActions();
            _disposables?.Dispose();
        }

        #endregion

        /// <summary>
        /// GameStateとActionMapの対応を初期化
        /// </summary>
        private void InitializeActionMaps()
        {
            _contextMaps[GameStateType.Field] = _actionAsset.FindActionMap("Field");
            _contextMaps[GameStateType.Title] = _actionAsset.FindActionMap("UI");
            _contextMaps[GameStateType.Story] = _actionAsset.FindActionMap("UI");

            // 存在確認
            foreach (var kvp in _contextMaps)
            {
                if (kvp.Value == null)
                {
                    LogUtility.Error($"{kvp.Key}のアクションマップが見つかりませんでした", LogCategory.Gameplay, this);
                }
            }
        }

        /// <summary>
        /// 入力コンテキストを切り替える
        /// </summary>
        private void SwitchInputContext(GameStateType newState)
        {
            // 全てのActionMapを無効化
            foreach (var actionMap in _contextMaps.Values)
            {
                actionMap?.Disable();
            }

            // 対応するActionMapを有効化
            if (_contextMaps.TryGetValue(newState, out var targetMap) && targetMap != null)
            {
                targetMap.Enable();
                LogUtility.Verbose($"入力方法を変更しました: {newState}", LogCategory.Gameplay, this);
            }
            else
            {
                LogUtility.Warning($"ActionMapが見つかりません: {newState}", LogCategory.Gameplay, this);
            }
        }

        #region Register

        /// <summary>
        /// PlayerInput の各アクションに対応するメソッドを登録
        /// </summary>
        private void RegisterInputActions()
        {
            foreach (var actionMap in _contextMaps.Values)
            {
                if (actionMap == null) continue;

                // 各ActionMapの共通アクションを登録
                RegisterCommonActions(actionMap);

                // 特定のActionMapのみの特殊アクション登録
                RegisterSpecificActions(actionMap);
            }
        }

        /// <summary>
        /// 全てのActionMapに共通するアクションを登録
        /// </summary>
        private void RegisterCommonActions(InputActionMap actionMap)
        {
            actionMap.FindAction(KInputActionNames.CONFIRM).performed += OnConfirm;
            actionMap.FindAction(KInputActionNames.PAUSE).performed += OnPause;
        }

        /// <summary>
        /// 特定のActionMapのみの特殊アクションを登録
        /// </summary>
        private void RegisterSpecificActions(InputActionMap actionMap)
        {
            switch (actionMap.name)
            {
                case "Field":
                    actionMap.FindAction(KInputActionNames.MOVE).started += OnMove;
                    actionMap.FindAction(KInputActionNames.MOVE).performed += OnMove;
                    actionMap.FindAction(KInputActionNames.MOVE).canceled += OnMove;
                    actionMap.FindAction(KInputActionNames.DASH).performed += OnDash;
                    actionMap.FindAction(KInputActionNames.SHORTCUT).performed += OnShortcut;
                    actionMap.FindAction(KInputActionNames.CHARACTER_MENU).performed += OnCharaMenu;
                    break;

                case "UI":
                    // TODO: 後で対応
                    break;
            }
        }

        #endregion

        #region UnRegister

        /// <summary>
        /// PlayerInput の登録を解除（OnDestroy用）
        /// </summary>
        private void UnregisterInputActions()
        {
            foreach (var actionMap in _contextMaps.Values)
            {
                if (actionMap == null) continue;

                // 共通アクションの解除
                UnregisterCommonActions(actionMap);

                // 特定のActionMapのみの特殊アクション解除
                UnregisterSpecificActions(actionMap);
            }
        }

        /// <summary>
        /// 全てのActionMapに共通するアクションの登録を解除
        /// </summary>
        private void UnregisterCommonActions(InputActionMap actionMap)
        {
            var confirmAction = actionMap.FindAction(KInputActionNames.CONFIRM);
            if (confirmAction != null)
            {
                confirmAction.performed -= OnConfirm;
            }

            var pauseAction = actionMap.FindAction(KInputActionNames.PAUSE);
            if (pauseAction != null)
            {
                pauseAction.performed -= OnPause;
            }
        }

        /// <summary>
        /// 特定のActionMapのみの特殊アクションの登録を解除
        /// </summary>
        private void UnregisterSpecificActions(InputActionMap actionMap)
        {
            switch (actionMap.name)
            {
                case "Field":
                    var moveAction = actionMap.FindAction(KInputActionNames.MOVE);
                    if (moveAction != null)
                    {
                        moveAction.started -= OnMove;
                        moveAction.performed -= OnMove;
                        moveAction.canceled -= OnMove;
                    }

                    var dashAction = actionMap.FindAction(KInputActionNames.DASH);
                    if (dashAction != null)
                    {
                        dashAction.performed -= OnDash;
                    }

                    var shortcutAction = actionMap.FindAction(KInputActionNames.SHORTCUT);
                    if (shortcutAction != null)
                    {
                        shortcutAction.performed -= OnShortcut;
                    }

                    var charaMenuAction = actionMap.FindAction(KInputActionNames.CHARACTER_MENU);
                    if (charaMenuAction != null)
                    {
                        charaMenuAction.performed -= OnCharaMenu;
                    }

                    break;

                case "UI":
                    // TODO: UIのアクションが追加された際にここに実装
                    break;
            }
        }

        #endregion

        #region 入力されたときのメソッド一覧

        /// <summary>移動処理</summary>
        public void OnMove(InputAction.CallbackContext context) =>
            _iPlayerInputReceiver.HandleMoveInput(context.ReadValue<Vector2>());

        /// <summary>ダッシュ処理</summary>
        public void OnDash(InputAction.CallbackContext context) => _iPlayerInputReceiver.HandleDashInput();

        /// <summary>決定</summary>
        public void OnConfirm(InputAction.CallbackContext context) => _iPlayerInputReceiver.HandleConfirmInput();

        /// <summary>ポーズ</summary>
        public void OnPause(InputAction.CallbackContext context) => _iPlayerInputReceiver.HandlePauseInput();

        /// <summary>ショートカットを開く</summary>
        public void OnShortcut(InputAction.CallbackContext context) => _iPlayerInputReceiver.HandleShortcutInput();

        /// <summary>キャラクターメニューを開く</summary>
        public void OnCharaMenu(InputAction.CallbackContext context) => _iPlayerInputReceiver.HandleCharaMenuInput();

        #endregion
    }
}