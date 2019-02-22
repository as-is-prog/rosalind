using Shiorose.Resource;
using Shiorose.Support;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shiorose
{

    internal class CompileErrorGhost : Ghost
    {
        private readonly string errorMessage;

        public CompileErrorGhost(string errorMessage)
        {
            Homeurl = "";

            _saveData = new MockSaveData();

            this.errorMessage = new TalkBuilder().Append("エラーが発生しました。").LineFeed().HalfLine().Append("\\_?" + errorMessage + "\\_?").Build();
            RandomTalks.Add(Talk.Create(this.errorMessage));
        }

        public override string OnBoot(IDictionary<int, string> reference, string shellName = "", bool isHalt = false, string haltGhostName = "")
        {
            return this.errorMessage;
        }

        public override string OnMouseDoubleClick(IDictionary<int, string> reference, string mouseX, string mouseY, string charId, string partsName, string buttonName, DeviceType deviceType)
        {
            return this.errorMessage;
        }
    }
}
