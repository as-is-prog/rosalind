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

        private readonly StringBuilder stringBuilder;
        private readonly List<object> objects;

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
        public TalkBuilder AppendMarker()
        {
            stringBuilder.Append("\\![*]");

            return this;
        }

        /// <summary>
        /// トークに値を埋め込みます。
        /// 埋め込んだ値はオートウェイトの対象となりません。
        /// </summary>
        /// <param name="value">埋め込みたい値</param>
        /// <returns></returns>
        public TalkBuilder EmbedValue(object value)
        {

            stringBuilder.Append("{"+objects.Count+"}");
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
            for(int i = 0; i < count; i++)
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
}
