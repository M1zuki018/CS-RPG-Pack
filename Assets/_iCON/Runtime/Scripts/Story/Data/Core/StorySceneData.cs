using System;
using UnityEngine;

namespace CryStar.Story.Data
{
    /// <summary>
    /// ストーリーシーンデータクラス
    /// </summary>
    [Serializable]
    public class StorySceneData
    {
        /// <summary>シーンID</summary>
        public int Id { get; set; }

        /// <summary>シーン名</summary>
        public string SceneName { get; set; }

        /// <summary>パートID</summary>
        public int PartId { get; set; }

        /// <summary>チャプターID</summary>
        public int ChapterId { get; set; }

        /// <summary>シーンID（チャプター内）</summary>
        public int SceneId { get; set; }

        /// <summary>範囲（セル範囲など）</summary>
        public string Range { get; set; }

        /// <summary>立ち絵の拡大率</summary>
        public float CharacterScale { get; set; }

        /// <summary>位置補正量</summary>
        public Vector3 PositionCorrection { get; set; }

        /// <summary>前提ストーリーID（null可）</summary>
        public int? PrerequisiteStoryId { get; set; }

        /// <summary>
        /// 文字列表現を取得
        /// </summary>
        public override string ToString()
        {
            return $"StoryScene[{Id}]: {SceneName} (Part{PartId}-Chapter{ChapterId}-Scene{SceneId})";
        }

        /// <summary>
        /// 位置補正量を文字列で取得
        /// </summary>
        public string GetPositionCorrectionString()
        {
            return $"{PositionCorrection.x}-{PositionCorrection.y}-{PositionCorrection.z}";
        }
    }
}