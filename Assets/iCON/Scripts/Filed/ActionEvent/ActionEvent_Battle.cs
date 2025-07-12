
using Cysharp.Threading.Tasks;
using iCON.System;
using UnityEngine;

namespace iCON.Field.System
{
    /// <summary>
    /// バトルを行うアクションイベント
    /// </summary>
    public class ActionEvent_Battle : ActionEventBase
    {
        protected override void OnPlayerEnter(Collider2D playerCollider)
        {
            base.OnPlayerEnter(playerCollider);
            ServiceLocator.GetGlobal<SceneLoader>().LoadSceneAsync(new SceneTransitionData(SceneType.Battle)).Forget();
        }
    }
}