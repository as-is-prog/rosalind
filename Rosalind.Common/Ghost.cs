using Shiorose.Resource;
using Shiorose.Resource.ShioriEvent;
using Shiorose.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose
{
    /// <summary>
    /// Rosalindを用いたゴースト開発者が継承すべき、SHIORIイベントに対する挙動を実装するクラス．
    /// <remarks>各種イベントをハンドルするメソッドのコメントはukadocの引用 + 一部改変して書いています。</remarks>
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
        /// Shiori RequestのSecurityLevelをもとに、Ghostまで伝えるリクエストを制限します。
        /// </summary>
        public RequestFilterLevel SecurityLevel { get; protected set; } = RequestFilterLevel.LocalOnly;
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
        public List<RandomTalk> RandomTalks { get; protected set; } = new List<RandomTalk>();

        /// <summary>
        /// 次回再生予定のランダムトーク
        /// </summary>
        public Func<string> NextRandomTalk { get; protected set; }

        private Random rand = new Random();

        /// <summary>
        /// 次のランダムトークまでの秒数
        /// </summary>
        protected int talkTimingCount;

        /// <summary>
        /// OnMouseMoveが呼ばれた回数のカウンタ
        /// </summary>
        protected int mouseMoveCount = 0;

        private BaseSaveData __saveData; 
        /// <summary>
        /// （常用非推奨）セーブデータを保持するプロパティ。
        /// セーブデータのロード時のみ使ってください。
        /// </summary>
        protected BaseSaveData _saveData {
            get => __saveData;
            set
            {
                __saveData = value;
                talkTimingCount = __saveData.TalkInterval;
            }
        }

        /// <summary>
        /// セーブデータ。基本的にこのプロパティをオーバーライドしてBaseSaveDataを継承した独自クラスを返すようにすれば便利？
        /// </summary>
        public virtual BaseSaveData SaveData { get => _saveData; }

        
        /// <summary>
        /// 
        /// </summary>
        public virtual SHIORIResource Resource { get; set; } = new SHIORIResource();
        

        /// <summary>
        /// 
        /// </summary>
        public Ghost()
        {
            NextRandomTalk = () => GetRandomTalk().TalkScript();
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
            return "";
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
            return "";
        }

        /// <summary>
        /// 終了時イベント
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="reason">Reference0 終了理由. ユーザならuser, シャットダウンならsystem.</param>
        /// <returns></returns>
        public virtual string OnClose(IDictionary<int, string> reference, string reason = "")
        {
            return "";
        }

        /// <summary>
        /// SSPごと終了時イベント
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="reason">Reference0 終了理由. ユーザならuser, シャットダウンならsystem.</param>
        /// <returns></returns>
        public virtual string OnCloseAll(IDictionary<int, string> reference, string reason = "")
        {
            return "";
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
            return "";
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
            return "";
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
            return "";
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
            return "";
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
            return "";
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
            return "";
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
            return "";
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
            return "";
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
            return "";
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
            return "";
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
            return "";
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
            return "";
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
            return DeferredEvent.Cancel();
        }

        /// <summary>
        /// TeachBoxからの入力があった際のイベント
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="teachRecents">Reference* 入力された言葉の履歴（0のほうが古くて末尾の方が新しい）</param>
        /// <returns></returns>
        public virtual string OnTeach(IDictionary<int,string> reference, IEnumerable<string> teachRecents)
        {
            return DeferredEvent.Exec(teachRecents.LastOrDefault());
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
            return "";
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
            return "";
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
            return DeferredEvent.Exec(content);
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
            return DeferredEvent.Cancel();
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
        /// <para>ランダムトークの処理をここで行っているので、オーバーライドした上でランダムトークをしたい場合はbaseのメソッドも呼び出してください。</para>
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


        /// <summary>
        /// 選択肢が選択された際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="selectedId">Reference0 選択された選択肢のID。</param>
        /// <param name="otherIds">Reference* ※CROWのみ　選択肢の２番目以降のID</param>
        /// <returns></returns>
        public virtual string OnChoiceSelect(IDictionary<int, string> reference, string selectedId, IEnumerable<string> otherIds)
        {
            if (Choice.Count == 0)
                return DeferredEvent.Exec(selectedId);
            else
                return Choice.Select(selectedId)();
        }

        /// <summary>
        /// 選択肢が選択された際に発生。
        /// OnChoiceSelectよりも先に開始する。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="selectedText">Reference0 選択肢のテキスト（ラベル）。</param>
        /// <param name="selectedId">Reference1 選択肢のID。</param>
        /// <param name="extInfo">Reference* 拡張情報(\qタグ内の3番目以降の引数)。</param>
        /// <returns></returns>
        public virtual string OnChoiceSelectEx(IDictionary<int, string> reference, string selectedText, string selectedId, IEnumerable<string> extInfo)
        {
            if (Choice.Count == 0)
                return DeferredEvent.Exec(selectedId);
            else
                return Choice.Select(selectedId)();
        }

        // TODO: OnChoiceEnter [NOTIFY]

        /// <summary>
        /// 選択肢待ち時間がタイムアウトした際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="script">Reference0 タイムアウトしたスクリプト。</param>
        /// <returns></returns>
        public virtual string OnChoiceTimeout(IDictionary<int, string> reference, string script)
        {
            if (Choice.Count == 0)
                return DeferredEvent.Exec("timeout");
            else
                return Choice.Select("timeout")();
        }

        /// <summary>
        /// 選択肢上で静止された際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="hoverdText">Reference0 選択肢のテキスト（ラベル）。</param>
        /// <param name="hoverdId">Reference1 選択肢のID。</param>
        /// <param name="extInfo">Reference* 拡張情報(\qタグ内の3番目以降の引数)。</param>
        /// <returns></returns>
        public virtual string OnChoiceHover(IDictionary<int, string> reference, string hoverdText, string hoverdId, IEnumerable<string> extInfo)
        {
            return "";
        }

        /// <summary>
        /// \_aジャンパがクリックされた瞬間に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="anchorId">Reference0 選択されたジャンパのID。</param>
        /// <returns></returns>
        public virtual string OnAnchorSelect(IDictionary<int, string> reference, string anchorId)
        {
            return "";
        }

        /// <summary>
        /// \_aジャンパがクリックされた瞬間に発生。このイベントにSHIORIが何も返さなかった場合にのみ、続けてOnAnchorSelectが発生する。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="anchorText">Reference0 選択されたジャンパのテキスト。</param>
        /// <param name="anchorId">Reference1 選択されたジャンパのID。</param>
        /// <param name="extInfo">Reference* 拡張情報(\_aタグ内の2番目以降の引数)。</param>
        /// <returns></returns>
        public virtual string OnAnchorSelectEx(IDictionary<int, string> reference, string anchorText, string anchorId, IEnumerable<string> extInfo)
        {
            if (anchorId.StartsWith("http://") || anchorId.StartsWith("https://"))
                return "\\![open,browser," + anchorId + "]";
            else
                return "";
        }

        #endregion

        #region サーフェスイベント

        // TODO: OnSurfaceChange [NOTIFY]        

        /// <summary>
        /// トーク後一定時間経過後に一度発生。
        /// SSPの場合は本体設定の喋りタイムアウトで設定された秒数+１５秒後に発生（つまりバルーンが閉じてから１５秒後）。
        /// ※喋りタイムアウト設定が０のときだけOnSurfaceRestoreが発生するまでバルーンが表示され続ける。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="sakuraSurface">Reference0 本体側の現在サーフェス。</param>
        /// <param name="keroSurface">Reference1 相方側の現在サーフェス。</param>
        /// <returns></returns>
        public virtual string OnSurfaceRestore(IDictionary<int, string> reference, string sakuraSurface, string keroSurface)
        {
            return "";
        }

        /// <summary>
        /// （自分以外の）他のゴーストのサーフェス（表情）が変わった際に通知されます。
        /// 負荷対策のため、標準では無効のイベントです。このイベントを使用するためには、事前に \![set,othersurfacechange,true] で有効化する必要があります。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="ghostName">Reference0 ゴースト名</param>
        /// <param name="sakuraName">Reference1 ゴーストSakura名</param>
        /// <param name="windowId">Reference2 サーフェスが切り替わったウインドウID(Sakura=0)</param>
        /// <param name="newSurfaceId">Reference3 新サーフェスID</param>
        /// <param name="oldSurfaceId">Reference4 旧サーフェスID</param>
        /// <param name="newSurfaceSize">Reference5 新しいサーフェスの大きさ(座標：左,上,右,下)</param>
        /// <returns></returns>
        public virtual string OnOtherSurfaceChange(IDictionary<int, string> reference, string ghostName, string sakuraName, string windowId, string newSurfaceId, string oldSurfaceId, string newSurfaceSize)
        {
            return "";
        }

        #endregion

        #region マウスイベント
        /* 
         * [MEMO]
         * マウスイベントの実装において、数値を渡すcomboCountとcharIdはそれぞれ以下の理由で型を決めた。
         * comboCount => 「N回以上連続クリックされた」など数値として扱うためintに。
         * charId => 「0のキャラ、1のキャラがクリックされた」などあくまで識別子として扱うためstringのままに。
         * 
         * TODO: mouseX, mouseYは保留中。
         */
        /// <summary>
        /// 左・右ボタン（＋中ボタン）でマウスクリックされた時(マウスボタンを1回押して放された瞬間)に、OnMouseUpに反応しない場合に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="mouseX">Reference0 マウスカーソルの x 座標（ローカル座標）</param>
        /// <param name="mouseY">Reference1 マウスカーソルの y 座標（ローカル座標）</param>
        /// <param name="charId">Reference3 本体の場合は0、相方の場合は1。SSP/CROWでは2以降もある。</param>
        /// <param name="partsName">Reference4 当たり判定の識別子。</param>
        /// <param name="buttonName">Reference5 左クリックは0、右クリックは1、中クリックは2(互換仕様：OnMouseClickExへの移行を推奨)。</param>
        /// <param name="deviceType">Reference6 ※SSPのみ　windows7以降、マルチタッチ対応環境のタッチパネル（※タッチパッド不可）からの入力でtouch、マウスなどからの入力でmouse</param>
        /// <returns></returns>
        public virtual string OnMouseClick(IDictionary<int, string> reference, string mouseX, string mouseY, string charId, string partsName, string buttonName, DeviceType deviceType)
        {
            return "";
        }
        /// <summary>
        /// 左・右ボタン以外でマウスクリックされた時(マウスボタンを1回押して放された瞬間)に、OnMouseUpExに反応しない場合に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="mouseX">Reference0 マウスカーソルの x 座標（ローカル座標）</param>
        /// <param name="mouseY">Reference1 マウスカーソルの y 座標（ローカル座標）</param>
        /// <param name="charId">Reference3 本体の場合は0、相方の場合は1。SSP/CROWでは2以降もある。</param>
        /// <param name="partsName">Reference4 当たり判定の識別子。</param>
        /// <param name="buttonName">Reference5 ホイールクリック(または3ボタンマウスの中ボタン)はmiddle、拡張ボタン1(通常「戻る」に割当)はxbutton1、拡張ボタン2(通常「進む」に割当)はxbutton2。</param>
        /// <param name="deviceType">Reference6 windows7以降、マルチタッチ対応環境のタッチパネル（※タッチパッド不可）からの入力でtouch、マウスなどからの入力でmouse</param>
        /// <returns></returns>
        public virtual string OnMouseClickEx(IDictionary<int, string> reference, string mouseX, string mouseY, string charId, string partsName, string buttonName, DeviceType deviceType)
        {
            return "";
        }
        /// <summary>
        /// 左・右ボタンでダブルクリックされた際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="mouseX">Reference0 マウスカーソルの x 座標（ローカル座標）</param>
        /// <param name="mouseY">Reference1 マウスカーソルの y 座標（ローカル座標）</param>
        /// <param name="charId">Reference3 本体の場合は0、相方の場合は1。SSP/CROWでは2以降もある。</param>
        /// <param name="partsName">Reference4 当たり判定の識別子。</param>
        /// <param name="buttonName">Reference5 左クリックは0、右クリックは1。</param>
        /// <param name="deviceType">Reference6 ※SSPのみ　windows7以降、マルチタッチ対応環境のタッチパネル（※タッチパッド不可）からの入力でtouch、マウスなどからの入力でmouse</param>
        /// <returns></returns>
        public virtual string OnMouseDoubleClick(IDictionary<int, string> reference, string mouseX, string mouseY, string charId, string partsName, string buttonName, DeviceType deviceType)
        {
            return "";
        }
        /// <summary>
        /// 左・右ボタン以外でダブルクリックされた際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="mouseX">Reference0 マウスカーソルの x 座標（ローカル座標）</param>
        /// <param name="mouseY">Reference1 マウスカーソルの y 座標（ローカル座標）</param>
        /// <param name="charId">Reference3 本体の場合は0、相方の場合は1。SSP/CROWでは2以降もある。</param>
        /// <param name="partsName">Reference4 当たり判定の識別子。</param>
        /// <param name="buttonName">Reference5 ホイールクリック(または3ボタンマウスの中ボタン)はmiddle、拡張ボタン1(通常「戻る」に割当)はxbutton1、拡張ボタン2(通常「進む」に割当)はxbutton2。</param>
        /// <param name="deviceType">Reference6 windows7以降、マルチタッチ対応環境のタッチパネル（※タッチパッド不可）からの入力でtouch、マウスなどからの入力でmouse</param>
        /// <returns></returns>
        public virtual string OnMouseDoubleClickEx(IDictionary<int, string> reference, string mouseX, string mouseY, string charId, string partsName, string buttonName, DeviceType deviceType)
        {
            return "";
        }

        /// <summary>
        /// 3回以上連続でクリックされた際に発生。
        /// 同イベントに応答が無い場合、通常通りOnMouseClickとOnMouseDoubleClickの流れに戻る。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="mouseX">Reference0 マウスカーソルの x 座標（ローカル座標）</param>
        /// <param name="mouseY">Reference1 マウスカーソルの y 座標（ローカル座標）</param>
        /// <param name="charId">Reference3 本体の場合は0、相方の場合は1。それ以降2～</param>
        /// <param name="partsName">Reference4 当たり判定の識別子。</param>
        /// <param name="buttonName">Reference5 左クリックは0、右クリックは1</param>
        /// <param name="deviceType">Reference6 windows7以降、マルチタッチ対応環境のタッチパネル（※タッチパッド不可）からの入力でtouch、マウスなどからの入力でmouse</param>
        /// <param name="comboCount">Reference7 連続クリック回数</param>
        /// <returns></returns>
        public virtual string OnMouseMultipleClick(IDictionary<int, string> reference, string mouseX, string mouseY, string charId, string partsName, string buttonName, DeviceType deviceType, int comboCount)
        {
            return "";
        }

        /// <summary>
        /// 左・右ボタン以外で3回以上連続でクリックされた際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="mouseX">Reference0 マウスカーソルの x 座標（ローカル座標）</param>
        /// <param name="mouseY">Reference1 マウスカーソルの y 座標（ローカル座標）</param>
        /// <param name="charId">Reference3 本体の場合は0、相方の場合は1。それ以降2～</param>
        /// <param name="partsName">Reference4 当たり判定の識別子。</param>
        /// <param name="buttonName">Reference5 ホイールクリック(または3ボタンマウスの中ボタン)はmiddle、拡張ボタン1(通常「戻る」に割当)はxbutton1、拡張ボタン2(通常「進む」に割当)はxbutton2。</param>
        /// <param name="deviceType">Reference6 windows7以降、マルチタッチ対応環境のタッチパネル（※タッチパッド不可）からの入力でtouch、マウスなどからの入力でmouse</param>
        /// <param name="comboCount">Reference7 連続クリック回数</param>
        /// <returns></returns>
        public virtual string OnMouseMultipleClickEx(IDictionary<int, string> reference, string mouseX, string mouseY, string charId, string partsName, string buttonName, DeviceType deviceType, int comboCount)
        {
            return "";
        }
        /// <summary>
        /// 左・右のマウスボタンが放された際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="mouseX">Reference0 マウスカーソルの x 座標（ローカル座標）</param>
        /// <param name="mouseY">Reference1 マウスカーソルの y 座標（ローカル座標）</param>
        /// <param name="charId">Reference3 本体の場合は0、相方の場合は1。SSP/CROWでは2以降もある。</param>
        /// <param name="partsName">Reference4 当たり判定の識別子。</param>
        /// <param name="buttonName">Reference5 左クリックは0、右クリックは1、中クリックは2(互換仕様：OnMouseClickExへの移行を推奨)。</param>
        /// <param name="deviceType">Reference6 windows7以降、マルチタッチ対応環境のタッチパネル（※タッチパッド不可）からの入力でtouch、マウスなどからの入力でmouse</param>
        /// <returns></returns>
        public virtual string OnMouseUp(IDictionary<int, string> reference, string mouseX, string mouseY, string charId, string partsName, string buttonName, DeviceType deviceType)
        {
            return "";
        }
        /// <summary>
        /// 左・右以外のマウスボタンが放された際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="mouseX">Reference0 マウスカーソルの x 座標（ローカル座標）</param>
        /// <param name="mouseY">Reference1 マウスカーソルの y 座標（ローカル座標）</param>
        /// <param name="Reference3">Reference3 本体の場合は0、相方の場合は1。SSP/CROWでは2以降もある。</param>
        /// <param name="partsName">Reference4 当たり判定の識別子。</param>
        /// <param name="buttonName">Reference5 ホイールクリック(または3ボタンマウスの中ボタン)はmiddle、拡張ボタン1(通常「戻る」に割当)はxbutton1、拡張ボタン2(通常「進む」に割当)はxbutton2。</param>
        /// <param name="deviceType">Reference6 windows7以降、マルチタッチ対応環境のタッチパネル（※タッチパッド不可）からの入力でtouch、マウスなどからの入力でmouse</param>
        /// <returns></returns>
        public virtual string OnMouseUpEx(IDictionary<int, string> reference, string mouseX, string mouseY, string Reference3, string partsName, string buttonName, DeviceType deviceType)
        {
            return "";
        }
        /// <summary>
        /// 左・右のマウスボタンが押された際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="mouseX">Reference0 マウスカーソルの x 座標（ローカル座標）</param>
        /// <param name="mouseY">Reference1 マウスカーソルの y 座標（ローカル座標）</param>
        /// <param name="charId">Reference3 本体の場合は0、相方の場合は1。SSP/CROWでは2以降もある。</param>
        /// <param name="partsName">Reference4 当たり判定の識別子。</param>
        /// <param name="buttonName">Reference5 左クリックは0、右クリックは1、中クリックは2(互換仕様：OnMouseClickExへの移行を推奨)。</param>
        /// <param name="deviceType">Reference6 windows7以降、マルチタッチ対応環境のタッチパネル（※タッチパッド不可）からの入力でtouch、マウスなどからの入力でmouse</param>
        /// <returns></returns>
        public virtual string OnMouseDown(IDictionary<int, string> reference, string mouseX, string mouseY, string charId, string partsName, string buttonName, DeviceType deviceType)
        {
            return "";
        }
        /// <summary>
        /// 左・右以外のマウスボタンが押された際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="mouseX">Reference0 マウスカーソルの x 座標（ローカル座標）</param>
        /// <param name="mouseY">Reference1 マウスカーソルの y 座標（ローカル座標）</param>
        /// <param name="charId">Reference3 本体の場合は0、相方の場合は1。SSP/CROWでは2以降もある。</param>
        /// <param name="partsType">Reference4 当たり判定の識別子。</param>
        /// <param name="buttonName">Reference5 ホイールクリック(または3ボタンマウスの中ボタン)はmiddle、拡張ボタン1(通常「戻る」に割当)はxbutton1、拡張ボタン2(通常「進む」に割当)はxbutton2。</param>
        /// <param name="deviceType">Reference6 windows7以降、マルチタッチ対応環境のタッチパネル（※タッチパッド不可）からの入力でtouch、マウスなどからの入力でmouse</param>
        /// <returns></returns>
        public virtual string OnMouseDownEx(IDictionary<int, string> reference, string mouseX, string mouseY, string charId, string partsType, string buttonName, DeviceType deviceType)
        {
            return "";
        }

        /// <summary>
        /// マウスが移動した際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="mouseX">Reference0 マウスカーソルの x 座標（ローカル座標）</param>
        /// <param name="mouseY">Reference1 マウスカーソルの y 座標（ローカル座標）</param>
        /// <param name="wheelRotation">Reference2 マウスホイールの回転量および回転方向。</param>
        /// <param name="charId">Reference3 本体の場合は0、相方の場合は1。SSP/CROWでは2以降もある。</param>
        /// <param name="partsName">Reference4 当たり判定の識別子。</param>
        /// <param name="deviceType">Reference6 ※SSPのみ　windows7以降、マルチタッチ対応環境のタッチパネル（※タッチパッド不可）からの入力でtouch、マウスなどからの入力でmouse</param>
        /// <returns></returns>
        public virtual string OnMouseMove(IDictionary<int, string> reference, string mouseX, string mouseY, string wheelRotation, string charId, string partsName, DeviceType deviceType)
        {
            if (++mouseMoveCount > 1 && mouseMoveCount % GetCallOnMouseStrokeTiming(partsName, deviceType) == 0)
            {
                return OnMouseStroke(partsName, deviceType);
            }
            return "";
        }
        /// <summary>
        /// マウスホイールが回転した際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="mouseX">Reference0 マウスカーソルの x 座標（ローカル座標）</param>
        /// <param name="mouseY">Reference1 マウスカーソルの y 座標（ローカル座標）</param>
        /// <param name="wheelRotation">Reference2 マウスホイールの回転量および回転方向。</param>
        /// <param name="charId">Reference3 本体の場合は0、相方の場合は1。SSP/CROWでは2以降もある。</param>
        /// <param name="partsName">Reference4 当たり判定の識別子。</param>
        /// <param name="deviceType">Reference6 ※SSPのみ　windows7以降、マルチタッチ対応環境のタッチパネル（※タッチパッド不可）からの入力でtouch、マウスなどからの入力でmouse</param>
        /// <returns></returns>
        public virtual string OnMouseWheel(IDictionary<int, string> reference, string mouseX, string mouseY, string wheelRotation, string charId, string partsName, DeviceType deviceType)
        {
            return "";
        }
        /// <summary>
        /// マウスがキャラクターウインドウに入った際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="mouseX">Reference0 マウスカーソルの x 座標（ローカル座標）</param>
        /// <param name="mouseY">Reference1 マウスカーソルの y 座標（ローカル座標）</param>
        /// <param name="charId">Reference3 本体の場合は0、相方の場合は1。SSP/CROWでは2以降もある。</param>
        /// <param name="partsName">Reference4 当たり判定の識別子。</param>
        /// <param name="deviceType">Reference6 windows7以降、マルチタッチ対応環境のタッチパネル（※タッチパッド不可）からの入力でtouch、マウスなどからの入力でmouse</param>
        /// <returns></returns>
        public virtual string OnMouseEnterAll(IDictionary<int, string> reference, string mouseX, string mouseY, string charId, string partsName, DeviceType deviceType)
        {
            return "";
        }
        /// <summary>
        /// マウスがキャラクターウインドウから出た際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="mouseX">Reference0 マウスカーソルの x 座標（ローカル座標）</param>
        /// <param name="mouseY">Reference1 マウスカーソルの y 座標（ローカル座標）</param>
        /// <param name="charId">Reference3 本体の場合は0、相方の場合は1。SSP/CROWでは2以降もある。</param>
        /// <param name="partsName">Reference4 当たり判定の識別子。</param>
        /// <param name="deviceType">Reference6 windows7以降、マルチタッチ対応環境のタッチパネル（※タッチパッド不可）からの入力でtouch、マウスなどからの入力でmouse</param>
        /// <returns></returns>
        public virtual string OnMouseLeaveAll(IDictionary<int, string> reference, string mouseX, string mouseY, string charId, string partsName, DeviceType deviceType)
        {
            return "";
        }
        /// <summary>
        /// マウスが当たり判定に入った際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="mouseX">Reference0 マウスカーソルの x 座標（ローカル座標）</param>
        /// <param name="mouseY">Reference1 マウスカーソルの y 座標（ローカル座標）</param>
        /// <param name="charId">Reference3 本体の場合は0、相方の場合は1。SSP/CROWでは2以降もある。</param>
        /// <param name="partsName">Reference4 当たり判定の識別子。</param>
        /// <param name="deviceType">Reference6 windows7以降、マルチタッチ対応環境のタッチパネル（※タッチパッド不可）からの入力でtouch、マウスなどからの入力でmouse</param>
        /// <returns></returns>
        public virtual string OnMouseEnter(IDictionary<int, string> reference, string mouseX, string mouseY, string charId, string partsName, DeviceType deviceType)
        {
            mouseMoveCount = 0;
            return "";
        }
        /// <summary>
        /// マウスが当たり判定から出た際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="mouseX">Reference0 マウスカーソルの x 座標（ローカル座標）</param>
        /// <param name="mouseY">Reference1 マウスカーソルの y 座標（ローカル座標）</param>
        /// <param name="charId">Reference3 本体の場合は0、相方の場合は1。SSP/CROWでは2以降もある。</param>
        /// <param name="partsName">Reference4 当たり判定の識別子。</param>
        /// <param name="deviceType">Reference6 windows7以降、マルチタッチ対応環境のタッチパネル（※タッチパッド不可）からの入力でtouch、マウスなどからの入力でmouse</param>
        /// <returns></returns>
        public virtual string OnMouseLeave(IDictionary<int, string> reference, string mouseX, string mouseY, string charId, string partsName, DeviceType deviceType)
        {
            return "";
        }
        /// <summary>
        /// マウスをドラッグし始めた際に発生。ただしパッシブモードでは抑制される。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="mouseX">Reference0 マウスカーソルの x 座標（ローカル座標）</param>
        /// <param name="mouseY">Reference1 マウスカーソルの y 座標（ローカル座標）</param>
        /// <param name="wheelRotation">Reference2 マウスホイールの回転量および回転方向。</param>
        /// <param name="charId">Reference3 本体の場合は0、相方の場合は1。SSP/CROWでは2以降もある。</param>
        /// <param name="partsName">Reference4 当たり判定の識別子。</param>
        /// <param name="buttonName">Reference5 左クリックは0、右クリックは1。</param>
        /// <param name="deviceType">Reference6 windows7以降、マルチタッチ対応環境のタッチパネル（※タッチパッド不可）からの入力でtouch、マウスなどからの入力でmouse</param>
        /// <returns></returns>
        public virtual string OnMouseDragStart(IDictionary<int, string> reference, string mouseX, string mouseY, string wheelRotation, string charId, string partsName, string buttonName, DeviceType deviceType)
        {
            return "";
        }
        /// <summary>
        /// マウスをドラッグし終えた際に発生。ただしパッシブモードでは抑制される。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="mouseX">Reference0 マウスカーソルの x 座標（ローカル座標）</param>
        /// <param name="mouseY">Reference1 マウスカーソルの y 座標（ローカル座標）</param>
        /// <param name="wheelRotation">Reference2 マウスホイールの回転量および回転方向。</param>
        /// <param name="charId">Reference3 本体の場合は0、相方の場合は1。SSP/CROWでは2以降もある。</param>
        /// <param name="partsName">Reference4 当たり判定の識別子。</param>
        /// <param name="buttonName">Reference5 左クリックは0、右クリックは1。</param>
        /// <param name="deviceType">Reference6 windows7以降、マルチタッチ対応環境のタッチパネル（※タッチパッド不可）からの入力でtouch、マウスなどからの入力でmouse</param>
        /// <returns></returns>
        public virtual string OnMouseDragEnd(IDictionary<int, string> reference, string mouseX, string mouseY, string wheelRotation, string charId, string partsName, string buttonName, DeviceType deviceType)
        {
            return "";
        }
        /// <summary>
        /// マウスが静止している際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="mouseX">Reference0 マウスカーソルの x 座標（ローカル座標）</param>
        /// <param name="mouseY">Reference1 マウスカーソルの y 座標（ローカル座標）</param>
        /// <param name="charId">Reference3 本体の場合は0、相方の場合は1。SSP/CROWでは2以降もある。</param>
        /// <param name="partsName">Reference4 当たり判定の識別子。</param>
        /// <param name="deviceType">Reference6 windows7以降、マルチタッチ対応環境のタッチパネル（※タッチパッド不可）からの入力でtouch、マウスなどからの入力でmouse</param>
        /// <returns></returns>
        public virtual string OnMouseHover(IDictionary<int, string> reference, string mouseX, string mouseY, string charId, string partsName, DeviceType deviceType)
        {
            return "";
        }
        /// <summary>
        /// マウスを右ドラッグまたはホイールをドラッグした際に発生。
        /// なおSSP2.3.53以降、タッチパネル環境でOnMouseGestureのReference5がupまたはdownであるようなものに対してSHIORIが何も返さなかった（204 No Contentが返された）場合、代替イベントとしてOnMouseWheelが発生する。詳細はOnMouseWheelの節を参照。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="charId">Reference0 本体の場合は0、相方の場合は1。SSP/CROWでは2以降もある。</param>
        /// <param name="mouseX">Reference1 マウスカーソルの x 座標</param>
        /// <param name="mouseY">Reference1 マウスカーソルの y 座標</param>
        /// <param name="partsName">Reference2 当たり判定の識別子。</param>
        /// <param name="startMouseX">Reference3 右ドラッグを開始したマウスカーソルの x 座標</param>
        /// <param name="startMouseY">Reference3 右ドラッグを開始したマウスカーソルの y 座標</param>
        /// <param name="startPartsName">Reference4 右ドラッグを開始した当たり判定の識別子。</param>
        /// <param name="gestureDirection">
        /// Reference5 ジェスチャー方向。
        /// left_up(左上) up(上) right_up(右上)
        /// left(左) right(右)
        /// left_down(左下) down(下) right_down(右下)
        /// circle.cw(時計回り) circle.ccw(反時計回り)
        /// end(右ボタンをはなした)
        /// </param>
        /// <param name="gestureRotation">Reference6 ジェスチャー移動角度。右0度から反時計回りに360度。</param>
        /// <returns></returns>
        public virtual string OnMouseGesture(IDictionary<int, string> reference, string charId, string mouseX, string mouseY, string partsName, string startMouseX, string startMouseY, string startPartsName, string gestureDirection, string gestureRotation)
        {
            return "";
        }


        #endregion

        #region インストールイベント

        /// <summary>
        /// アーカイブのインストール開始の際に発生。
        /// </summary>
        /// <returns></returns>
        public virtual string OnInstallBegin()
        {
            return @"(インストール開始)";
        }

        /// <summary>
        /// インストールが正常終了した際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="installType">Reference0 インストールした物の識別子。</param>
        /// <param name="installName">Reference1 インストールした物の名前（install.txtのname指定）。</param>
        /// <param name="installName2">Reference2 インストールした物の名前2（ghost with balloonなどの場合のballoon側の名前）。</param>
        /// <returns></returns>
        public virtual string OnInstallComplete(IDictionary<int, string> reference, InstallType installType, string installName, string installName2)
        {
            return string.Format(@"({0}, {1}をインストールしました。)", installType.ToString(), installName);
        }

        /// <summary>
        /// インストールが正常終了した際に発生。
        /// このイベントが無かった場合OnInstallCompleteが発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="installTypes">Reference0 インストールした物の識別子。</param>
        /// <param name="installNames">Reference1 インストールした物の名前。</param>
        /// <param name="installPaths">Reference2 インストールした場所。</param>
        /// <returns></returns>
        public virtual string OnInstallCompleteEx(IDictionary<int, string> reference, InstallType[] installTypes, string[] installNames, string[] installPaths)
        {
            return "";
        }

        /// <summary>
        /// インストールに失敗した際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="failureReason">Reference0 ※失敗理由。</param>
        /// <returns></returns>
        public virtual string OnInstallFailure(IDictionary<int, string> reference, InstallFailureReason failureReason)
        {
            return @"(インストール失敗。\n理由:"+ failureReason.ToString() +")";
        }

        /// <summary>
        /// インストールするファイルが他のゴーストを指名していた際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="sakuraName">Reference0 指定されたゴーストの本体側名（install.txtのaccept指定、つまり本来渡すべき相手）。</param>
        /// <returns></returns>
        public virtual string OnInstallRefuse(IDictionary<int, string> reference, string sakuraName)
        {
            return @"(インストール失敗。これは"+ sakuraName +"専用です。)";
        }

        /// <summary>
        /// インストールするファイルが他のゴーストを指名していた際、かつ、指名対象ゴーストが一緒に起動していた時に発生。以降のインストールイベントは指名対象ゴースト側に発生する。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="sakuraName">Reference0 指定されたゴーストの本体側名（install.txtのaccept指定、つまり本来渡すべき相手）。</param>
        /// <returns></returns>
        public virtual string OnInstallReroute(IDictionary<int, string> reference, string sakuraName)
        {
            return @"(これは" + sakuraName + "専用です。" + sakuraName + "にインストールします。)";
        }

        #endregion

        #region ファイルドロップイベント
        /// <summary>
        /// ファイルをドラッグしたままのカーソルがゴースト上に乗った際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="filePath">Reference0 ドラッグしているファイルパス。</param>
        /// <param name="charId">Reference1 ドロップされたキャラクターのスコープ番号。本体側0、相方1、3人目以降は2以降。</param>
        /// <returns></returns>
        public virtual string OnFileDropping(IDictionary<int, string> reference, string filePath, string charId)
        {
            return "";
        }

        /// <summary>
        /// ディレクトリがDnD された際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="dirPath">Reference0 ドロップされたディレクトリのパス。</param>
        /// <param name="charId">Reference1 ドロップされたキャラクターのスコープ番号。本体側0、相方1、3人目以降は2以降。</param>
        /// <returns></returns>
        public virtual string OnDirectoryDrop(IDictionary<int, string> reference, string dirPath, string charId)
        {
            return "";
        }

        /// <summary>
        /// 画像ファイルのDnDによって壁紙が変更された際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="filePath">Reference0 ドロップされたファイルパス。</param>
        /// <returns></returns>
        public virtual string OnWallpaperChange(IDictionary<int, string> reference, string filePath)
        {
            return "";
        }

        /// <summary>
        /// ファイルがDnDされた際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="filePaths">Reference0 ドロップされたファイルパスを（複数あればbyte値1で区切って）返す。</param>
        /// <param name="charId">Reference1 ドロップされたキャラクターのスコープ番号。本体側0、相方1、3人目以降は2以降。</param>
        /// <returns></returns>
        public virtual string OnFileDropEx(IDictionary<int, string> reference, string[] filePaths, string charId)
        {
            return "";
        }

        /// <summary>
        /// ゴーストフォルダがDnDされた際に発生。
        /// </summary>
        /// <returns></returns>
        public virtual string OnUpdatedataCreating()
        {
            return "";
        }

        /// <summary>
        /// updates2.dauが作成された際に発生。
        /// </summary>
        /// <returns></returns>
        public virtual string OnUpdatedataCreated()
        {
            return @"(updates2.dau作成完了)";
        }

        /// <summary>
        /// install.txtの入ったゴーストフォルダがDnDされた際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="objectName">Reference0 narにするオブジェクト名。</param>
        /// <param name="fileName">Reference1 narに出力されたファイル名。</param>
        /// <param name="installType">Reference2 ※識別子。インストールの識別子参照。</param>
        /// <returns></returns>
        public virtual string OnNarCreating(IDictionary<int, string> reference, string objectName, string fileName, InstallType installType)
        {
            return "";
        }

        /// <summary>
        /// narファイルが作成された際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="objectName">Reference0 narにするオブジェクト名。</param>
        /// <param name="fileName">Reference1 narに出力されたファイル名。</param>
        /// <param name="installType">Reference2 ※識別子。インストールの識別子参照。</param>
        /// <returns></returns>
        public virtual string OnNarCreated(IDictionary<int, string> reference, string objectName, string fileName, InstallType installType)
        {
            return @"(\_?"+fileName+@"\_?としてnarファイル作成完了)";
        }


        #endregion

        #region URLドロップイベント
        /// <summary>
        /// URLがドロップされた際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="urlPath">Reference0 URLパス。</param>
        /// <returns></returns>
        public virtual string OnURLDropping(IDictionary<int, string> reference, string urlPath)
        {
            return "";
        }

        /// <summary>
        /// ドロップされたURLを受領し終わった際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="filePath">Reference0 ダウンロードし終わったファイルのローカルパス。</param>
        /// <returns></returns>
        public virtual string OnURLDropped(IDictionary<int, string> reference, string filePath)
        {
            return @"("+filePath+"をダウンロード完了)";
        }
        /// <summary>
        /// ドロップされたURLの受領に失敗したかキャンセルされた際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="filePath">Reference0 ダウンロードし終わった分のファイルのローカルパス。</param>
        /// <param name="failureReason">Reference1 ユーザーのダブルクリックによって中断した場合、artificial</param>
        /// <returns></returns>
        public virtual string OnURLDropFailure(IDictionary<int, string> reference, string filePath, string failureReason)
        {
            return @"(ダウンロード失敗 理由:"+failureReason+")";
        }

        /// <summary>
        /// URLがキャラクターウィンドウにドラッグ＆ドロップされた際に通知されます。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="urlPath">Reference0 URLパス。</param>
        /// <param name="charId">Reference1 本体の場合は0、相方の場合は1。SSP/CROWでは2以降もある。</param>
        /// <param name="mimeType">Reference2 ドロップされたURLのMIMEタイプ</param>
        /// <returns></returns>
        public virtual string OnURLQuery(IDictionary<int, string> reference, string urlPath, string charId, string mimeType)
        {
            return "";
        }

        #endregion

        #region ネットワーク更新イベント
        /// <summary>
        /// ネットワーク更新開始が指示された際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="ghostName">Reference0 ゴースト名。</param>
        /// <param name="fullPath">Reference1 フルパス。</param>
        /// <param name="updateType">Reference3 更新対象の識別子</param>
        /// <param name="updateReason">Reference4 SSP 2.3：※更新実行理由。</param>
        /// <returns></returns>
        public virtual string OnUpdateBegin(IDictionary<int, string> reference, string ghostName, string fullPath, UpdateType updateType, UpdateReason updateReason)
        {
            return "(ネットワーク更新開始)";
        }

        /// <summary>
        /// 更新ファイルが確認された際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="updateCount">Reference0 更新を行うファイルの総数。</param>
        /// <param name="updateFileNames">Reference1 更新されたファイル名のリスト。</param>
        /// <param name="updateType">Reference3 更新対象種別。(shell ghost balloon headline plugin)</param>
        /// <param name="updateReason">Reference4 SSP 2.3：更新実行理由。</param>
        /// <returns></returns>
        public virtual string OnUpdateReady(IDictionary<int, string> reference, int updateCount, string[] updateFileNames, UpdateType updateType, UpdateReason updateReason)
        {
            return "("+(updateCount)+"個の更新を確認)";
        }

        /// <summary>
        /// ネットワーク更新が成功し完了した際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="isUpdated">Reference0 更新があればtrue, なければfalse</param>
        /// <param name="updateFileNames">Reference1 更新されたファイル名のリスト。</param>
        /// <param name="updateType">Reference3 SSP：更新対象種別</param>
        /// <param name="updateReason">Reference4 SSP 2.3：更新実行理由。</param>
        /// <returns></returns>
        public virtual string OnUpdateComplete(IDictionary<int, string> reference, bool isUpdated, string[] updateFileNames, UpdateType updateType, UpdateReason updateReason)
        {
            return isUpdated ? "(ネットワーク更新完了)" : "(更新ファイルなし)";
        }

        /// <summary>
        /// ネットワーク更新に失敗した際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="failureReason">Reference0 失敗理由
        /// <para> timeout -> タイムアウト</para>
        /// <para> too slow -> 回線が非常に重い</para>
        /// <para> md5 miss -> MD5不一致</para>
        /// <para> artificial -> ユーザによる中断</para>
        /// <para> 404等 -> 各ステータスコードによる失敗</para>
        /// <para> fileio -> ストレージの容量不足</para>
        /// <para> readonly -> 更新対象が読み取り専用だった</para>
        /// </param>
        /// <param name="failureFileName">Reference1 カンマでセパレートされた更新されたファイル名のリスト。</param>
        /// <param name="updateType">Reference3 SSP：※更新対象種別</param>
        /// <param name="updateReason">Reference4 SSP 2.3：※更新実行理由。</param>
        /// <returns></returns>
        public virtual string OnUpdateFailure(IDictionary<int, string> reference, string failureReason, string failureFileName, UpdateType updateType, UpdateReason updateReason)
        {
            return "(ネットワーク更新失敗 理由:"+failureReason+")";
        }
        /// <summary>
        /// ファイルダウンロード開始の際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="downloadFileName">Reference0 ダウンロードするファイル名。</param>
        /// <param name="downloadProgressCount">Reference1 ダウンロード中の更新ファイルが何番目か。</param>
        /// <param name="downloadTotalCount">Reference2 更新を行うファイルの総数。</param>
        /// <param name="updateType">Reference3 （SSPのみ）※更新対象種別。</param>
        /// <param name="updateReason">Reference4 SSP 2.3：※更新実行理由。</param>
        /// <returns></returns>
        public virtual string OnUpdateOnDownloadBegin(IDictionary<int, string> reference, string downloadFileName, int downloadProgressCount, int downloadTotalCount, UpdateType updateType, UpdateReason updateReason)
        {
            return "("+downloadFileName+"をダウンロード開始 "+downloadProgressCount+"/"+downloadTotalCount+")";
        }

        /// <summary>
        /// MD5の照合開始の際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="fileName">Reference0 比較するファイル名。</param>
        /// <param name="exceptedMd5">Reference1 正しいMD5値。</param>
        /// <param name="actualMd5">Reference2 落としたファイルのMD5値。</param>
        /// <param name="updateType">Reference3 SSP　※更新対象種別</param>
        /// <param name="updateReason">Reference4 SSP 2.3：※更新実行理由。</param>
        /// <returns></returns>
        public virtual string OnUpdateOnMD5CompareBegin(IDictionary<int, string> reference, string fileName, string exceptedMd5, string actualMd5, UpdateType updateType, UpdateReason updateReason)
        {
            return "";
        }

        /// <summary>
        /// MD5が一致した際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="fileName">Reference0 比較するファイル名。</param>
        /// <param name="exceptedMd5">Reference1 正しいMD5値。</param>
        /// <param name="actualMd5">Reference2 落としたファイルのMD5値。</param>
        /// <param name="updateType">Reference3 SSP　※更新対象種別</param>
        /// <param name="updateReason">Reference4 SSP 2.3：※更新実行理由。</param>
        /// <returns></returns>
        public virtual string OnUpdateOnMD5CompareComplete(IDictionary<int, string> reference, string fileName, string exceptedMd5, string actualMd5, UpdateType updateType, UpdateReason updateReason)
        {
            return "";
        }

        /// <summary>
        /// MD5が一致しなかった際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="fileName">Reference0 比較するファイル名。</param>
        /// <param name="exceptedMd5">Reference1 正しいMD5値。</param>
        /// <param name="actualMd5">Reference2 落としたファイルのMD5値。</param>
        /// <param name="updateType">Reference3 SSP　※更新対象種別</param>
        /// <param name="updateReason">Reference4 SSP 2.3：※更新実行理由。</param>
        /// <returns></returns>
        public virtual string OnUpdateOnMD5CompareFailure(IDictionary<int, string> reference, string fileName, string exceptedMd5, string actualMd5, UpdateType updateType, UpdateReason updateReason)
        {
            return "";
        }

        /// <summary>
        /// ゴースト以外のネットワーク更新開始が指示された際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="ghostName">Reference0 ゴースト名。</param>
        /// <param name="fullPath">Reference1 フルパス。</param>
        /// <param name="updateType">Reference3 更新対象の識別子</param>
        /// <param name="updateReason">Reference4 SSP 2.3：※更新実行理由。</param>
        /// <returns></returns>
        public virtual string OnUpdateOtherBegin(IDictionary<int, string> reference, string ghostName, string fullPath, UpdateType updateType, UpdateReason updateReason)
        {
            return "";
        }

        /// <summary>
        /// ゴースト以外の更新ファイルが確認された際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="updateCount">Reference0 更新を行うファイルの総数。</param>
        /// <param name="updateFileNames">Reference1 更新されたファイル名のリスト。</param>
        /// <param name="updateType">Reference3 更新対象種別。(shell ghost balloon headline plugin)</param>
        /// <param name="updateReason">Reference4 SSP 2.3：更新実行理由。</param>
        /// <returns></returns>
        public virtual string OnUpdateOtherReady(IDictionary<int, string> reference, int updateCount, string[] updateFileNames, UpdateType updateType, UpdateReason updateReason)
        {
            return "";
        }

        /// <summary>
        /// ゴースト以外のネットワーク更新が成功し完了した際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="isUpdated">Reference0 更新があればtrue, なければfalse</param>
        /// <param name="updateFileNames">Reference1 更新されたファイル名のリスト。</param>
        /// <param name="updateType">Reference3 SSP：更新対象種別</param>
        /// <param name="updateReason">Reference4 SSP 2.3：更新実行理由。</param>
        /// <returns></returns>
        public virtual string OnUpdateOtherComplete(IDictionary<int, string> reference, bool isUpdated, string[] updateFileNames, UpdateType updateType, UpdateReason updateReason)
        {
            return "";
        }

        /// <summary>
        /// ゴースト以外のネットワーク更新に失敗した際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="failureReason">Reference0 失敗理由
        /// <para> timeout -> タイムアウト</para>
        /// <para> too slow -> 回線が非常に重い</para>
        /// <para> md5 miss -> MD5不一致</para>
        /// <para> artificial -> ユーザによる中断</para>
        /// <para> 404等 -> 各ステータスコードによる失敗</para>
        /// <para> fileio -> ストレージの容量不足</para>
        /// <para> readonly -> 更新対象が読み取り専用だった</para>
        /// </param>
        /// <param name="failureFileName">Reference1 カンマでセパレートされた更新されたファイル名のリスト。</param>
        /// <param name="updateType">Reference3 SSP：※更新対象種別</param>
        /// <param name="updateReason">Reference4 SSP 2.3：※更新実行理由。</param>
        /// <returns></returns>
        public virtual string OnUpdateOtherFailure(IDictionary<int, string> reference, string failureReason, string failureFileName, UpdateType updateType, UpdateReason updateReason)
        {
            return "";
        }

        /// <summary>
        /// ゴースト以外のファイルダウンロード開始の際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="downloadFileName">Reference0 ダウンロードするファイル名。</param>
        /// <param name="downloadProgressCount">Reference1 ダウンロード中の更新ファイルが何番目か。</param>
        /// <param name="downloadTotalCount">Reference2 更新を行うファイルの総数。</param>
        /// <param name="updateType">Reference3 （SSPのみ）※更新対象種別。</param>
        /// <param name="updateReason">Reference4 SSP 2.3：※更新実行理由。</param>
        /// <returns></returns>
        public virtual string OnUpdateOtherOnDownloadBegin(IDictionary<int, string> reference, string downloadFileName, int downloadProgressCount, int downloadTotalCount, UpdateType updateType, UpdateReason updateReason)
        {
            return "";
        }

        /// <summary>
        /// ゴースト以外のMD5の照合開始の際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="fileName">Reference0 比較するファイル名。</param>
        /// <param name="exceptedMd5">Reference1 正しいMD5値。</param>
        /// <param name="actualMd5">Reference2 落としたファイルのMD5値。</param>
        /// <param name="updateType">Reference3 SSP　※更新対象種別</param>
        /// <param name="updateReason">Reference4 SSP 2.3：※更新実行理由。</param>
        /// <returns></returns>
        public virtual string OnUpdateOtherOnMD5CompareBegin(IDictionary<int, string> reference, string fileName, string exceptedMd5, string actualMd5, UpdateType updateType, UpdateReason updateReason)
        {
            return "";
        }

        /// <summary>
        /// ゴースト以外のMD5が一致した際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="fileName">Reference0 比較するファイル名。</param>
        /// <param name="exceptedMd5">Reference1 正しいMD5値。</param>
        /// <param name="actualMd5">Reference2 落としたファイルのMD5値。</param>
        /// <param name="updateType">Reference3 SSP　※更新対象種別</param>
        /// <param name="updateReason">Reference4 SSP 2.3：※更新実行理由。</param>
        /// <returns></returns>
        public virtual string OnUpdateOtherOnMD5CompareComplete(IDictionary<int, string> reference, string fileName, string exceptedMd5, string actualMd5, UpdateType updateType, UpdateReason updateReason)
        {
            return "";
        }

        /// <summary>
        /// ゴースト以外のMD5が一致しなかった際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="fileName">Reference0 比較するファイル名。</param>
        /// <param name="exceptedMd5">Reference1 正しいMD5値。</param>
        /// <param name="actualMd5">Reference2 落としたファイルのMD5値。</param>
        /// <param name="updateType">Reference3 SSP　※更新対象種別</param>
        /// <param name="updateReason">Reference4 SSP 2.3：※更新実行理由。</param>
        /// <returns></returns>
        public virtual string OnUpdateOtherOnMD5CompareFailure(IDictionary<int, string> reference, string fileName, string exceptedMd5, string actualMd5, UpdateType updateType, UpdateReason updateReason)
        {
            return "";
        }

        /// <summary>
        /// ネットワーク更新のチェックに成功した際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="isUpdate">Reference0 更新があればtrue、なければfalse。</param>
        /// <param name="updateFileNames">Reference1 カンマでセパレートされた更新されたファイル名のリスト。</param>
        /// <param name="updateType">Reference3 ※更新対象種別</param>
        /// <param name="updateReason">Reference4 SSP 2.3：更新実行理由。scriptのみ。</param>
        /// <returns></returns>
        public virtual string OnUpdateCheckComplete(IDictionary<int, string> reference, bool isUpdate, string[] updateFileNames, UpdateType updateType, UpdateReason updateReason)
        {
            return "(ネットワーク更新"+(isUpdate ? "あり" : "なし")+")";
        }

        /// <summary>
        /// ネットワーク更新のチェックに失敗した際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="failureReason">Reference0 失敗理由
        /// <para> timeout -> タイムアウト</para>
        /// <para> too slow -> 回線が非常に重い</para>
        /// <para> md5 miss -> MD5不一致</para>
        /// <para> artificial -> ユーザによる中断</para>
        /// <para> 404等 -> 各ステータスコードによる失敗</para>
        /// <para> fileio -> ストレージの容量不足</para>
        /// <para> readonly -> 更新対象が読み取り専用だった</para>
        /// </param>
        /// <param name="updateReason">Reference4 SSP 2.3：更新実行理由。scriptのみ。</param>
        /// <returns></returns>
        public virtual string OnUpdateCheckFailure(IDictionary<int, string> reference, string failureReason, UpdateReason updateReason)
        {
            return @"(ネットワーク更新のチェック失敗 理由: "+failureReason+")";
        }

        /// <summary>
        /// ネットワーク更新を実行した際に発生し、バルーン・シェル一括更新機能も含めてすべての更新結果を一括で通知する。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="updateResults">Reference* 更新を実行した順に、更新結果が以下のフォーマットで入っている
        /// <para>（\1はバイト値1）</para>
        /// <para>(※1)[\1] (※2)[\1] (※3)[\1] (※4)</para> 
        /// <para>または</para>
        /// <para>(※1)[\1] (※2)[\1] (※3)</para>
        ///
        /// <para>※1……ネットワーク更新対象の種別</para>
        /// <para>※2……成功時「OK」　失敗時「NG」</para>
        /// <para>※3……成功した場合は更新ファイル数(更新ファイルなし= noneの場合0)、失敗した場合は失敗理由。</para>
        /// <para>※4……失敗して、かつ失敗原因のファイルがわかる場合、そのファイル名。</para>
        /// </param>
        /// <returns></returns>
        public virtual string OnUpdateResult(IDictionary<int, string> reference, IDictionary<int, string> updateResults)
        {
            return "";
        }

        /// <summary>
        /// ゴーストエクスプローラからネットワーク更新を実行した際に発生。OnUpdateResultのエクスプローラ版。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="updateResults">Reference* 更新を実行した順に、更新結果が以下のフォーマットで入っている
        /// <para>（\1はバイト値1）</para>
        /// <para>(※1)[\1] (※2)[\1] (※3)[\1] (※4)</para> 
        /// <para>または</para>
        /// <para>(※1)[\1] (※2)[\1] (※3)</para>
        ///
        /// <para>※1……ネットワーク更新対象の種別</para>
        /// <para>※2……成功時「OK」　失敗時「NG」</para>
        /// <para>※3……成功した場合は更新ファイル数(更新ファイルなし= noneの場合0)、失敗した場合は失敗理由。</para>
        /// <para>※4……失敗して、かつ失敗原因のファイルがわかる場合、そのファイル名。</para>
        /// </param>
        /// <returns></returns>
        public virtual string OnUpdateResultExplorer(IDictionary<int, string> reference, IDictionary<int, string> updateResults)
        {
            return "";
        }


        #endregion

        #region 時計合わせイベント
        /// <summary>
        /// 時計合わせが開始が指示された際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="serverName">Reference0 接続先サーバ名。</param>
        /// <returns></returns>
        public virtual string OnSNTPBegin(IDictionary<int, string> reference, string serverName)
        {
            return "(時計合わせ開始)";
        }
        /// <summary>
        /// サーバに接続が確立された際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="serverName">Reference0 接続先サーバ名。</param>
        /// <param name="serverTime">Reference1 カンマでセパレートされたサーバ側の現時刻。</param>
        /// <param name="localTime">Reference2 カンマでセパレートされたローカル側の現時刻</param>
        /// <param name="diffSecond">Reference3 サーバとローカルの時刻のずれ。（秒単位）</param>
        /// <param name="diffMSecond">Reference4 ※SSPのみ　サーバとローカルの時刻のずれ。（ミリ秒単位）</param>
        /// <returns></returns>
        public virtual string OnSNTPCompare(IDictionary<int, string> reference, string serverName, DateTime? serverTime, DateTime? localTime, int diffSecond, int diffMSecond)
        {
            const string SET = "合わせる";
            const string NO_SET = "そのままにする";
            return new TalkBuilder().Append("("+diffSecond).AppendLine("秒のずれています)")
                                    .AppendLine("時計を合わせますか？")
                                    .HalfLine()
                                    .Marker().AppendChoice(SET).LineFeed()
                                    .Marker().AppendChoice(NO_SET).LineFeed()
                                    .Build()
                                    .ContinueWith((id) =>
                                    {
                                        if (id == SET)
                                            return "\\6";
                                        else
                                            return "キャンセルしました。";
                                    });
        }
        /// <summary>
        /// ローカル時刻をサーバ時刻に修正した際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="serverName">Reference0 接続先サーバ名。</param>
        /// <param name="serverTime">Reference1 カンマでセパレートされたサーバ側の現時刻。</param>
        /// <param name="localTime">Reference2 カンマでセパレートされたローカル側の現時刻</param>
        /// <param name="diffSecond">Reference3 サーバとローカルの時刻のずれ。（秒単位）</param>
        /// <returns></returns>
        public virtual string OnSNTPCorrect(IDictionary<int, string> reference, string serverName, DateTime? serverTime, DateTime? localTime, int diffSecond)
        {
            return "(時刻を修正しました)";
        }

        /// <summary>
        /// 時計合わせに失敗した際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="serverName">Reference0 接続先サーバ名。</param>
        /// <returns></returns>
        public virtual string OnSNTPFailure(IDictionary<int, string> reference, string serverName)
        {
            return "(時計合わせに失敗しました)";
        }

        #endregion

        #region メールチェックイベント
        /// <summary>
        /// メールチェック開始が指示された際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="accountName">Reference2 チェックするメールサーバ名、SSP、CROWはアカウント名。</param>
        /// <returns></returns>
        public virtual string OnBIFFBegin(IDictionary<int, string> reference, string accountName)
        {
            return "("+accountName+"のメールチェック開始)";
        }

        /// <summary>
        /// メールチェックが成功した際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="spoolMailCount">Reference0 スプールされているメールの通数、１オリジン表記。</param>
        /// <param name="spoolMailByte">Reference1 スプールされているメールのバイト数。</param>
        /// <param name="mailServerName">Reference2 チェックするメールサーバ名。</param>
        /// <param name="newMailDiff">Reference3 メールの新着差分。</param>
        /// <param name="topResults">Reference4 全メールのTopResult。POP通信全ヘッダリスト。メールごとバイト値2区切りの、ヘッダごとバイト値1区切り（通常はReference7の情報を利用すればよい）。</param>
        /// <param name="listResult">Reference5 ListResult。メールサーバ上のID。POPコマンドの「LIST」の結果に相当する内容。</param>
        /// <param name="uidlResult">Reference6 UidlResult。メールを区別するためのID。POPコマンドの「UIDL」の結果に相当する内容。</param>
        /// <param name="mailSender">Reference7 メールの送信者。</param>
        /// <param name="mailTitle">Reference7 メールのタイトル。</param>
        /// <returns></returns>
        public virtual string OnBIFFComplete(IDictionary<int, string> reference, int spoolMailCount, int spoolMailByte, string mailServerName, string newMailDiff, string topResults, string listResult, string uidlResult, string mailSender, string mailTitle)
        {
            return string.Format("(送信者:{0} タイトル: {1} のメールがあります)", mailSender, mailTitle);
        }

        /// <summary>
        /// メールチェックに成功した際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="failureReason">Reference0 ※失敗理由</param>
        /// <param name="accountName">Reference2 チェックするメールサーバ名、SSP、CROWはアカウント名。</param>
        /// <returns></returns>
        public virtual string OnBIFFFailure(IDictionary<int, string> reference, string failureReason, string accountName)
        {
            return "("+accountName+"のメールチェックに失敗: "+failureReason+")";
        }


        #endregion

        #region ヘッドライン/RSSセンスイベント

        #endregion

        #region カレンダーイベント

        #endregion

        #region 単体イベント
        /// <summary>
        /// 本体がアクティブな状態でキーが入力された際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="keyName">Reference0 入力されたキー。(キートップに書かれた文字)</param>
        /// <param name="keyCode">Reference1 入力されたキー。(Win32仮想キーコード)</param>
        /// <param name="pressCount">Reference2 ※SSPのみ　キーの押しっぱなしカウント。</param>
        /// <param name="charId">Reference3 ※SSPのみ　キーイベントが来たキャラクターウインドウID。</param>
        /// <param name="extraKeys">Reference4 ※SSPのみ　修飾キーがカンマ区切りで列挙される。例："ctrl,alt"</param>
        /// <returns></returns>
        public virtual string OnKeyPress(IDictionary<int, string> reference, string keyName, string keyCode, int pressCount, string charId, string extraKeys)
        {
            return "";
        }

        /// <summary>
        /// テキストデータがDnDされた際に発生。
        /// </summary>
        /// <param name="reference">Reference</param>
        /// <param name="content">Reference0 テキスト内容。</param>
        /// <param name="charId">Reference1 DnDされたキャラクターのID。</param>
        /// <returns></returns>
        public virtual string OnTextDrop(IDictionary<int, string> reference, string content, string charId)
        {
            return "";
        }

        #endregion

        #region Notifyイベント

        /// <summary>
        /// 起動時に通知されるNotifyイベント
        /// </summary>
        public virtual void OnInitialize()
        {
        }

        /// <summary>
        /// 終了時に通知されるNotifyイベント
        /// </summary>
        public virtual void OnDestroy()
        {
        }

        #endregion

        #region イベント処理補助


        /* ランダムトーク */

        private RandomTalk GetRandomTalk()
        {
            var filtedTalks = RandomTalks.Where(t => t.Filter()).ToArray();

            return filtedTalks[rand.Next(0, filtedTalks.Length)];
        }

        /// <summary>
        /// ランダムトークが発生するタイミングで呼び出される。
        /// <para>ただしOnMinuteChangeをオーバーライドした場合は自分で呼び出すように実装してください。</para>
        /// </summary>
        /// <returns></returns>
        protected virtual string OnRandomTalk()
        {
            var randomTalk = NextRandomTalk;
            NextRandomTalk = () => GetRandomTalk().TalkScript();

            return randomTalk();
        }

        /* マウス関連 */

        /// <summary>
        /// RosalindがOnMouseMoveを何回カウントしたらOnMouseStrokeを呼び出すかを返すメソッド。
        /// オーバーライドでOnMouseStrokeの呼び出しタイミングを制御できます。
        /// <para>引数で当たり判定の識別子が渡されるので、当たり判定が小さい部位は値を小さめにして調整するといいかも。</para>
        /// <para>初期実装は100を固定で返すようになっています。</para>
        /// </summary>
        /// <param name="partsName">当たり判定の識別子</param>
        /// <param name="deviceType">タッチパネルならtouch、それ以外ならmouse。</param>
        /// <returns></returns>
        protected virtual int GetCallOnMouseStrokeTiming(string partsName, DeviceType deviceType)
        {
            return 100;
        }

        /// <summary>
        /// いわゆる当たり判定へのなでなで判定がされた際に呼び出されるメソッド。
        /// なでなで判定の設定方法は<see cref="GetCallOnMouseStrokeTiming"/>を参照。
        /// </summary>
        /// <param name="partsName">当たり判定の識別子</param>
        /// <param name="deviceType">タッチパネルならtouch、それ以外ならmouse。</param>
        /// <returns></returns>
        protected virtual string OnMouseStroke(string partsName, DeviceType deviceType)
        {
            return "";
        }

        #endregion

    }

}
