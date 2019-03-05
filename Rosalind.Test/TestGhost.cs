using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose.Test
{
    class TestGhost : Shiorose.Ghost
    {
        public TestGhost()
        {
            _saveData = new Shiorose.Resource.MockSaveData();
        }

        public override string OnBoot(IDictionary<int, string> reference, string shellName = "", bool isHalt = false, string haltGhostName = "")
        {
            if (isHalt)
                return string.Format("{0}で落ちましたが無事起動しました", haltGhostName);
            else
                return "起動しました";
        }

        public override string OnClose(IDictionary<int, string> reference, string reason = "")
        {
            return "終了します";
        }
    }
}
