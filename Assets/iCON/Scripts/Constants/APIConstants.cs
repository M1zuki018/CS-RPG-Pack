namespace iCON.Constants
{
    /// <summary>
    /// 通信に関連する定数
    /// </summary>
    public static class APIConstants
    {
        /// <summary>
        /// サービスアカウントキーのファイル名
        /// </summary>
        public const string SERVICE_ACCOUNT_KEY_FILE_NAME = "service-account-key.json";

        /// <summary>
        /// APIリクエスト時に送信されるアプリケーション名。Google側でのログや制限管理に使用される
        /// </summary>
        public const string APPLICATION_NAME = "iCON-MasterDataLoader";
    }
}
