# CryStar Editor Tools

Unity開発効率を向上させるエディター拡張ツール集。スクリプト作成支援、Summaryコメント表示など、日常的な開発作業を自動化・効率化します。

## 特徴
# CryStar Editor Tools

Unity開発効率を向上させるエディター拡張ツール集です。日常的な開発作業を自動化・効率化し、より快適な開発体験を提供します。

## 特徴

- **Button Text Renamer**: ボタンコンポーネントの子テキストオブジェクトをリネームするウィンドウを提供
- **Hierarchy Highlighter**: ヒエラルキービューでのハイライト表示。ルール設定ウィンドウも提供
- **Script Creator**: テンプレートベースのスクリプト作成と特定のフォルダ内のアセット名を元にしたEnum自動生成
- **Summary Display**: プロジェクトビューでC#スクリプトのSummaryコメント表示

## 📦 インストール

### Package Manager経由（推奨）

1. Unity Package Managerを開く
2. `+` → `Add package from git URL...`
3. 以下のURLを入力：
```
https://github.com/M1zuki018/crystar-editor-tools.git
```

### 特定のバージョンをインストール

```
https://github.com/M1zuki018/crystar-editor-tools.git#v1.0.0
```

### manifest.jsonに直接追加

`Packages/manifest.json`に以下を追加：

```json
{
  "dependencies": {
    "com.crystar.editor-tools": "https://github.com/M1zuki018/crystar-editor-tools.git"
  }
}
```

## 🛠️ 機能詳細

### Button Text Renamer

UIボタンの子テキストオブジェクトを親ボタン名に合わせてリネームします。

**使用方法:**
- `CryStar/Tools/Button Text Renamer`でウィンドウを開く
- 設定オプションを自由に変更
- 「シーン内の全ボタンテキストをリネーム」または「選択されたボタンのテキストをリネーム」を実行

**対応コンポーネント:**
- Legacy Text (UI)
- TextMeshPro UGUI

**設定オプション:**
- 非アクティブオブジェクトの処理
- カスタムサフィックスの設定
- デバッグログの表示

### Hierarchy Highlighter

ヒエラルキービューで特定のプレフィックスを持つGameObjectの背景色とテキストスタイルを変更できます。

**デフォルトルール:**
- `#Folder` - 赤色背景
- `#Section` - 青色背景（太字）
- `#Group` - 緑色背景
- `#UI` - オレンジ色背景（斜体）

**使用方法:**
- `CryStar/Tools/Hierarchy Folder Highlighter`で設定を開く
- カスタムルールの追加・編集が可能

### Script Creator

テンプレートベースのスクリプト作成とフォルダ内ファイルからのEnum生成を提供します。

**使用方法:**
- `CryStar/Tools/Script Creation Window`でウィンドウを開く
- **Script Creation**タブ: テンプレートからスクリプト作成
- **Enum Generation**タブ: フォルダ内ファイル名からEnum生成

**機能:**
- カスタムテンプレートサポート
- クラス名の自動抽出
- ファイル重複チェック
- 作成後のオブジェクト自動選択


### テンプレートの追加

1. `Assets/_CryStar/Editor/ScriptTemplates/`フォルダにテンプレートファイル（.txt）を配置
2. `{ClassName}`をクラス名のプレースホルダーとして使用

**テンプレート例:**
```csharp
using UnityEngine;

/// <summary>
/// {ClassName}の説明
/// </summary>
public class {ClassName} : MonoBehaviour
{
    void Start()
    {
        
    }
}
```

### Summary Display

プロジェクトビューでC#スクリプトファイルのSummaryコメントを表示します。

**特徴:**
- クラス宣言直前の`<summary>`コメントを自動抽出
- キャッシュシステムによる高速表示
- ファイル更新の自動検知
- 表示幅に応じた文字列切り詰め

## 要件

- Unity 2022.3 以上
- .NET Standard 2.1

## ライセンス

MIT License

## サポート

Issue報告やフィードバックは[GitHubリポジトリ](https://github.com/M1zuki018/crystar-editor-tools)までお願いします。

## 変更履歴

詳細な変更履歴は[CHANGELOG.md](CHANGELOG.md)をご覧ください。