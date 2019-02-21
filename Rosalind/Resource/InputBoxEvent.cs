using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose.Resource
{
    public class InputBoxEvent : DeferredEvent
    {

        private InputBoxEvent(string id, Func<string> talkScript) : base(id, talkScript)
        {
        }

        public static string Create(string id, Func<string> talkScript)
        {
            var inputBoxEvent = new InputBoxEvent(id, talkScript);

            return inputBoxEvent.ToString();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
