using Shiorose.Shiolink;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shiorose.Resource
{
    /// <summary>
    /// SecurityLevelと組み合わせて使うFilter条件クラス。
    /// BlackListの場合、等しいものを受け付けない。
    /// WhiteListの場合、等しいものを受け付ける。
    /// </summary>
    public class RequestFilter
    {
        /// <summary>
        /// Shiori Request内のID
        /// </summary>
        public string ID { get; set; }
    }
}
