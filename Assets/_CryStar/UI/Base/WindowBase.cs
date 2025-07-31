using CryStar.Core;
using UnityEngine;

/// <summary>
/// 開閉を行うCanvasにつけるクラスが継承すべきベースクラス
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class WindowBase : CustomBehaviour
{
    private CanvasGroup _canvasGroup;
    
    protected virtual void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    
    /// <summary>
    /// 表示
    /// </summary>
    public virtual void Show()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }
    
    /// <summary>
    /// 非表示
    /// </summary>
    public virtual void Hide()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
    
    /// <summary>
    /// alpha値は変更せず、見た目は残したまま操作できないようにする
    /// </summary>
    public virtual void Block()
    {
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
    
    /// <summary>
    /// Block状態の解除
    /// </summary>
    public virtual void Unblock()
    {
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }
}