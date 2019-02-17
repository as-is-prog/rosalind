using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose.Resource
{
    /// <summary>
    /// トークに関わるクラス
    /// </summary>
    public class Talk
    {
        /// <summary>
        /// トークが有効かを判断するメソッド
        /// </summary>
        public Func<bool> Filter { get; private set; }
        /// <summary>
        /// トーク内容を返すメソッド
        /// </summary>
        public Func<string> TalkScript { get; private set; } 

        private Talk(Func<string> talkScript, Func<bool> filter)
        {
            TalkScript = talkScript;
            Filter = filter ?? (() => true);
        }

        private Talk(string talkScript, Func<bool> filter = null) : this(() => talkScript, filter)
        {

        }

        /// <summary>
        /// トークを生成します
        /// </summary>
        /// <param name="talkScript">トーク内容を返すメソッド</param>
        /// <param name="filter">このトークが選ばれる条件（省略可）</param>
        /// <returns></returns>
        public static Talk Create(Func<string> talkScript, Func<bool> filter = null)
        {
            return new Talk(talkScript, filter);
        }

        /// <summary>
        /// トークを生成します
        /// </summary>
        /// <param name="talkScript">トーク内容</param>
        /// <param name="filter">このトークが選ばれる条件（省略可）</param>
        /// <returns></returns>
        public static Talk Create(string talkScript, Func<bool> filter = null)
        {
            return new Talk(talkScript, filter);
        }

        /// <summary>
        /// 自動ウェイト付きのトークを生成します
        /// </summary>
        /// <param name="talkScript">トーク内容を返すメソッド</param>
        /// <param name="filter">このトークが選ばれる条件（省略可）</param>
        /// <returns></returns>
        public static Talk CreateWithAutoWait(Func<string> talkScript, Func<bool> filter = null)
        {
            return new Talk(AutoWait(talkScript), filter);
        }

        /// <summary>
        /// 自動ウェイト付きのトークを生成します
        /// </summary>
        /// <param name="talkScript">トーク内容</param>
        /// <param name="filter">このトークが選ばれる条件（省略可）</param>
        /// <returns></returns>
        public static Talk CreateWithAutoWait(string talkScript, Func<bool> filter = null)
        {
            return new Talk(AutoWait(talkScript), filter);
        }

        /// <summary>
        /// 渡されたスクリプトに自動Waitを挿入します。
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public static string AutoWait(string script)
        {
            return script.Replace("、", "、\\w5").Replace("。", "。\\w8").Replace("…", "…\\w5");
        }

        /// <summary>
        /// 渡されたスクリプトを自動Waitを挿入するメソッドでラッピングします。
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public static Func<string> AutoWait(Func<string> script)
        {
            return () => AutoWait(script());
        }

    }
}
