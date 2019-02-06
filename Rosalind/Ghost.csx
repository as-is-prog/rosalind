using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shiorose;

class MyGhost : Ghost
{
    private int n = 50;

    public MyGhost()
    {

    }

    public override string OnBoot()
    {
        return @"\u\s[-1]\h\s[0]こんちは";
    }

}

return new MyGhost();