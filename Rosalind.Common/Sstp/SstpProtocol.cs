using System;
using System.Collections.Generic;
using System.Text;

namespace Shiorose.Sstp
{
    abstract class SstpProtocol
    {
        /// <summary>
        /// SSTP用のフォーマットの文字列に変換する
        /// </summary>
        /// <returns></returns>
        public abstract string ToSstpString();
    }
}
