using System;
using System.Collections.Generic;
using System.Text;

namespace Shiorose.Resource
{
    /// <summary>
    /// Security Levelに対応
    /// </summary>
    public enum RequestFilterLevel
    {
        /// <summary>
        /// [非推奨] すべてのリクエストを受け付けます。
        /// </summary>
        None                  = 0x00,
        /// <summary>
        /// Security Level: localのみ受け付けます。
        /// </summary>
        LocalOnly             = 0x01,
        /// <summary>
        /// 指定されたもの以外のすべてのリクエストを受け付けます。
        /// </summary>
        NoneAndBlackList      = 0x10,
        /// <summary>
        /// Security Level: localなら無条件に、externalであっても指定されているリクエストであれば受け付けます。
        /// </summary>
        LocalOnlyAndWhiteList = 0x21,
    }
}
