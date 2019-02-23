using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose.Resource
{
    /// <summary>
    /// SHIORI Resourceの値を管理するクラス。
    /// </summary>
    public class SHIORIResource
    {
        /// <summary>
        /// 本体側のおすすめサイトのキャプション
        /// </summary>
        public virtual Func<string> SakuraRecommendButtonCaption { get; set; } = () => "";

        /// <summary>
        /// 本体側のポータルサイトのキャプション
        /// </summary>
        public virtual Func<string> SakuraPortalButtonCaption { get; set; } = () => "";

        /// <summary>
        /// 相方側のおすすめサイトのキャプション
        /// </summary>
        public virtual Func<string> KeroRecommendButtonCaption { get; set; } = () => "";

        /// <summary>
        /// 相方側のポータルサイトのキャプション
        /// </summary>
        public virtual Func<string> KeroPortalButtonCaption { get; set; } = () => "";

        /// <summary>
        /// ネットワーク更新の名称
        /// </summary>
        public virtual Func<string> UpdateButtonCaption { get; set; } = () => "";

        /// <summary>
        /// 消滅通告の名称
        /// </summary>
        public virtual Func<string> VanishButtonCaption { get; set; } = () => "";

        /// <summary>
        /// Read Me!の名称
        /// </summary>
        public virtual Func<string> ReadmeButtonCaption { get; set; } = () => "";

        /// <summary>
        /// 消滅通告の表示。1で表示、0で非表示。
        /// </summary>
        public virtual Func<int> VanishButtonVisible { get; set; } = () => 1;

        /// <summary>
        /// 本体側のオーナードローメニュー（右クリックメニュー）の表示。1で表示、0で非表示。
        /// </summary>
        public virtual Func<int> SakuraPopupMenuVisible { get; set; } = () => 1;

        /// <summary>
        /// 相方側のオーナードローメニューの表示。1で表示、0で非表示。
        /// </summary>
        public virtual Func<int> KeroPopupMenuVisible { get; set; } = () => 1;

        /// <summary>
        /// AIグラフのSHIORI/3.0版。
        /// <para>(Rosalind未実装)</para>
        /// </summary>
        public virtual Func<string> GetAIState { get; set; } = () => "";

        /// <summary>
        /// GetAIStateの複数指定可能版。
        /// 第一引数に何個目のグラフ用のデータかが0スタートで渡される。
        /// ので、それに対応したデータを返してください。
        /// 表示しないグラフ番号には空文字列を返してください。
        /// <para>(Rosalind未実装)</para>
        /// </summary>
        public virtual Func<int, string> GetAIStateEx { get; set; } = (graphNum) => "";

    }
}
