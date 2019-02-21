using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose.Resource
{
    /// <summary>
    /// 遅延して実行されるスクリプトを持つイベントの基底クラス。
    /// </summary>
    public abstract class DeferredEvent
    {
        /// <summary>
        /// イベントの識別子
        /// </summary>
        protected string Id { get; set; }
        /// <summary>
        /// イベント発生時に実行されるスクリプト
        /// </summary>
        protected Func<string> TalkScript { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="talkScript"></param>
        protected DeferredEvent(string id, Func<string> talkScript)
        {
            Id = id;
            TalkScript = talkScript;
        }

    }
}
