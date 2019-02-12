using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose
{
    /// <summary>
    /// Rosalindを用いたゴースト開発者が継承すべき、SHIORIイベントに対する挙動を実装するクラス．
    /// 各種イベントをハンドルするメソッドのコメントは 2019/02/10にukadocを参照し参考にしています．
    /// </summary>
    public class Ghost
    {
        /// <summary>
        /// ユーザ名
        /// </summary>
        public string UserName { get; protected set; }
        /// <summary>
        /// 更新URL
        /// </summary>
        public string Homeurl { get; protected set; } = "";
        /// <summary>
        /// Sakura側おすすめサイト
        /// </summary>
        public List<Site> SakuraRecommendSites { get; protected set; } = new List<Site>();
        /// <summary>
        /// Sakura側ポータルサイト
        /// </summary>
        public List<Site> SakuraPortalSites { get; protected set; } = new List<Site>();
        /// <summary>
        /// Kero側おすすめサイト
        /// </summary>
        public List<Site> KeroRecommendSites { get; protected set; } = new List<Site>();
        /// <summary>
        /// Kero側ポータルサイト
        /// </summary>
        public List<Site> KeroPortalSites { get; protected set; } = new List<Site>();

        public List<Func<string>> RandomTalks { get; protected set; } = new List<Func<string>>();

        /// <summary>
        ///
        /// </summary>
        protected Ghost()
        {

        }

        #region 起動・終了・切り替えイベント
        /// <summary>
        /// 初回起動イベント
        /// </summary>
        /// <param name="references">Reference</param>
        /// <param name="vanishCount">Reference0 vanishされた回数</param>
        /// <returns></returns>
        public virtual string OnFirstBoot(IDictionary<int, string> references, int vanishCount = 0)
        {
            return @"\u\s[-1]\h\s[0](Base)OnFirstBoot";
        }

        /// <summary>
        /// 初回起動イベント
        /// </summary>
        /// <param name="references">Reference</param>
        /// <param name="shellName">Reference0 起動時のシェル名.</param>
        /// <param name="isHalt">Reference6 前回落ちたかどうか</param>
        /// <param name="haltGhostName">Reference7 前回落ちたゴースト名</param>
        /// <returns></returns>
        public virtual string OnBoot(IDictionary<int, string> references, string shellName = "", bool isHalt = false, string haltGhostName = "")
        {
            return @"\u\s[-1]\h\s[0](Base)OnBoot";
        }

        /// <summary>
        /// 終了時イベント
        /// </summary>
        /// <param name="references">Reference</param>
        /// <param name="reason">Reference0 終了理由. ユーザならuser, シャットダウンならsystem.</param>
        /// <returns></returns>
        public virtual string OnClose(IDictionary<int, string> references, string reason = "")
        {
            return @"\u\s[-1]\h\s[0](Base)OnClose";
        }

        /// <summary>
        /// SSPごと終了時イベント
        /// </summary>
        /// <param name="references">Reference</param>
        /// <param name="reason">Reference0 終了理由. ユーザならuser, シャットダウンならsystem.</param>
        /// <returns></returns>
        public virtual string OnCloseAll(IDictionary<int, string> references, string reason = "")
        {
            return @"\u\s[-1]\h\s[0](Base)OnCloseAll";
        }

        /// <summary>
        /// 他のゴーストから切り替え時イベント
        /// </summary>
        /// <param name="references">Reference</param>
        /// <param name="prevSakuraName">Reference0, 切り替え元ゴーストのSakura側の名前</param>
        /// <param name="prevScript">Reference1 切り替え元の繰り替え時Script</param>
        /// <param name="prevGhostName">Reference2 切り替え元のゴーストの名前</param>
        /// <param name="prevGhostPath">Reference3 切り替え元のゴーストのPath</param>
        /// <param name="nowGhostShellName">Reference7 切り替わった（今の）ゴーストのシェル名</param>
        /// <returns></returns>
        public virtual string OnGhostChanged(IDictionary<int, string> references, string prevSakuraName = "", string prevScript = "", string prevGhostName = "", string prevGhostPath = "", string nowGhostShellName = "")
        {
            return @"\u\s[-1]\h\s[0](Base)OnGhostchanged";
        }

        /// <summary>
        /// 他のゴーストへの切り替え時イベント
        /// </summary>
        /// <param name="references">Reference</param>
        /// <param name="nextSakuraName">Reference0 切り替え先のゴーストの本体側の名前</param>
        /// <param name="changeReason">Reference1 切り替え理由: 手動だとmanual, システムによる場合だとautomatic</param>
        /// <param name="nextGhostName">Reference2 切り替え先のゴーストの名前</param>
        /// <param name="nextGhostPath">Reference3 切り替え先のゴーストのPath</param>
        /// <returns></returns>
        public virtual string OnGhostChanging(IDictionary<int, string> references, string nextSakuraName = "", string changeReason = "", string nextGhostName = "", string nextGhostPath = "")
        {
            return @"\u\s[-1]\h\s[0](Base)OnGhostChanging";
        }

        /// <summary>
        /// 他のゴーストから呼び出し時イベント
        /// </summary>
        /// <param name="references">Reference</param>
        /// <param name="calledSakuraName">Reference0 呼び出し元ゴーストのSakura側の名前</param>
        /// <param name="calledScript">Reference1 呼び出し元ゴーストの呼び出し時Script</param>
        /// <param name="calledGhostName">Reference2 呼び出し元のゴーストの名前</param>
        /// <param name="calledGhostPath">Reference3 呼び出し元のゴーストのPath</param>
        /// <param name="nowGhostShellName">Reference7 呼び出された（今の）ゴーストのシェル名</param>
        /// <returns></returns>
        public virtual string OnGhostCalled(IDictionary<int, string> references, string calledSakuraName = "", string calledScript = "", string calledGhostName = "", string calledGhostPath = "", string nowGhostShellName = "")
        {
            return @"\u\s[-1]\h\s[0](Base)OnGhostCalled";
        }

        /// <summary>
        /// 他のゴーストの呼び出し時イベント
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="callingSakuraName">Reference0 呼び出すゴーストのSakura側の名前</param>
        /// <param name="callingReason">Reference1 呼び出し理由: 手動だとmanual, システムによる場合だとautomatic</param>
        /// <param name="callingGhostName">Reference2 呼び出すゴーストの名前</param>
        /// <param name="callingGhostPath">Reference3 呼び出すゴーストのPath</param>
        /// <returns></returns>
        public virtual string OnGhostCalling(IDictionary<int, string> reference, string callingSakuraName = "", string callingReason = "", string callingGhostName = "", string callingGhostPath = "")
        {
            return @"\u\s[-1]\h\s[0](Base)OnGhostCalling";
        }

        /// <summary>
        /// 他のゴーストの呼び出し完了時イベント
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="calledSakuraName">Reference0 呼び出し完了したゴーストのSakura側の名前</param>
        /// <param name="calledScript">Reference1 呼び出し完了したゴーストの起動時Script</param>
        /// <param name="calledGhostName">Reference2 呼び出し完了したゴーストの名前</param>
        /// <param name="calledGhostShellName">Reference7 呼び出し完了したゴーストのシェル名</param>
        /// <returns></returns>
        public virtual string OnGhostCallComplete(IDictionary<int, string> reference, string calledSakuraName = "", string calledScript = "", string calledGhostName = "", string calledGhostShellName = "")
        {
            return @"\u\s[-1]\h\s[0](Base)OnGhostCallComplete";
        }

        /// <summary>
        /// ゴースト自身が呼び出したわけではない他のゴーストの起動時イベント
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="bootedSakuraName">Reference0 起動したゴーストのSakura側の名前</param>
        /// <param name="bootedScript">Reference1 起動したゴーストの起動時Script</param>
        /// <param name="bootedGhostName">Reference2 起動したゴーストの名前</param>
        /// <param name="bootedGhostShellName">Reference7 起動したゴーストのシェル名</param>
        /// <returns></returns>
        public virtual string OnOtherGhostBooted(IDictionary<int, string> reference, string bootedSakuraName = "", string bootedScript = "", string bootedGhostName = "", string bootedGhostShellName = "")
        {
            return @"\u\s[-1]\h\s[0](Base)OnOtherGhostBooted";
        }

        /// <summary>
        /// 他のゴーストの切り替わり時イベント
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="prevSakuraName">Reference0 切り替え元ゴーストのSakura側の名前</param>
        /// <param name="nextSakuraName">Reference1 切り替え先ゴーストのSakura側の名前</param>
        /// <param name="prevScript">Reference2 切り替え元ゴーストの切り替え時Script</param>
        /// <param name="nextScript">Reference3 切り替え先ゴーストの切り替え時Script</param>
        /// <param name="prevGhostName">Reference4 切り替え元ゴーストの名前</param>
        /// <param name="nextGhostName">Reference5 切り替え先ゴーストの名前</param>
        /// <param name="prevGhostShellName">Reference14 切り替え元ゴーストのシェル名</param>
        /// <param name="nextGhostShellName">Reference15 切り替え先ゴーストのシェル名</param>
        /// <returns></returns>
        public virtual string OnOtherGhostChanged(IDictionary<int, string> reference, string prevSakuraName = "", string nextSakuraName = "", string prevScript = "", string nextScript = "", string prevGhostName = "", string nextGhostName = "", string prevGhostShellName = "", string nextGhostShellName = "")
        {
            return @"\u\s[-1]\h\s[0](Base)OnOtherGhostChanged";
        }

        /// <summary>
        /// 他のゴーストの終了時イベント
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="closedSakuraName">Reference0 終了したゴーストのSakura側の名前</param>
        /// <param name="closedScript">Reference1 終了したゴーストの終了時Script</param>
        /// <param name="closedGhostName">Reference2 終了したゴーストの名前</param>
        /// <param name="closedGhostShellName">Reference3 終了したゴーストのシェル名</param>
        /// <returns></returns>
        public virtual string OnOtherGhostClosed(IDictionary<int, string> reference, string closedSakuraName = "", string closedScript = "", string closedGhostName = "", string closedGhostShellName = "")
        {
            return @"\u\s[-1]\h\s[0](Base)OnOtherGhostClosed";
        }

        // TODO: OnShellChanged

        // TODO: OnShellChanging

        // TODO: OnDressupChanged

        // TODO: OnShellChanged

        // TODO: OnBalloonChange

        // TODO: OnWindowStateRestore

        // TODO: OnWindowStateMinimize

        // TODO: OnFullScreenAppMinimize

        // TODO: OnFullScreenAppRestore

        // TODO: OnVirtualDesktopChanged

        // TODO: OnCacheSuspend

        // TODO: OnCacheRestore

        // TODO: OnInitialize [NOTIFY]

        // TODO: OnDestroy [NOTIFY]

        // TODO: OnSysResume

        // TODO: OnSysSuspend

        // TODO: OnBasewareUpdating

        // TODO: OnBasewareUpdated

        #endregion



    }
}
