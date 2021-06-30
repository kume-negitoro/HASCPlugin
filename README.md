# HASC_ACC_Plugin

これは iCopusStudio で HASC から出力した CSV 形式の加速度をインポートするためのプラグインです。


# Install

HASCPlugin.dll を plugins フォルダにコピーしてください。


# Usage

プラグインから "HASC (CSV) から加速度を読み込む" を選択し、読み込む CSV ファイルを指定します。


# Build Dependencies

## Windows

VisualStudio などで C# 向けの開発環境をセットアップしてください。


## Linux

[Microsoft](https://docs.microsoft.com/ja-jp/dotnet/core/install/linux-ubuntu) の公式サイトに従って dotnet-sdk をインストールします。


# Build

## Windows

HASCPlugin.csproj を VisualStudio などで開いてビルドしてください。


## Linux

$ `make`
