using System;

namespace iCON.System
{
    /// <summary>
    /// ストーリーのデータを取得する際に必要なデータ
    /// </summary>
    [Serializable]
    public class StoryLine
    {
        public string SceneName = "シーン名";
        public string SpreadsheetName;
        public string HeaderRange = "SheetName!A2:N2";
        public String Range = "SheetName!A3:N3";
    }
}
