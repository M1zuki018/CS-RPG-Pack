using CryStar.Story.Enums;

namespace CryStar.Story.Data
{
    /// <summary>
    /// オーダーデータ
    /// </summary>
    public class OrderData
    {
        #region Private Fields

        private int _partId;
        private int _chapterId;
        private int _sceneId;
        private int _orderId;
        private OrderType _orderType;
        private SequenceType _sequence;
        private int _speakerId;
        private string _dialogText;
        private string _overrideDisplayName;
        private string _filePath;
        private CharacterPositionType _position;
        private FacialExpressionType _facialExpressionType;
        private float _overrideTextSpeed;
        private float _duration;

        #endregion
        
        /// <summary>
        /// 属しているパートの管理ID
        /// </summary>
        public int PartId => _partId;

        /// <summary>
        /// 属しているチャプターの管理ID
        /// </summary>
        public int ChapterId => _chapterId;

        /// <summary>
        /// 属しているシーンの管理ID
        /// </summary>
        public int SceneId => _sceneId;

        /// <summary>
        /// オーダーの管理ID
        /// </summary>
        public int OrderId => _orderId;

        /// <summary>
        /// オーダーの種類
        /// </summary>
        public OrderType OrderType => _orderType;

        /// <summary>
        /// 前のオーダーとの連結方法
        /// </summary>
        public SequenceType Sequence => _sequence;

        /// <summary>
        /// 話し手のキャラクターの管理ID
        /// </summary>
        public int SpeakerId => _speakerId;

        /// <summary>
        /// テキスト
        /// </summary>
        public string DialogText => _dialogText ?? string.Empty;

        /// <summary>
        /// 表示名のオーバーライド
        /// </summary>
        public string OverrideDisplayName => _overrideDisplayName ?? string.Empty;

        /// <summary>
        /// ファイルパス
        /// </summary>
        public string FilePath => _filePath ?? string.Empty;

        /// <summary>
        /// キャラクターの位置タイプ
        /// </summary>
        public CharacterPositionType Position => _position;

        /// <summary>
        /// 使用する表情差分
        /// </summary>
        public FacialExpressionType FacialExpressionType => _facialExpressionType;

        /// <summary>
        /// テキスト速度のオーバーライド
        /// </summary>
        public float OverrideTextSpeed => _overrideTextSpeed;

        /// <summary>
        /// 継続時間
        /// </summary>
        public float Duration => _duration;
        
        /// <summary>
        /// オーバーライド表示名が設定されているか
        /// </summary>
        public bool HasOverrideDisplayName => !string.IsNullOrEmpty(_overrideDisplayName);

        /// <summary>
        /// ファイルパスが設定されているか
        /// </summary>
        public bool HasFilePath => !string.IsNullOrEmpty(_filePath);
        
        /// <summary>
        /// 表示名を取得（オーバーライドがあればそれを、なければマスターから表示名を取得）
        /// </summary>
        public string DisplayName => HasOverrideDisplayName ? _overrideDisplayName : MasterStoryCharacter.GetCharacter(_speakerId)?.DisplayName ?? string.Empty;

        /// <summary>
        /// 表情差分のパス
        /// </summary>
        public string FacialExpressionPath => MasterStoryCharacter.GetExpressionPath(_speakerId, _facialExpressionType) ?? string.Empty;

        /// <summary>
        /// キャラクターデータ
        /// </summary>
        public CharacterData CharacterData => MasterStoryCharacter.GetCharacter(_speakerId);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OrderData(
            int partId, int chapterId, int sceneId, int orderId,
            OrderType orderType, SequenceType sequence, int speakerId,
            string dialogText = "", string overrideDisplayName = "", string filePath = "",
            CharacterPositionType position = CharacterPositionType.Center,
            FacialExpressionType facialExpressionType = FacialExpressionType.Default,
            float overrideTextSpeed = 0f, float duration = 0f)
        {
            _partId = partId;
            _chapterId = chapterId;
            _sceneId = sceneId;
            _orderId = orderId;
            _orderType = orderType;
            _sequence = sequence;
            _speakerId = speakerId;
            _dialogText = dialogText ?? string.Empty;
            _overrideDisplayName = overrideDisplayName ?? string.Empty;
            _filePath = filePath ?? string.Empty;
            _position = position;
            _facialExpressionType = facialExpressionType;
            _overrideTextSpeed = overrideTextSpeed;
            _duration = duration;
        }
    }
}