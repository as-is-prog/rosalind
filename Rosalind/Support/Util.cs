using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose.Support
{
    /// <summary>
    /// 
    /// </summary>
    public static class Util
    {
        private static Random random = new Random();

        /// <summary>
        /// 引数で渡された文字列からランダムに一つ選択し返します。
        /// </summary>
        /// <param name="strs">文字列</param>
        /// <returns></returns>
        public static string RandomChoice(params string[] strs)
        {
            return strs[random.Next(strs.Length)];
        }
    }
}
