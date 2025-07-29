using UnityEngine;

namespace CryStar.Story.Data
{
    /// <summary>
    /// ストーリーシーンデータ
    /// </summary>
    public class StorySceneData
    {
        #region Private Fields

        private int _id;
        private string _sceneName;
        private int _partId;
        private int _chapterId;
        private int _sceneId;
        private string _range;
        private float _characterScale;
        private Vector3 _positionCorrection;
        private int? _prerequisiteStoryId;

        #endregion
        
        /// <summary>
        /// 管理ID
        /// </summary>
        public int Id => _id;

        /// <summary>
        /// シーン名
        /// </summary>
        public string SceneName => _sceneName;

        /// <summary>
        /// パートID
        /// </summary>
        public int PartId => _partId;

        /// <summary>
        /// チャプターID
        /// </summary>
        public int ChapterId => _chapterId;

        /// <summary>
        /// シーンID
        /// </summary>
        public int SceneId => _sceneId;

        /// <summary>
        /// データの読み取り範囲
        /// </summary>
        public string Range => _range;

        /// <summary>
        /// 立ち絵の拡大率
        /// </summary>
        public float CharacterScale => _characterScale;

        /// <summary>
        /// 位置補正量
        /// </summary>
        public Vector3 PositionCorrection => _positionCorrection;

        /// <summary>
        /// 前提ストーリーID（null可）
        /// </summary>
        public int? PrerequisiteStoryId => _prerequisiteStoryId;

        /// <summary>
        /// 前提ストーリーが設定されているか
        /// </summary>
        public bool HasPrerequisite => _prerequisiteStoryId.HasValue;

        /// <summary>
        /// ストーリーシーンデータを作成
        /// </summary>
        public StorySceneData(int id, string sceneName, int partId, int chapterId, int sceneId, 
            string range, float characterScale, Vector3 positionCorrection, int? prerequisiteStoryId)
        {
            _id = id;
            _sceneName = sceneName;
            _partId = partId;
            _chapterId = chapterId;
            _sceneId = sceneId;
            _range = range ?? string.Empty;
            _characterScale = characterScale;
            _positionCorrection = positionCorrection;
            _prerequisiteStoryId = prerequisiteStoryId;
        }
        
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