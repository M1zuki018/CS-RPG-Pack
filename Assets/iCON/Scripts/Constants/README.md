# Constants フォルダ

このフォルダは、プロジェクト全体で使用される定数を管理しています。

## ルール

- **カテゴリごとに**フォルダに分けて管理する
- 使用用途が近いものは同じクラス内で定義する
- 名前空間は「**iCON.Constants**」
- 定数の命名は**アッパースネークケース**とする
- クラス名の先頭には「**K**」をつける
- ドキュメントコメントをつける

## 基本例

```csharp
namespace iCON.Constants.Core
{
    /// <summary>
    /// アプリケーション基本設定
    /// </summary>
    public static class KCoreApplication
    {
        /// <summary>
        /// アプリケーション名
        /// </summary>
        public const string APPLICATION_NAME = "iCON";
        
        /// <summary>
        /// ターゲットフレームレート
        /// </summary>
        public const int TARGET_FRAME_RATE = 60;
    }
}
```

## やってはいけないこと

- マジックナンバーの直書き
- 複数個所での重複定義
- 不適切なカテゴリ分け
- XMLコメントの省略
- 命名規則の無視

## チェックリスト

新しい定数追加時：

- [ ] 適切なカテゴリフォルダに配置
- [ ] K + カテゴリ + 機能 の命名
- [ ] UPPER_SNAKE_CASE で定数定義
- [ ] XMLコメント記述
- [ ] 重複確認
- [ ] 動作テスト

---

最終更新: 2025年7月13日  
バージョン: 1.0.0