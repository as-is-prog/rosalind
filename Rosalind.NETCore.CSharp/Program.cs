using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shiorose;

namespace Shiorose.CSharp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Shiori.Main(args, (firstLoad) => RosaCSharp.Load(firstLoad));
        }
    }
}
