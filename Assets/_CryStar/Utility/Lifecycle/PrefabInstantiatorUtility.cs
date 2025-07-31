using System;
using CryStar.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CryStar.Utility
{
    /// <summary>
    /// GameObjectの生成と管理に関するユーティリティクラス
    /// </summary>
    public static class PrefabInstantiatorUtility
    {
        /// <summary>
        /// プレハブをインスタンス化し、指定されたCustomBehaviour派生コンポーネントを取得して返す
        /// </summary>
        public static T Instantiate<T>(GameObject prefab) where T : CustomBehaviour
        {
            if (prefab == null)
            {
                Debug.LogException(new ArgumentNullException(nameof(prefab), "⛔ Prefabがnullです"));
                return null;
            }

            try
            {
                var instance = Object.Instantiate(prefab); // インスタンス生成
                var lifecycleObject = instance.GetComponent<T>(); // T型のコンポーネント取得
                
                if (lifecycleObject == null)
                {
                    throw new InvalidOperationException($"⛔ {prefab.name} に {typeof(T).Name} のコンポーネントがアタッチされていません");
                }

                return lifecycleObject;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }
    }
}