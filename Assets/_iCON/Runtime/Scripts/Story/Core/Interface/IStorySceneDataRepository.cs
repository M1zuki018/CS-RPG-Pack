using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace CryStar.Story.Core
{
    /// <summary>
    /// データベースからデータを取得クラスが継承すべきインターフェース
    /// </summary>
    public interface IStorySceneDataRepository
    {
        /// <summary>
        /// ヘッダー行の読み込みを行う
        /// </summary>
        UniTask<IList<IList<object>>> InitializeAsync(string spreadsheetName, string headerRange);
        
        /// <summary>
        /// 指定範囲のデータを読み込んでSceneDataを作成する
        /// </summary>
        UniTask<IList<IList<object>>> LoadSceneDataAsync(string spreadsheetName, string dataRange);
    }
}