using System;
using System.Collections.Generic;
using CryStar.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// UIマネージャーの基底クラス
/// </summary>
public abstract class SceneCanvasManagerBase : CustomBehaviour
{
    [SerializeField] protected List<WindowBase> _canvasObjects = new List<WindowBase>();
    [SerializeField] protected int _defaultCanvasIndex = 0;
    
    protected Stack<int> _canvasStack = new Stack<int>(); // UIパネルのスタック管理用
    protected int _currentCanvasIndex = -1; // 現在表示中のキャンバスインデックス
    public event Action<int, int> OnBeforeCanvasChange; // キャンバス切り替え前のイベント(前のインデックス, 次のインデックス)
    public event Action<int> OnAfterCanvasChange;     // キャンバス切り替え後のイベント(現在のインデックス)
    
    public override UniTask OnStart()
    {
        ShowCanvas(_defaultCanvasIndex);
        return base.OnStart();
    }

    /// <summary>
    /// 指定したキャンバスを表示し、それ以外を非表示にする
    /// </summary>
    public virtual void ShowCanvas(int index)
    {
        // インデックスの範囲チェック
        if (index < 0 || index >= _canvasObjects.Count)
        {
            Debug.LogError($"キャンバスインデックスが範囲外です: {index}");
            return;
        }
        
        // 同じキャンバスを表示しようとしている場合は何もしない
        if (_currentCanvasIndex == index  && _canvasStack.Count == 1 && _canvasStack.Peek() == index)
            return;
        
        OnBeforeCanvasChange?.Invoke(_currentCanvasIndex, index); // 切り替え前イベント発火
        
        // 全てのキャンバスを非表示にする
        foreach (var canvas in _canvasObjects)
        {
            canvas?.Hide();
        }
        
        // スタックをクリアして新しいインデックスをプッシュ
        _canvasStack.Clear();
        _canvasStack.Push(index);
        
        // 指定したキャンバスを表示
        _canvasObjects[index]?.Show();
        
        _currentCanvasIndex = index; // 現在のインデックスを更新
        OnAfterCanvasChange?.Invoke(_currentCanvasIndex); // 切り替え後イベント発火
    }
    
    /// <summary>
    /// 現在の画面の上に新しい画面をオーバーレイとして表示する
    /// </summary>
    public virtual void PushCanvas(int index)
    {
        // インデックスの範囲チェック
        if (index < 0 || index >= _canvasObjects.Count)
        {
            Debug.LogError($"キャンバスインデックスが範囲外です: {index}");
            return;
        }
        
        // 同じキャンバスが最上位に既に表示されている場合は何もしない
        if (_currentCanvasIndex == index)
            return;
        
        OnBeforeCanvasChange?.Invoke(_currentCanvasIndex, index); // 切り替え前イベント発火
        
        // キャンバス切り替え
        for (int i = 0; i < _canvasObjects.Count; i++)
        {
            if (i == index)
            {
                _canvasObjects[i]?.Show();
            }
            else
            {
                _canvasObjects[i]?.Block();
            }
        }
        
        _canvasStack.Push(index); // スタックに新しいインデックスをプッシュ
        _currentCanvasIndex = index; // 現在のインデックスを更新
        OnAfterCanvasChange?.Invoke(_currentCanvasIndex); // 切り替え後イベント発火
    }

    /// <summary>
    /// 最上位の画面を閉じて、一つ前の画面に戻る
    /// </summary>
    public virtual void PopCanvas()
    {
        // スタックが空または1つしかない場合は何もしない
        if (_canvasStack.Count <= 1)
        {
            Debug.Log("これ以上前の画面に戻れません");
            return;
        }
        
        int currentIndex = _canvasStack.Pop(); // 現在の画面をポップ
        int previousIndex = _canvasStack.Peek(); // 一つ前の画面を取得
        
        OnBeforeCanvasChange?.Invoke(currentIndex, previousIndex); // 切り替え前イベント発火
        
        _canvasObjects[currentIndex]?.Hide();　// 現在の画面を非表示にする
        _canvasObjects[previousIndex]?.Unblock();　// 一つ前の画面のブロックを解除
        
        _currentCanvasIndex = previousIndex; // 現在のインデックスを更新
        OnAfterCanvasChange?.Invoke(_currentCanvasIndex); // 切り替え後イベント発火
    }
    
    /// <summary>
    /// 特定のインデックスまでスタックをポップする
    /// </summary>
    public virtual void PopToCanvas(int targetIndex)
    {
        // スタックに目的のインデックスがない場合
        if (!IsCanvasInStack(targetIndex))
        {
            Debug.LogError($"指定されたインデックス {targetIndex} はスタック内に存在しません");
            return;
        }
        
        // 目的のインデックスが最上位の場合は何もしない
        if (_currentCanvasIndex == targetIndex)
            return;
            
        OnBeforeCanvasChange?.Invoke(_currentCanvasIndex, targetIndex); // 切り替え前イベント発火
            
        // 目的のインデックスが出てくるまでポップして非表示にする
        while (_canvasStack.Count > 0 && _canvasStack.Peek() != targetIndex)
        {
            int index = _canvasStack.Pop();
            _canvasObjects[index]?.Hide();
        }
        
        // 目的のキャンバスをUnblockする
        _canvasObjects[targetIndex]?.Unblock();
        
        _currentCanvasIndex = targetIndex; // 現在のインデックスを更新
        OnAfterCanvasChange?.Invoke(_currentCanvasIndex); // 切り替え後イベント発火
    }
    
    /// <summary>
    /// スタック内に特定のキャンバスが存在するかチェック
    /// </summary>
    protected bool IsCanvasInStack(int index)
    {
        return _canvasStack.Contains(index);
    }
    
    /// <summary>
    /// 現在のキャンバスインデックスを取得
    /// </summary>
    public int GetCurrentCanvasIndex()
    {
        return _currentCanvasIndex;
    }
}