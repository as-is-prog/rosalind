using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose
{
    public class Ghost
    {
        protected string username = "test";
        public string Homeurl { get; private set; }

        public Ghost()
        {
            Homeurl = "";
        }

        public virtual string OnBoot()
        {
            return @"\u\s[-1]\h\s[0](Base)こんちは";
        }

        public virtual string OnClose()
        {
            return @"\u\s[-1]\h\s[0](Base)さいなら";
        }
    }
}
