using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose.Resource.ShioriEvent
{
    /// <summary>
    /// マウスイベントにおけるデバイスタイプ。
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// マウス
        /// </summary>
        MOUSE,
        /// <summary>
        /// タッチパネル
        /// </summary>
        TOUCH
    }

    /// <summary>
    /// インストールイベントにおけるインストール対象の識別子
    /// </summary>
    public enum InstallType
    {
        /// <summary>
        /// シェル
        /// </summary>
        SHELL,
        /// <summary>
        /// ゴースト
        /// </summary>
        GHOST,
        /// <summary>
        /// バルーン
        /// </summary>
        BALLOON,
        /// <summary>
        /// プラグイン
        /// </summary>
        PLUGIN,
        /// <summary>
        /// ヘッドライン
        /// </summary>
        HEADLINE,
        /// <summary>
        /// サプリメント
        /// </summary>
        SUPPLEMENT,
        /// <summary>
        /// ゴーストとバルーン
        /// </summary>
        GHOST_WITH_BALLOON,
        /// <summary>
        /// シェルとバルーン
        /// </summary>
        SHELL_WITH_BALLOON,
        /// <summary>
        /// カレンダーとスキン
        /// </summary>
        CALENDAR_SKIN,
        /// <summary>
        /// カレンダーとプラグイン
        /// </summary>
        CALENDAR_PLUGIN,
        /// <summary>
        /// パッケージ
        /// </summary>
        PACKAGE
    }

    /// <summary>
    /// インストールイベントにおけるインストール失敗理由の識別子
    /// </summary>
    public enum InstallFailureReason
    {
        /// <summary>
        /// lzh形式を展開するdllのロード失敗
        /// </summary>
        UNLHA32,
        /// <summary>
        /// 解凍失敗（ファイル破損）
        /// </summary>
        EXTRACTION,
        /// <summary>
        /// install.txtの不備
        /// </summary>
        INVALID_TYPE,
        /// <summary>
        /// ユーザによるインストール中断
        /// </summary>
        ARTIFICIAL,
        /// <summary>
        /// サポートしていないアーカイブ形式
        /// </summary>
        UNSUPPORTED,
        /// <summary>
        /// (Rosalind独自)未知の失敗理由が渡された
        /// </summary>
        UNKNOWN
    }

    /// <summary>
    /// ネットワーク更新イベントにおける更新対象の識別子
    /// </summary>
    public enum UpdateType
    {
        /// <summary>
        /// ゴースト
        /// </summary>
        GHOST,
        /// <summary>
        /// シェル
        /// </summary>
        SHELL,
        /// <summary>
        /// バルーン
        /// </summary>
        BALLOON,
        /// <summary>
        /// プラグイン
        /// </summary>
        PLUGIN,
        /// <summary>
        /// ヘッドライン
        /// </summary>
        HEADLINE,
        /// <summary>
        /// ベースウェア(CROWのみ)
        /// </summary>
        BASEWARE,
        /// <summary>
        /// (Rosalind独自)Rosalindが未知の識別子
        /// </summary>
        UNKNOWN
    }

    /// <summary>
    /// ネットワーク更新イベントにおける更新理由の識別子
    /// </summary>
    public enum UpdateReason
    {
        /// <summary>
        /// SSPの設定などによる自動更新
        /// </summary>
        AUTO,
        /// <summary>
        /// オーナードローメニューからの選択などによる手動実行。
        /// </summary>
        MANUAL,
        /// <summary>
        /// さくらスクリプトタグによる更新。
        /// </summary>
        SCRIPT,
        /// <summary>
        /// (Rosalind独自)Rosalindが未知の識別子
        /// </summary>
        UNKNOWN
    }



    /// <summary>
    /// SHIORIイベントで渡される各種識別子のenumを拡張するクラス。
    /// </summary>
    internal static class ShioriParamTypeUtil
    {
        public static DeviceType StringToDeviceType(string str)
        {
            switch (str)
            {
                case "mouse":
                    return DeviceType.TOUCH;
                default:
                    return DeviceType.MOUSE;
            }
        }

        public static InstallType StringToInstallType(string str)
        {
            switch (str)
            {
                case "shell":
                    return InstallType.SHELL;
                case "ghost":
                    return InstallType.GHOST;
                case "balloon":
                    return InstallType.BALLOON;
                case "plugin":
                    return InstallType.PLUGIN;
                case "headline":
                    return InstallType.HEADLINE;
                case "supplement":
                    return InstallType.SUPPLEMENT;
                case "ghost with balloon":
                    return InstallType.GHOST_WITH_BALLOON;
                case "shell with balloon":
                    return InstallType.SHELL_WITH_BALLOON;
                case "calendar skin":
                    return InstallType.CALENDAR_SKIN;
                case "calendar plugin":
                    return InstallType.CALENDAR_PLUGIN;
                default:
                    return InstallType.PACKAGE;
            }
        }

        public static InstallFailureReason StringToInstallFailureReason(string str)
        {
            switch (str)
            {
                case "unlha32":
                    return InstallFailureReason.UNLHA32;
                case "extraction":
                    return InstallFailureReason.EXTRACTION;
                case "invalid type":
                    return InstallFailureReason.INVALID_TYPE;
                case "artificial":
                    return InstallFailureReason.ARTIFICIAL;
                case "unsupported":
                    return InstallFailureReason.UNSUPPORTED;
                default:
                    return InstallFailureReason.UNKNOWN;
            }
        }

        public static UpdateType StringToUpdateType(string str)
        {
            switch (str)
            {
                case "shell":
                    return UpdateType.SHELL;
                case "ghost":
                    return UpdateType.GHOST;
                case "balloon":
                    return UpdateType.BALLOON;
                case "plugin":
                    return UpdateType.PLUGIN;
                case "headline":
                    return UpdateType.HEADLINE;
                case "baseware":
                    return UpdateType.BASEWARE;
                default:
                    return UpdateType.UNKNOWN;
            }
        }

        public static UpdateReason StringToUpdateReason(string str)
        {
            switch (str)
            {
                case "auto":
                    return UpdateReason.AUTO;
                case "manual":
                    return UpdateReason.MANUAL;
                case "script"]
                    return UpdateReason.SCRIPT;
                default:
                    return UpdateReason.UNKNOWN;
            }
        }
    }
}
