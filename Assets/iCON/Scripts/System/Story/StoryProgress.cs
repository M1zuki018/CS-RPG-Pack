using Cysharp.Threading.Tasks;

namespace iCON.System
{
    /// <summary>
    /// ストーリー進行状態
    /// </summary>
    public class StoryProgress
    {
        /// <summary>
        /// ストーリーデータを取得する
        /// </summary>
        private StoryMasterGetter _masterGetter = new();
        
        private SceneData _sceneData;
    
        /// <summary>現在のパートID</summary>
        public int CurrentPart { get; set; } = 1;
        
        /// <summary>現在のチャプターID</summary>
        public int CurrentChapterId { get; set; } = 1;
        
        /// <summary>現在のシーンID</summary>
        public int CurrentSceneId { get; set; } = 1;
        
        /// <summary>現在のオーダー</summary>
        public int CurrentOrderIndex { get; set; } = -1;

        public async UniTask Setup()
        {
            _sceneData =  await _masterGetter.Setup();
        }
        
        /// <summary>
        /// 次のオーダーに進む
        /// </summary>
        public OrderData NextOrder()
        {
            CurrentOrderIndex++;
            return Get();
        }
    
        /// <summary>
        /// 次のシーンに進む
        /// </summary>
        public OrderData NextScene()
        {
            CurrentSceneId++;
            CurrentOrderIndex = -1;
            return Get();
        }
    
        /// <summary>
        /// 次のチャプターに進む
        /// </summary>
        public OrderData NextChapter()
        {
            CurrentChapterId++;
            CurrentSceneId = 1;
            CurrentOrderIndex = -1;
            return Get();
        }

        /// <summary>
        /// 次のパートに進む
        /// </summary>
        public OrderData NextPart()
        {
            CurrentPart++;
            CurrentChapterId = 1;
            CurrentSceneId = 1;
            CurrentOrderIndex = -1;
            return Get();
        }
    
        /// <summary>
        /// 特定の位置にジャンプ
        /// </summary>
        public OrderData JumpTo(int partId, int chapterId, int sceneId, int orderIndex = 0)
        {
            CurrentPart = partId;
            CurrentChapterId = chapterId;
            CurrentSceneId = sceneId;
            CurrentOrderIndex = orderIndex;
            return Get();
        }

        /// <summary>
        /// マスターデータを取得し、オーダーデータを受け取る
        /// </summary>
        private OrderData Get()
        {
            return _sceneData.Orders[CurrentOrderIndex];
        }
    }   
}