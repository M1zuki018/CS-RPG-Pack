using System.Collections.Generic;
using CryStar.Story.Data;

namespace CryStar.Story.Core
{
    /// <summary>
    /// ストーリーのデータをゲーム内で使用できる形に変換するロジックの実装を行うクラスが
    /// 継承すべきインターフェース
    /// </summary>
    public interface IStorySceneDataConverter
    {
        /// <summary>
        /// ヘッダーデータから列マップを初期化
        /// </summary>
        void Initialize(IList<IList<object>> headerData);
        
        /// <summary>
        /// 生データをOrderDataリストに変換
        /// </summary>
        List<OrderData> ConvertToOrderDataList(IList<IList<object>> rawData);
    }
}