namespace iCON.Constants
{
    /// <summary>
    /// Google APIとの通信に関連する定数を定義するクラス
    /// </summary>
    public static class KGoogleApi
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
