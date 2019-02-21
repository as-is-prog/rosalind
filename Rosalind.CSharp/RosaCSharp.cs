using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Shiorose.Shiolink;

namespace Shiorose.CSharp
{
    internal class RosaCSharp : Rosalind
    {
        internal RosaCSharp(Load load) : base(load)
        {

        }

        internal async static Task<Rosalind> Load(Load load)
        {
            RosaCSharp rosa = new RosaCSharp(load);

            var files = Directory.GetFiles(ShioriDir, "*.csx", SearchOption.AllDirectories).Where(f => !(f.EndsWith("Ghost.csx") || f.EndsWith("SaveData.csx")));

            var imp = string.Join("\r\n", files.Select(f => string.Format("#load \"{0}\"", f))) + "\r\n";

            // Script Options
            var ssr = ScriptSourceResolver.Default.WithBaseDirectory(ShioriDir);
            var smr = ScriptMetadataResolver.Default.WithBaseDirectory(ShioriDir);
            var so = ScriptOptions.Default.WithSourceResolver(ssr).WithMetadataResolver(smr);

            var ghostScript = CSharpScript.Create<Ghost>(imp + File.ReadAllText(ShioriDir + "Ghost.csx"), so);

            rosa.ghost = (await ghostScript.RunAsync()).ReturnValue;
            return rosa;

        }
    }
}
