using Cysharp.Threading.Tasks;
using UnityEngine;
using iCON.Utility;

/// <summary>
/// 独自のライフサイクルを定義した基底クラス
/// </summary>
public abstract class ViewBase : MonoBehaviour
{
    /// <summary>
    /// 他クラスに干渉しない処理
    /// </summary>
    public virtual UniTask OnAwake()
    {
        LogUtility.Verbose($"[ViewBase] {gameObject.name} の Awake 実行");
        return UniTask.CompletedTask;
    }

    /// <summary>
    /// UI表示の初期化
    /// </summary>
    public virtual UniTask OnUIInitialize()
    {
        LogUtility.Verbose($"[ViewBase] {gameObject.name} の UIInitialize 実行");
        return UniTask.CompletedTask;
    }

    /// <summary>
    /// event Actionの登録など他のクラスと干渉する処理
    /// </summary>
    public virtual UniTask OnBind()
    {
        LogUtility.Verbose($"[ViewBase] {gameObject.name} の Bind 実行");
        return UniTask.CompletedTask;
    }

    /// <summary>
    /// ゲーム開始前最後に実行される処理
    /// </summary>
    public virtual UniTask OnStart()
    {
        LogUtility.Verbose($"[ViewBase] {gameObject.name} の Start 実行");
        return UniTask.CompletedTask;
    }
}