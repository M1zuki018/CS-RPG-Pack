using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIのGetComponentを備えたベースクラス
/// </summary>
public abstract class UIElementBase<T> : MonoBehaviour where T : MaskableGraphic
{
    protected T _element;
    
    protected virtual void Awake()
    {
        if(!TryGetComponent(out _element)) Debug.LogAssertion($"{gameObject.name} {typeof(T)}が取得できませんでした");
    }
}