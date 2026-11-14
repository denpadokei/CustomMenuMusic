# Custom Menu Music

[![Build and Release](https://github.com/denpadokei/CustomMenuMusic/actions/workflows/build-and-release.yml/badge.svg)](https://github.com/denpadokei/CustomMenuMusic/actions/workflows/build-and-release.yml)

メニュー画面の BGM を、手持ちの ogg ファイルに差し替える MOD です。上流は Snow1226/CustomMenuMusic (アーカイブ済み) で、現在はこのフォークで Beat Saber 1.39.0 向けにメンテナンスしています。

## 機能

- `UserData\CustomMenuMusic\MenuSongs` に置いた .ogg をランダムに再生する (サブフォルダも検索)
- 曲の再生開始位置をランダムにし、曲が終わると次の曲へ切り替える (Loop オンなら 1 曲をループ)
- `.` (ピリオド) キーで次の曲へスキップする
- リザルト画面の効果音を `ResultSound` の .wav に差し替える (先頭の 1 ファイルのみ使用)
- Now Playing テキストをゲーム内に表示する (左 / 中央 / 右パネル、20 色のプリセット)
- 譜面選択時のプレビュー再生の有無と、プレビュー長の上限を設定できる
- ゲームがフォーカスを失ってもメニュー曲を止めないオプション
- ゲームプレイ設定の「Custom Menu Music」タブで、再生中の曲名表示・SKIP・PLAY NOW PLAY が使える

## 動作環境

| 項目 | 内容 |
| --- | --- |
| Beat Saber | 1.39.0 |
| 依存 MOD | BSIPA 4.3.5 / BS Utils 1.14.2 / BeatSaberMarkupLanguage 1.12.3 / SiraUtil 3.1.12 |

## インストール

1. Beat Saber を 1.39.0 に合わせる (BSManager などのバージョン管理ツールを推奨)
2. BSIPA と上記の依存 MOD をインストールする
3. [Releases](https://github.com/denpadokei/CustomMenuMusic/releases) から `Custom Menu Music-9.0.1-bs1.39.0.zip` をダウンロードし、`Plugins` フォルダをゲームフォルダへ上書きコピーする

## 使い方

1. `UserData\CustomMenuMusic\MenuSongs` に .ogg ファイルを置く (フォルダは初回起動時に作成される)
2. ゲーム内 Settings の「Custom Menu Music」で「Use Custom Menu Songs」をオンにする (既定はオフ)
3. リザルト曲を変える場合は `UserData\CustomMenuMusic\ResultSound` に .wav を置き、「Use Custom Result Sound」をオンにする
4. 曲を飛ばしたいときは `.` キー、またはゲームプレイ設定の「Custom Menu Music」タブにある SKIP を押す
5. Now Playing の表示位置と文字色は設定の「Now Playing」サブメニューから選ぶ

「Use Custom Menu Songs」がオフのときはカスタム譜面フォルダの音源を使う実装ですが、現行コードはそこを `*.egg` で検索するため通常は曲が見つかりません。MenuSongs に ogg を置いてオンにする運用を推奨します。

PLAY NOW PLAY は MenuSongs ではなくカスタム譜面の曲を再生しているときだけ、その譜面を曲リストで選択します。

## 設定

ゲーム内 Settings の「Custom Menu Music」から変更できます。値は BSIPA の生成設定として `UserData` フォルダ内の JSON に保存されます。

| 項目 | 型 | 既定値 | 説明 |
| --- | --- | --- | --- |
| UseCustomMenuSongs | bool | false | `MenuSongs` の曲をメニュー曲に使う |
| Loop | bool | false | 1 曲をループ再生する。オフなら曲の終了時に次の曲へ進む |
| ShowNowPlaying | bool | true | Now Playing テキストを表示する |
| NowPlayingLocation | int | 0 | 表示位置 (0 = 左パネル / 1 = 中央パネル / 2 = 右パネル) |
| NowPlayingColor | int | 0 | 文字色のプリセット番号 (全 20 色、0 = White) |
| CustomResultSound | bool | false | `ResultSound` の .wav をリザルト曲に使う |
| GrobalActiveSound | bool | true | ゲームがフォーカスを失ってもメニュー曲を止めない (綴りは実装どおり `Grobal`) |
| EnableSongPreview | bool | true | 譜面を選んだときのプレビュー再生を有効にする |
| OverrideSongPrevewLength | bool | false | プレビュー長を `MaxSongPreviewLength` に制限する (綴りは実装どおり `Prevew`) |
| MaxSongPreviewLength | float | 10 | プレビューの最大秒数 (UI のスライダーは 0.5〜20 秒) |

## ビルド

```powershell
git clone https://github.com/denpadokei/CustomMenuMusic.git
cd CustomMenuMusic
dotnet build .\CustomMenuMusic\CustomMenuMusic.csproj -c Release
```

`BeatSaberDir` に Beat Saber のインストール先を設定すると、ゲームの参照アセンブリを解決できます。csproj には開発環境のパスが入っているため、`-p:BeatSaberDir="<Beat Saber のフォルダ>"` で上書きしてください。

GitHub Actions の `build-and-release.yml` が push ごとにビルドし、`X.X.X` 形式のタグを push するとリリースを作成します。

## ライセンス

MIT License (Copyright (c) 2020 denpadokei)。上流の [Snow1226/CustomMenuMusic](https://github.com/Snow1226/CustomMenuMusic)、オリジナルの作者 deadlykitten (Steven)、ベースコードを提供した andruzzzhka / Lunikc / lolPants、MOD アイコンを描いた Klouder に感謝します。
