# Shiren 6 Azukariya Capacity

Steam版『風来のシレン6 とぐろ島探検録』の **あずかり屋** の容量上限を拡張する BepInEx 6 / IL2CPP 用MODです。

標準の **960個** を **9600個** に拡張します。

> [!WARNING]
> 非公式MODです。使用前にセーブデータのバックアップを推奨します。
> 9600個すべてを埋める実地テストはしていません。

## 動作確認

確認できている範囲:

- あずかり屋の表示上限が 960 → 9600 に変わる
- 960個の満杯判定を超えて、961個目を預けられる
- あずかり屋を閉じて再度開いても961個目が残る
- セーブしてゲームを終了し、再起動しても961個目が保持される

9600個付近での一覧表示・ソート・セーブ時間などは未検証です。

## 必要なもの

- Steam版『風来のシレン6 とぐろ島探検録』
- **BepInEx 6 Bleeding Edge / Unity.IL2CPP x64**
- **.NET 6 SDK**
- Git（cloneする場合）

### 重要: BepInEx導入後に一度ゲームを起動する

このプロジェクトは、BepInExが生成する以下のIL2CPP interop assemblyをビルド時に参照します。

```text
BepInEx\interop\Assembly-CSharp.dll
BepInEx\interop\Il2Cppmscorlib.dll
```

そのため、**BepInExをゲームへ導入しただけでは不十分です**。

1. BepInEx 6 Unity.IL2CPP版をシレン6へ導入
2. ゲームを一度起動
3. タイトル画面まで進んだら終了
4. `BepInEx\interop` が生成されていることを確認

してからビルドしてください。

## ビルド

### Steamが標準パスの場合

```powershell
dotnet build -c Release
```

標準では次のゲームフォルダを参照します。

```text
C:\Program Files (x86)\Steam\steamapps\common\ShirenTheWanderer6
```

### Steamライブラリが別の場所にある場合

`Shiren6Dir` を指定してください。

```powershell
dotnet build -c Release -p:Shiren6Dir="D:\SteamLibrary\steamapps\common\ShirenTheWanderer6"
```

成功すると、通常は次にDLLが生成されます。

```text
bin\Release\net6.0\Shiren6.AzukariyaCapacity.dll
```

## インストール

生成された

```text
Shiren6.AzukariyaCapacity.dll
```

を、

```text
ShirenTheWanderer6\BepInEx\plugins\
```

へコピーしてください。

旧バージョンのDLLを使っている場合は、二重ロードを避けるため古いDLLを削除してから入れ替えてください。

## アンインストールについて

MODを外した状態で、960個を超えて保存されているアイテムがどう扱われるかは十分に検証していません。

MODを外す場合は、先にセーブデータをバックアップすることを強く推奨します。特に、MODなしの状態でセーブを上書きする前に確認してください。

## 実装メモ

このMODでは、あずかり屋関連で使用される容量値を `960` から `9600` へ変更します。

表示側だけでなく、`cgn.ssr(...)` を通る預け入れ判定側にも同じ容量変更を適用しています。

ゲーム本体のDLL、`Assembly-CSharp.dll`、`GameAssembly.dll` などはこのリポジトリには含めません。各自のゲーム環境にあるBepInEx生成物を参照してビルドします。

## 動作確認環境

開発時に使用した環境:

- Windows x64
- Steam版 シレン6
- BepInEx 6.0.0-be.788
- Unity 2022.3.4f1
- .NET 6

ゲーム更新やBepInEx更新で内部構造が変わると動作しなくなる可能性があります。

## License

MIT License