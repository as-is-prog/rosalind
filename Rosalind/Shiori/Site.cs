using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose
{
    /// <summary>
    /// おすすめサイト・ポータルサイトを表すクラス
    /// </summary>
    public class Site
    {
        /// <summary>
        /// 表示名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 表示する画像のパス
        /// </summary>
        public string ImgPath { get; set; }
        /// <summary>
        /// 選択時に実行するスクリプト
        /// </summary>
        public string TalkScript { get; set; } = "";

        /// <summary>
        ///
        /// </summary>
        /// <param name="name">表示名</param>
        /// <param name="url">URL</param>
        /// <param name="imgPath">表示する画像のパス</param>
        public Site(string name, string url = "", string imgPath = "")
        {
            Name = name;
            Url = url;
            ImgPath = imgPath;
        }
    }

    /// <summary>
    /// Siteの補助クラス
    /// </summary>
    public static class SitesUtil
    {
        private static readonly string BYTE_1_STR = Shiori.DEFAULT_CHARSET.GetString(new byte[] { 1 });
        private static readonly string BYTE_2_STR = Shiori.DEFAULT_CHARSET.GetString(new byte[] { 2 });

        /// <summary>
        /// SiteのコレクションをSHIORIメソッドで定義された形式の文字列に変換する
        /// </summary>
        /// <param name="sites"></param>
        /// <returns></returns>
        public static string ToStringFromSites(this IEnumerable<Site> sites)
        {
            var siteStrs = sites.Select(site => {
                var siteUrlPart = site.TalkScript == "" ? site.Url : string.Format(@"script:\![open,browser,{0}]{1}", site.Url, site.TalkScript);
                return string.Format("{1}{0}{2}{0}{3}{0}", BYTE_1_STR, site.Name, siteUrlPart, site.ImgPath);
            });
            
            return String.Join(BYTE_2_STR, siteStrs);
        }
    }
}
