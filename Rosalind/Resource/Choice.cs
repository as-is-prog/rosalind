using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose.Resource
{
    /// <summary>
    /// 選択肢に関するクラス
    /// </summary>
    public class Choice : DeferredEvent
    {
        /// <summary>
        /// 現在登録されてる選択肢の数
        /// </summary>
        public static int Count { get => Choices.Count; }

        private static List<Choice> Choices { get; set; } = new List<Choice>();

        /// <summary>
        /// 選択がタイムアップしたときに返すスクリプト
        /// </summary>
        public static Func<string> TimeoutScript { get; set; }

        /// <summary>
        /// 選択肢の表示名
        /// </summary>
        private string Title { get; set; }

        private Choice(string title, Func<string> talkScript, string id) : base(id, talkScript)
        {
            Title = title;
        }

        /// <summary>
        /// 選択肢を作成します
        /// </summary>
        /// <param name="title">選択肢のラベル</param>
        /// <param name="talkScript">この選択肢を選んだ時に返すスクリプト</param>
        /// <param name="id">選択肢のID(省略可。省略時はラベルをIDとして使う)</param>
        /// <returns>選択肢のSakuraScript</returns>
        public static string Create(string title, Func<string> talkScript, string id = null)
        {
            var choice = new Choice(title, talkScript, id ?? title);
            Choices.Add(choice);

            return choice.ToString();
        }

        /// <summary>
        /// 指定のIDを持つ選択肢のスクリプトを取得します。
        /// </summary>
        /// <remarks>
        /// このメソッドを実行した時点で選択肢のリストはクリアされます。
        /// </remarks>
        /// <param name="id"></param>
        /// <returns>選択肢のスクリプト</returns>
        public static Func<string> Select(string id)
        {
            var choiceScript = Choices.Where(c => c.Id == id)
                                 .Select(c => c.TalkScript)
                                 .FirstOrDefault() ?? (id == "timeout" && TimeoutScript != null ? TimeoutScript : () => "");
            Choices.Clear();
            return choiceScript;
        }

        /// <summary>
        /// この選択肢インスタンスをSakuraScriptで挿入する際の文字列表現に変換します。
        /// </summary>
        /// <returns>この選択肢のSakuraScript文字列</returns>
        public override string ToString()
        {
            return string.Format(@"\q[{0},{1}]", Title, Id);
        }
    }
}
