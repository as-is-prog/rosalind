using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose
{
    public class Site
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string ImgPath { get; set; }
        public string TalkScript { get; set; }

        public Site(string name, string url = "", string imgPath = "")
        {
            Name = name;
            Url = url;
            ImgPath = imgPath;
        }
    }

    public static class SitesUtil
    {
        private static readonly string BYTE_1_STR = Shiori.DEFAULT_CHARSET.GetString(new byte[] { 1 });
        private static readonly string BYTE_2_STR = Shiori.DEFAULT_CHARSET.GetString(new byte[] { 2 });

        public static string ToStringFromSites(this IEnumerable<Site> sites)
        {
            var siteStrs = sites.Select(site => string.Format("{1}{0}{2}{0}{3}{0}{4}", BYTE_1_STR, site.Name, site.Url, site.ImgPath, site.TalkScript));

            return String.Join(BYTE_2_STR, siteStrs);
        }
    }
}
