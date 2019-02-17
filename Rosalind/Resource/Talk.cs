using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose.Resource
{
    public class Talk
    {
        public Func<bool> Filter { get; set; }
        public Func<string> TalkScript { get; set; } 

        public Talk(Func<string> talkScript, Func<bool> filter = null)
        {
            TalkScript = talkScript;
            Filter = filter ?? (() => true);
        }

        public Talk(string talkScript, Func<bool> filter = null) : this(() => talkScript, filter)
        {

        }

    }
}
