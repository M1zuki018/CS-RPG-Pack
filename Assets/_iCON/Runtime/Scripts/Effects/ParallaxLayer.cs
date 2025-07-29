using System;
using CryStar.Attribute;
using UnityEngine;

namespace CryStar.Effects
{
    [Serializable]
    public class ParallaxLayer
    {
        /// <summary>
        /// 識別名
        /// </summary>
        [Comment("識別名")]
        public string Name = "";
        
        /// <summary>
        /// パララックス効果の強さ
        /// </summary>
        [Comment("パララックス効果の強さ"), Range(0f, 0.1f)]
        public float Factor = 0.02f;
        
        /// <summary>
        /// 逆方向に動かすかどうか
        /// </summary>
        [Comment("逆方向に動かすかどうか")]
        public bool Inverse = false;
        
        /// <summary>
        /// パララックス効果を適用するオブジェクト
        /// </summary>
        [Comment("パララックス効果を適用するオブジェクト")]
        public RectTransform[] Objects = new RectTransform[0];
    }
}