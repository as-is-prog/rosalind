using Shiorose.Resource;
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
    public abstract class Ghost
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

        /// <summary>
        /// ランダムトークのリスト
        /// </summary>
        public List<Talk> RandomTalks { get; protected set; } = new List<Talk>();


        private Random rand = new Random();

        protected int TALK_INTERVAL = 30;

        protected int talkTimingCount;

        private BaseSaveData __saveData; 
        protected BaseSaveData _saveData {
            get => __saveData;
            set
            {
                __saveData = value;
                talkTimingCount = __saveData.TalkInterval;
            }
        }
        public virtual BaseSaveData SaveData { get => _saveData; }

        public Ghost()
        {
            talkTimingCount = TALK_INTERVAL;
        }

        #region 起動・終了・切り替えイベント
        /// <summary>
        /// 初回起動イベント
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="vanishCount">Reference0 vanishされた回数</param>
        /// <returns></returns>
        public virtual string OnFirstBoot(IDictionary<int, string> reference, int vanishCount = 0)
        {
            return @"\u\s[-1]\h\s[0](Base)OnFirstBoot";
        }

        /// <summary>
        /// 初回起動イベント
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="shellName">Reference0 起動時のシェル名.</param>
        /// <param name="isHalt">Reference6 前回落ちたかどうか</param>
        /// <param name="haltGhostName">Reference7 前回落ちたゴースト名</param>
        /// <returns></returns>
        public virtual string OnBoot(IDictionary<int, string> reference, string shellName = "", bool isHalt = false, string haltGhostName = "")
        {
            return @"\u\s[-1]\h\s[0](Base)OnBoot";
        }

        /// <summary>
        /// 終了時イベント
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="reason">Reference0 終了理由. ユーザならuser, シャットダウンならsystem.</param>
        /// <returns></returns>
        public virtual string OnClose(IDictionary<int, string> reference, string reason = "")
        {
            return @"\u\s[-1]\h\s[0](Base)OnClose";
        }

        /// <summary>
        /// SSPごと終了時イベント
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="reason">Reference0 終了理由. ユーザならuser, シャットダウンならsystem.</param>
        /// <returns></returns>
        public virtual string OnCloseAll(IDictionary<int, string> reference, string reason = "")
        {
            return @"\u\s[-1]\h\s[0](Base)OnCloseAll";
        }

        /// <summary>
        /// 他のゴーストから切り替え時イベント
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="prevSakuraName">Reference0, 切り替え元ゴーストのSakura側の名前</param>
        /// <param name="prevScript">Reference1 切り替え元の繰り替え時Script</param>
        /// <param name="prevGhostName">Reference2 切り替え元のゴーストの名前</param>
        /// <param name="prevGhostPath">Reference3 切り替え元のゴーストのPath</param>
        /// <param name="nowGhostShellName">Reference7 切り替わった（今の）ゴーストのシェル名</param>
        /// <returns></returns>
        public virtual string OnGhostChanged(IDictionary<int, string> reference, string prevSakuraName = "", string prevScript = "", string prevGhostName = "", string prevGhostPath = "", string nowGhostShellName = "")
        {
            return @"\u\s[-1]\h\s[0](Base)OnGhostchanged";
        }

        /// <summary>
        /// 他のゴーストへの切り替え時イベント
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="nextSakuraName">Reference0 切り替え先のゴーストの本体側の名前</param>
        /// <param name="changeReason">Reference1 切り替え理由: 手動だとmanual, システムによる場合だとautomatic</param>
        /// <param name="nextGhostName">Reference2 切り替え先のゴーストの名前</param>
        /// <param name="nextGhostPath">Reference3 切り替え先のゴーストのPath</param>
        /// <returns></returns>
        public virtual string OnGhostChanging(IDictionary<int, string> reference, string nextSakuraName = "", string changeReason = "", string nextGhostName = "", string nextGhostPath = "")
        {
            return @"\u\s[-1]\h\s[0](Base)OnGhostChanging";
        }

        /// <summary>
        /// 他のゴーストから呼び出し時イベント
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="calledSakuraName">Reference0 呼び出し元ゴーストのSakura側の名前</param>
        /// <param name="calledScript">Reference1 呼び出し元ゴーストの呼び出し時Script</param>
        /// <param name="calledGhostName">Reference2 呼び出し元のゴーストの名前</param>
        /// <param name="calledGhostPath">Reference3 呼び出し元のゴーストのPath</param>
        /// <param name="nowGhostShellName">Reference7 呼び出された（今の）ゴーストのシェル名</param>
        /// <returns></returns>
        public virtual string OnGhostCalled(IDictionary<int, string> reference, string calledSakuraName = "", string calledScript = "", string calledGhostName = "", string calledGhostPath = "", string nowGhostShellName = "")
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

        /// <summary>
        /// 他のシェルから切り替わった際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="nowShellName">Reference0 現在のシェル名。</param>
        /// <param name="nowGhostName">Reference1（CROWのみ）Reference0と同じ。（SSPのみ）現在のゴースト名。</param>
        /// <param name="nowShellPath">Reference2（SSPのみ）現在のシェルのパス。</param>
        /// <returns></returns>
        public virtual string OnShellChanged(IDictionary<int, string> reference, string nowShellName, string nowGhostName, string nowShellPath)
        {
            return @"\u\s[-1]\h\s[0](Base)OnShellChanged";
        }

        /// <summary>
        /// 他のシェルへ切り替えた際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="nextShellName">Reference0 切り替わるシェル名。</param>
        /// <param name="nowShellName">Reference1（SSPのみ）切り替わる前（現在）のシェル名。</param>
        /// <param name="nextShellPath">Reference2（SSPのみ）切り替わるシェルのパス。</param>
        /// <returns></returns>
        public virtual string OnShellChanging(IDictionary<int, string> reference, string nextShellName, string nowShellName, string nextShellPath)
        {
            return @"\u\s[-1]\h\s[0](Base)OnShellChanging";
        }

        /// <summary>
        /// 着せ替えが切替えられた際に発生する。
        /// このイベントの後に発生するOnNotifyDressupInfoも参照すること。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="charId">Reference0 着せ替え対象のキャラクターID。</param>
        /// <param name="partsName">Reference1 パーツの名称。</param>
        /// <param name="isEnable">Reference2 有効になった時に1、無効になった時に0。</param>
        /// <param name="category">Reference3 パーツのカテゴリ名。</param>
        /// <returns></returns>
        public virtual string OnDressupChanged(IDictionary<int, string> reference, string charId, string partsName, bool isEnable, string category)
        {
            return @"\u\s[-1]\h\s[0](Base)OnDressupChanged";
        }

        /// <summary>
        /// 他のバルーンから切り替わった際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="nowBalloonName">Reference0 切り替わったバルーン名。</param>
        /// <param name="nowBalloonPath">Reference1 切り替わったバルーンのパス。</param>
        /// <returns></returns>
        public virtual string OnBalloonChange(IDictionary<int, string> reference, string nowBalloonName, string nowBalloonPath)
        {
            return "";
        }

        /// <summary>
        /// 最小化が解除された際に発生。
        /// </summary>
        /// <returns></returns>
        public virtual string OnWindowStateRestore()
        {
            return "";
        }

        /// <summary>
        /// 最小化が指示された際に発生。
        /// </summary>
        /// <returns></returns>
        public virtual string OnWindowStateMinimize()
        {
            return "";
        }

        /// <summary>
        /// 全画面表示のアプリ起動に対するSSP最小化の際に発生。
        /// 通常の最小化時に発生するOnWindowStateMinimizeに上書きされる。
        /// </summary>
        /// <returns></returns>
        public virtual string OnFullScreenAppMinimize()
        {
            return "";
        }

        /// <summary>
        /// 全画面表示のアプリ終了に対するSSP最小化復帰の際に発生。
        /// </summary>
        /// <returns></returns>
        public virtual string OnFullScreenAppRestore()
        {
            return "";
        }

        /// <summary>
        /// 仮想デスクトップが切り替わった際に発生。
        /// 現状Windows 10上でのみ有効。
        /// 実験的機能のため今後仕様が変わる可能性がある。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="state">Reference0 current：現在の仮想デスクトップとゴースト表示中のデスクトップが同じ
        /// hidden：仮想デスクトップが切り替えられ、非表示状態
        /// minimize：ウインドウが最小化されておりどの仮想デスクトップに属するかわからない</param>
        /// <param name="vDesktopId">Reference1 現在の仮想デスクトップID。</param>
        /// <returns></returns>
        public virtual string OnVirtualDesktopChanged(IDictionary<int, string> reference, string state, string vDesktopId)
        {
            return "";
        }

        /// <summary>
        /// ゴーストキャッシュに入った際に発生。
        /// </summary>
        /// <returns></returns>
        public virtual string OnCacheSuspend()
        {
            return "";
        }

        /// <summary>
        /// ゴーストキャッシュから出た際に発生。
        /// </summary>
        /// <returns></returns>
        public virtual string OnCacheRestore()
        {
            return "";
        }

        // TODO: OnInitialize [NOTIFY]

        // TODO: OnDestroy [NOTIFY]

        /// <summary>
        /// サスペンド（スリープ・休止状態（ハイバネーション）両方）解除の際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="resumeReason">Reference0 解除理由。通常normal、システムのスケジュールによる自動解除時auto、バッテリー切れで非常停止した場合critical（Windows Vista以降廃止）</param>
        /// <returns></returns>
        public virtual string OnSysResume(IDictionary<int, string> reference, string resumeReason)
        {
            return "";
        }


        // TODO: OnSysSuspend [NOTIFY]        

        /// <summary>
        /// 本体更新時、ダウンロードが終わりファイル更新を開始する前に発生。このイベントに反応がない場合はOnCloseAll、OnCloseの順に発生する。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="nowVersionNumber">Reference0 更新前のバージョン番号　例：2.3.58</param>
        /// <param name="nowDetailVersionNumber">Reference1 更新前のWindowsエクスプローラで見られるプロパティと同じバージョン番号　例：2.3.58.3000</param>
        /// <returns></returns>
        public virtual string OnBasewareUpdating(IDictionary<int, string> reference, string nowVersionNumber, string nowDetailVersionNumber)
        {
            return "";
        }

        /// <summary>
        /// 本体更新終了後、ゴーストが起動する時に発生。このイベントに反応がない場合はOnBootが発生する。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="updatedVersionNumber">Reference0 更新後のバージョン番号　例：2.3.59</param>
        /// <param name="updatedDetailVersionNumber">Reference1 更新後のWindowsエクスプローラで見られるプロパティと同じバージョン番号　例：2.3.59.3000</param>
        /// <returns></returns>
        public virtual string OnBasewareUpdated(IDictionary<int, string> reference, string updatedVersionNumber, string updatedDetailVersionNumber)
        {
            return "";
        }

        #endregion

        #region 入力ボックスイベント

        /// <summary>
        /// TeachBoxのオープン時イベント
        /// </summary>
        /// <returns></returns>
        public virtual string OnTeachStart()
        {
            return @"\u\s[-1]\h\s[0](Base)OnTeachStart";
        }

        /// <summary>
        /// TeachBoxが閉じられた際のイベント。
        /// このイベントのReferenceはOnUserInputCancelと互換を持たせるために用意されている節がある。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="cancelReason">Reference1 タイムアウトした場合:timeout 閉じられた場合: close</param>
        /// <returns></returns>
        public virtual string OnTeachInputCancel(IDictionary<int,string> reference, string cancelReason = "")
        {
            return @"\u\s[-1]\h\s[0](Base)OnTeachInputCancel";
        }

        /// <summary>
        /// TeachBoxからの入力があった際のイベント
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="teachRecents">Reference* 入力された言葉の履歴（0のほうが古くて末尾の方が新しい）</param>
        /// <returns></returns>
        public virtual string OnTeach(IDictionary<int,string> reference, IEnumerable<string> teachRecents)
        {
            return @"\u\s[-1]\h\s[0](Base)OnTeach";
        }

        /// <summary>
        /// コミュニケートボックスか、他ゴーストからスクリプトを渡された際のイベント
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="senderName">Reference0 送り主の名前。ユーザならuser、ゴーストならSakura側の名前</param>
        /// <param name="script">Reference1 スクリプトの内容</param>
        /// <param name="extInfo">Reference* 拡張情報</param>
        /// <returns></returns>
        public virtual string OnCommunicate(IDictionary<int, string> reference, string senderName = "", string script = "", IEnumerable<string> extInfo = null)
        {
            return @"\u\s[-1]\h\s[0](Base)OnCommunicate";
        }

        /// <summary>
        /// コミュニケートボックスが閉じられた際のイベント。
        /// このイベントのReferenceはOnUserInputCancelと互換を持たせるために用意されている節がある。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="cancelReason">Reference1 タイムアウトした場合:timeout 閉じられた場合: close</param>
        /// <returns></returns>
        public virtual string OnCommunicateInputCancel(IDictionary<int, string> reference, string cancelReason = "")
        {
            return @"\u\s[-1]\h\s[0](Base)OnCommunicateInputCancel";
        }

        /// <summary>
        /// インプットボックスに入力があり、決定された際のイベント
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="inputBoxId">Reference0 インプットボックスのID</param>
        /// <param name="content">Reference1 入力内容</param>
        /// <returns></returns>
        public virtual string OnUserInput(IDictionary<int, string> reference, string inputBoxId = "", string content = "")
        {
            return @"\u\s[-1]\h\s[0](Base)OnUserInput";
        }

        /// <summary>
        /// インプットボックスが閉じられた際のイベント
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="eventId">Reference0 入力イベントのID</param>
        /// <param name="cancelReason">Reference1 タイムアウトした場合:timeout 閉じられた場合: close</param>
        /// <returns></returns>
        public virtual string OnUserInputCancel(IDictionary<int, string> reference, string eventId = "", string cancelReason = "")
        {
            return @"\u\s[-1]\h\s[0](Base)OnUserInputCancel";
        }


        /// <summary>
        /// 入力ボックス展開時に発生する、オートコンプリート機能を使うためのイベント。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="inputBoxType">Reference0 入力ボックスの種類を表す。inputbox, teachbox, communicateboxのどれか</param>
        /// <param name="nextEventId">Reference1 種類がinputboxの場合のみ存在。入力完了時に発生するイベントの識別子</param>
        /// <returns>オートコンプリートしたい文字列をバイト値1区切りでつないだ文字列。</returns>
        public virtual string InputboxAutocomplete(IDictionary<int, string> reference, string inputBoxType = "", string nextEventId = "")
        {
            return "";
        }

        #endregion

        #region ダイアログボックスイベント

        /// <summary>
        /// ファイル関連のダイアログで保存/OKが押された際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="dialogType">Reference0 open・save・folder・colorのいずれか。</param>
        /// <param name="eventId">Reference1 イベントID。</param>
        /// <param name="result">Reference2 ファイルのパスまたはカンマ区切りのRGB値。</param>
        /// <returns></returns>
        public virtual string OnSystemDialog(IDictionary<int, string> reference, string dialogType, string eventId, string result)
        {
            return "";
        }

        /// <summary>
        /// ファイル関連のダイアログでキャンセルが押された際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="dialogType">Reference0 open・save・folder・colorのいずれか。</param>
        /// <param name="eventId">Reference1 イベントID。</param>
        /// <returns></returns>
        public virtual string OnSystemDialogCancel(IDictionary<int, string> reference, string dialogType, string eventId)
        {
            return "";
        }

        /// <summary>
        /// 設定ダイアログのヘルプボタンで項目をクリックした際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="dialogId">Reference0 ダイアログID。種類とページとの対応は以下。
        /// <para>setup …… 一般</para>
        /// <para>ghost …… ゴースト(1)</para>
        /// <para>ghost2 …… ゴースト(2)</para>
        /// <para>folder …… フォルダ</para>
        /// <para>display …… 表示</para>
        /// <para>talk …… 喋り/バルーン</para>
        /// <para>network …… 接続(1)</para>
        /// <para>network2 …… 接続(2)</para>
        /// <para>biff …… POP</para>
        /// <para>application …… 外部アプリ</para>
        /// <para>messenger …… IM</para>
        /// <para>ipmessenger …… IPMessenger</para>
        /// <para>international …… 国際化</para>
        /// <para>developer …… 開発/その他</para>
        /// </param>
        /// <param name="dialogTitle">Reference1 ダイアログタイトル。</param>
        /// <param name="helpType">Reference2 項目タイプ:項目名。</param>
        /// <param name="helpContent">Reference3 項目内容テキスト。</param>
        /// <returns></returns>
        public virtual string OnConfigurationDialogHelp(IDictionary<int, string> reference, string dialogId, string dialogTitle, string helpType, string helpContent)
        {
            return "";
        }

        #endregion

        #region 時間イベント

        /// <summary>
        /// 1秒ごとに発生する時間イベント。
        /// トーク再生不能な時は、Reference3が0になった上でNOTIFYでイベント通知される。返されたスクリプトは無視される。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="uptime">Reference0 OSの連続起動時間（hour）</param>
        /// <param name="isOffScreen">Reference1 見切れ時に1、それ以外は0。</param>
        /// <param name="isOverlap">Reference2 重なり時に1、それ以外は0。</param>
        /// <param name="canTalk">Reference3 トーク再生可能なときに1、それ以外は0。</param>
        /// <param name="leftSecond">Reference4 ※SSPのみ　(OSレベルで)何も操作せず放置されている時間。秒単位。</param>
        /// <returns></returns>
        public virtual string OnSecondChange(IDictionary<int, string> reference, string uptime, bool isOffScreen, bool isOverlap, bool canTalk, string leftSecond)
        {
            // 間隔0なら自発的に発言しない
            if (SaveData.TalkInterval == 0) return "";

            if (--talkTimingCount <= 0 && canTalk)
            {
                talkTimingCount = SaveData.TalkInterval;
                return OnRandomTalk();
            }
            return "";
        }

        /// <summary>
        /// 1分ごとに発生する時間イベント。
        /// トーク再生不能な時は、Reference3が0になった上でNOTIFYでイベント通知される。返されたスクリプトは無視される。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="uptime">Reference0 OSの連続起動時間（hour）</param>
        /// <param name="isOffScreen">Reference1 見切れ時に1、それ以外は0。</param>
        /// <param name="isOverlap">Reference2 重なり時に1、それ以外は0。</param>
        /// <param name="canTalk">Reference3 トーク再生可能なときに1、それ以外は0。</param>
        /// <param name="leftSecond">Reference4 ※SSPのみ　(OSレベルで)何も操作せず放置されている時間。秒単位。</param>
        /// <returns></returns>
        public virtual string OnMinuteChange(IDictionary<int, string> reference, string uptime, bool isOffScreen, bool isOverlap, bool canTalk, string leftSecond)
        {
            return "";
        }

        /// <summary>
        /// 時報イベント。毎正時(?時0分0秒近く)に発生。
        /// トーク再生不能な時はトークできるようになるまで待つ。そのため少しずれた時間に通知されることがある。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="uptime">Reference0 OSの連続起動時間（hour）</param>
        /// <param name="isOffScreen">Reference1 見切れ時に1、それ以外は0。</param>
        /// <param name="isOverlap">Reference2 重なり時に1、それ以外は0。</param>
        /// <param name="canTalk">Reference3 常に1。(トーク再生可能になるまで待つため)</param>
        /// <param name="leftSecond">Reference4 ※SSPのみ　(OSレベルで)何も操作せず放置されている時間。秒単位。</param>
        /// <returns></returns>
        public virtual string OnHourTimeSignal(IDictionary<int, string> reference, string uptime, bool isOffScreen, bool isOverlap, bool canTalk, string leftSecond)
        {
            return "";
        }

        #endregion

        #region 消滅イベント

        /// <summary>
        /// 消滅が指示された際に発生。
        /// </summary>
        /// <returns></returns>
        public virtual string OnVanishSelecting()
        {
            return "";
        }

        /// <summary>
        /// 確認ダイアログでYESが選択された際に発生。
        /// </summary>
        /// <returns></returns>
        public virtual string OnVanishSelected()
        {
            return "";
        }

        /// <summary>
        /// 確認ダイアログでNOが選択された際に発生。
        /// </summary>
        /// <returns></returns>
        public virtual string OnVanishCancel()
        {
            return "";
        }

        /// <summary>
        /// 消滅イベント中ダブルクリックでキャンセルされた際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="pauseScript">Reference0 中断の操作が起きたスクリプト。</param>
        /// <param name="pauseScopeNum">Reference1 中断の操作が起きたバルーンのスコープ番号。本体側0、相方側1、それ以降も。</param>
        /// <param name="pauseCharIndex">Reference2 中断位置。スクリプト先頭からの文字数で表現。文字数はさくらスクリプトのタグも含めたもの。</param>
        /// <returns></returns>
        public virtual string OnVanishButtonHold(IDictionary<int, string> reference, string pauseScript, string pauseScopeNum, string pauseCharIndex)
        {
            return "";
        }

        /// <summary>
        /// 直前のゴーストの消滅により自分に切り替わった際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="vanishSakuraName">Reference0 消滅したゴーストの本体側の名前。</param>
        /// <param name="vanishScript">Reference1 ※SSPのみ　消滅したゴーストのOnVanishSelectedイベントのスクリプト。</param>
        /// <param name="vanishGhostName">Reference2 ※SSPのみ　消滅したゴースト名。</param>
        /// <param name="nowGhostShellName">Reference7 ※SSPのみ　切り替わったゴーストのシェル名。</param>
        /// <returns></returns>
        public virtual string OnVanished(IDictionary<int, string> reference, string vanishSakuraName, string vanishScript, string vanishGhostName, string nowGhostShellName)
        {
            return "";
        }

        /// <summary>
        /// 複数起動中、他のゴーストが消滅した際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="vanishSakuraName">Reference0 消滅したゴーストの本体側の名前。</param>
        /// <param name="vanishScript">Reference1 消滅したゴーストのOnVanishSelectedイベントのスクリプト。</param>
        /// <param name="vanishGhostName">Reference2 消滅したゴースト名。</param>
        /// <param name="vanishGhostShellName">Reference7 切り替わったゴーストのシェル名。</param>
        /// <returns></returns>
        public virtual string OnOtherGhostVanish(IDictionary<int, string> reference, string vanishSakuraName, string vanishScript, string vanishGhostName, string vanishGhostShellName)
        {
            return "";
        }

        #endregion

        #region 選択肢イベント

        // TODO: OnChoiceSelect

        // TODO: OnChoiceSelectEx

        // TODO: OnChoiceEnter

        // TODO: OnChoiceTimeout

        // TODO: OnChoiceHover

        // TODO: OnAnchorSelect

        // TODO: OnAnchorSelectEx

        #endregion

        #region イベント処理補助

        private Talk GetRandomTalk()
        {
            var filtedTalks = RandomTalks.Where(t => t.Filter()).ToArray();

            return filtedTalks[rand.Next(0, filtedTalks.Length)];
        }

        protected virtual string OnRandomTalk()
        {
            return GetRandomTalk().TalkScript();
        }

        #endregion

    }
}
