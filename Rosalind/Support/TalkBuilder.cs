using Shiorose.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose.Support
{
    /// <summary>
    /// トーク文字列作成を補助するクラスです。
    /// </summary>
    public class TalkBuilder
    {
        /// <summary>
        /// 自動で初めに挿入されるスコープ番号です。
        /// </summary>
        public static int defaultScope = 0;
        /// <summary>
        /// 自動で初めに挿入されるサーフェス番号です。
        /// </summary>
        public static int defaultSurface = 0;

        internal StringBuilder stringBuilder;
        internal List<object> objects;

        /// <summary>
        /// トークビルダーを作成します。
        /// </summary>
        public TalkBuilder()
        {
            stringBuilder = new StringBuilder("\\").Append(defaultScope).Append("\\s[").Append(defaultSurface).Append("]");
            objects = new List<object>();
        }

        /// <summary>
        /// トークに文字列を追加します。
        /// </summary>
        /// <param name="str">追加したい文字列</param>
        /// <returns></returns>
        public TalkBuilder Append(string str)
        {
            stringBuilder.Append(str);

            return this;
        }

        /// <summary>
        /// トークにマーカー(\![*])を追加します。
        /// </summary>
        /// <returns></returns>
        public TalkBuilder Marker()
        {
            stringBuilder.Append("\\![*]");

            return this;
        }

        /// <summary>
        /// 選択肢を追加します。
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public DeferredEventTalkBuilder AppendChoice(string title, string id = null)
        {
            EmbedValue(Choice.CreateScript(title, id));
            return new DeferredEventTalkBuilder(this);
        }

        /// <summary>
        /// TeachBoxを表示するさくらスクリプトを追加します。
        /// </summary>
        /// <returns></returns>
        public DeferredEventTalkBuilder AppendTeach()
        {
            Append(@"\![open,teachbox]");
            return new DeferredEventTalkBuilder(this);
        }

        /// <summary>
        /// CommunicateBoxを表示するさくらスクリプトを追加します。
        /// </summary>
        /// <returns></returns>
        public TalkBuilder AppendCommunicate()
        {
            Append(@"\![open,communicatebox]");
            return this;
        }

        /// <summary>
        /// ユーザ入力を受け付けるさくらスクリプトを追加します。
        /// </summary>
        /// <param name="timeout">タイムアウト時間（ミリ秒）</param>
        /// <param name="defValue">InputBoxの初期値</param>
        /// <returns></returns>
        public DeferredEventTalkBuilder AppendUserInput(int timeout = 0, string defValue = "")
        {
            EmbedValue(string.Format(@"\![open,inputbox,ユーザ入力,{0},{1}]", timeout, defValue));
            return new DeferredEventTalkBuilder(this);
        }

        /// <summary>
        /// パスワード入力を受け付けるさくらスクリプトを追加します。
        /// </summary>
        /// <param name="timeout">タイムアウト時間（ミリ秒）</param>
        /// <param name="defValue">InputBoxの初期値</param>
        /// <returns></returns>
        public DeferredEventTalkBuilder AppendPassInput(int timeout = 0, string defValue = "")
        {
            EmbedValue(string.Format(@"\![open,passwordinput,パスワード入力,{0},{1}]", timeout, defValue));
            return new DeferredEventTalkBuilder(this);
        }

        /// <summary>
        /// 年月日の入力を受け付けるさくらスクリプトを追加します。
        /// </summary>
        /// <param name="year">年の初期値</param>
        /// <param name="month">月の初期値</param>
        /// <param name="day">日の初期値</param>
        /// <param name="timeout">タイムアウト時間（ミリ秒）</param>
        /// <returns></returns>
        public DeferredEventTalkBuilder AppendDateInput(int year, int month, int day, int timeout = 0)
        {
            EmbedValue(string.Format(@"\![open,dateinput,年月日入力,{0},{1},{2},{3}]", timeout, year, month, day));
            return new DeferredEventTalkBuilder(this);
        }

        /// <summary>
        /// スライダーによる数値入力を受け付けるさくらスクリプトを追加します。
        /// </summary>
        /// <param name="defValue">現在値</param>
        /// <param name="min">最小</param>
        /// <param name="max">最大</param>
        /// <param name="timeout">タイムアウト時間（ミリ秒）</param>
        /// <returns></returns>
        public DeferredEventTalkBuilder AppendSliderInput(int defValue, int min, int max, int timeout = 0)
        {
            EmbedValue(string.Format(@"\![open,sliderinput,スライダー入力,{0},{1},{2},{3}]", timeout, defValue, min, max));
            return new DeferredEventTalkBuilder(this);
        }

        /// <summary>
        /// 時分秒の入力を受け付けるさくらスクリプトを追加します。
        /// </summary>
        /// <param name="hour">時の初期値</param>
        /// <param name="minute">分の初期値</param>
        /// <param name="second">秒の初期値</param>
        /// <param name="timeout">タイムアウト時間（ミリ秒）</param>
        /// <returns></returns>
        public DeferredEventTalkBuilder AppendTimeInput(int hour, int minute, int second, int timeout = 0)
        {
            EmbedValue(string.Format(@"\![open,dateinput,時分秒入力,{0},{1},{2},{3}]", timeout, hour, minute, second));
            return new DeferredEventTalkBuilder(this);
        }

        /// <summary>
        /// トークに値を埋め込みます。
        /// 埋め込んだ値はオートウェイトの対象となりません。
        /// </summary>
        /// <param name="value">埋め込みたい値</param>
        /// <returns></returns>
        public TalkBuilder EmbedValue(object value)
        {

            stringBuilder.Append("{" + objects.Count + "}");
            objects.Add(value);

            return this;
        }

        /// <summary>
        /// トークに文字列を追加し、かつ改行も追加します。
        /// </summary>
        /// <param name="str">追加したい文字列</param>
        /// <returns></returns>
        public TalkBuilder AppendLine(string str = "")
        {
            stringBuilder.Append(str);
            stringBuilder.Append("\\n");

            return this;
        }

        /// <summary>
        /// トークに改行を追加します。
        /// </summary>
        /// <param name="count">追加する改行文字の数</param>
        /// <returns></returns>
        public TalkBuilder LineFeed(int count = 1)
        {
            for (int i = 0; i < count; i++)
                stringBuilder.Append("\\n");

            return this;
        }

        /// <summary>
        /// トークに半改行(\n[half])を追加します。
        /// </summary>
        /// <returns></returns>
        public TalkBuilder HalfLine()
        {
            stringBuilder.Append("\\n[half]");
            return this;
        }

        /// <summary>
        /// トークにスコープ変更文字列を追加します。
        /// </summary>
        /// <param name="scopeNumber">変更後のスコープ番号</param>
        /// <returns></returns>
        public TalkBuilder ChangeScope(int scopeNumber)
        {
            stringBuilder.Append("\\").Append(scopeNumber);

            return this;
        }

        /// <summary>
        /// トークにサーフェス変更文字列を追加します。
        /// </summary>
        /// <param name="surfaceNumber">変更後のサーフェス番号</param>
        /// <returns></returns>
        public TalkBuilder ChangeSurface(int surfaceNumber)
        {
            stringBuilder.Append("\\").Append(surfaceNumber);

            return this;
        }

        /// <summary>
        /// トーク文字列を生成します。
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            var buildedStr = stringBuilder.Append("\\e").ToString();

            return Embed(buildedStr);
        }

        /// <summary>
        /// トーク文字列をオートウェイトを挿入した上で生成します。
        /// </summary>
        /// <returns></returns>
        public string BuildWithAutoWait()
        {
            return Embed(Talk.AutoWait(stringBuilder.Append("\\e").ToString()));
        }

        private string Embed(string str)
        {
            var embededStr = str;

            for (int i = 0; i < objects.Count; i++)
                embededStr = embededStr.Replace("{" + i + "}", objects[i].ToString());

            return embededStr;
        }
    }

    /// <summary>
    /// 各種ユーザの文字列入力もしくは選択肢選択で発生する、遅延イベントを含むトーク作成補助クラス
    /// </summary>
    public class DeferredEventTalkBuilder
    {
        private TalkBuilder tb;

        internal DeferredEventTalkBuilder(TalkBuilder tb)
        {
            this.tb = tb;
        }

        /// <summary>
        /// トークにマーカー(\![*])を追加します。
        /// </summary>
        /// <returns></returns>
        public DeferredEventTalkBuilder Marker()
        {
            tb.Marker();
            return this;
        }
        /// <summary>
        /// 選択肢を追加します。
        /// </summary>
        /// <param name="title">選択肢の表示名</param>
        /// <param name="id">選択肢の識別子</param>
        /// <returns></returns>
        public DeferredEventTalkBuilder AppendChoice(string title, string id = null)
        {
            tb.EmbedValue(Choice.CreateScript(title, id));
            return this;
        }

        /// <summary>
        /// トークに改行を追加します。
        /// </summary>
        /// <param name="count">追加する改行文字の数</param>
        /// <returns></returns>
        public DeferredEventTalkBuilder LineFeed(int count = 1)
        {
            tb.LineFeed(count);
            return this;
        }

        /// <summary>
        /// トークに半改行(\n[half])を追加します。
        /// </summary>
        /// <returns></returns>
        public DeferredEventTalkBuilder HalfLine()
        {
            tb.HalfLine();
            return this;
        }

        /// <summary>
        /// 遅延イベント付きトークオブジェクトを生成します。
        /// <para>
        /// 通常はこの後ContinuteWithメソッドで遅延イベント発生時の処理を追加します。
        /// </para>
        /// </summary>
        /// <returns></returns>
        public DeferredEventTalk Build()
        {
            return new DeferredEventTalk(tb.Build());
        }

        /// <summary>
        /// 遅延イベント付きトークオブジェクトをオートウェイト挿入有りで生成します。
        /// <para>
        /// 通常はこの後ContinuteWithメソッドで遅延イベント発生時の処理を追加します。
        /// </para>
        /// </summary>
        /// <returns></returns>
        public DeferredEventTalk BuildWithAutoWait()
        {
            return new DeferredEventTalk(tb.BuildWithAutoWait());
        }
    }

    /// <summary>
    /// 各種ユーザの文字列入力もしくは選択肢選択で発生する、遅延イベントを表すクラスです。
    /// </summary>
    public class DeferredEventTalk {
        private string sakuraScript;

        internal DeferredEventTalk(string sakuraScript)
        {
            this.sakuraScript = sakuraScript;
        }

        /// <summary>
        /// このオブジェクトに関連付けられた遅延イベント時の処理を指定し、
        /// かつこのオブジェクトが持つさくらスクリプトを返します。
        /// </summary>
        /// <param name="talkScript">遅延イベント発生時に実行するさくらスクリプトを返すメソッド</param>
        /// <param name="cancelScript">遅延イベントがキャンセルされた時に実行するさくらスクリプトを返すメソッド</param>
        /// <returns></returns>
        public string ContinueWith(Func<string, string> talkScript, Func<string> cancelScript = null)
        {
            DeferredEvent.Set(talkScript);
            if (cancelScript != null) DeferredEvent.SetCancelScript(cancelScript);

            return sakuraScript;
        }

        /// <summary>
        /// このオブジェクトが持つさくらスクリプトを返します。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return sakuraScript;
        }
    }
}
