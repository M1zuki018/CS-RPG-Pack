using Cysharp.Threading.Tasks;
using iCON.Enums;
using R3;

/// <summary>
/// ゲームマネージャー
/// </summary>
public class GameManager : ViewBase, IGameManager
{
    private bool _isFirstLoad = true; // 最初の読み込みかどうか
    private readonly ReactiveProperty<GameStateType> _currentGameState = new ReactiveProperty<GameStateType>();
    public bool IsFirstLoad => _isFirstLoad;
    
    /// <summary>
    /// 現在のゲームの進行状態
    /// </summary>
    public ReadOnlyReactiveProperty<GameStateType> CurrentGameStateProp => _currentGameState;

    public override UniTask OnAwake()
    {
        // 既に別のインスタンスが存在する場合、このオブジェクトを破棄
        if (GameManagerServiceLocator.IsInitialized() && GameManagerServiceLocator.Instance != this)
        {
            Destroy(gameObject);
            return base.OnAwake();
        }
        
        // サービスロケーターに自身を登録
        GameManagerServiceLocator.SetInstance(this);
        DontDestroyOnLoad(gameObject);
        
        // _currentGameState
        //     .Skip(1) // 初期値はスキップ
        //     .Take(1) // 最初の一回だけのみ処理
        //     .Subscribe(newState => 
        //     {
        //         // Titleから他のステートに変わった時点でフラグをオフにする
        //         if (newState != GameStateEnum.Title)
        //         {
        //             _isFirstLoad = false;
        //         }
        //     })
        //     .AddTo(this);
        
        return base.OnAwake();
    }
    
    private void OnDestroy()
    {
        // 自身がインスタンスとして登録されている場合のみリセット
        if ((GameManager)GameManagerServiceLocator.Instance == this)
        {
            GameManagerServiceLocator.Reset();
        }
    }
}