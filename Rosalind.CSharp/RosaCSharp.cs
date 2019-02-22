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
    public class RosaCSharp : Rosalind
    {
        internal RosaCSharp(Load load) : base(load)
        {

        }

        internal async static Task<Rosalind> Load(Load load)
        {
            RosaCSharp rosa = new RosaCSharp(load);
            try
            {
                var files = Directory.GetFiles(ShioriDir, "*.csx", SearchOption.AllDirectories).Where(f => !(f.EndsWith("Ghost.csx") || f.EndsWith("SaveData.csx")));

                var imp = string.Join("\r\n", files.Select(f => string.Format("#load \"{0}\"", f))) + "\r\n";

                // Script Options
                var ssr = ScriptSourceResolver.Default.WithBaseDirectory(ShioriDir);
                var smr = ScriptMetadataResolver.Default.WithBaseDirectory(ShioriDir);
                var so = ScriptOptions.Default.WithSourceResolver(ssr).WithMetadataResolver(smr);

                var ghostScript = CSharpScript.Create<Ghost>(imp + File.ReadAllText(ShioriDir + "Ghost.csx"), so);

                rosa.ghost = (await ghostScript.RunAsync()).ReturnValue;
            }
            catch (Exception e)
            {
                rosa.ghost = new CompileErrorGhost(e.Message);
            }
            return rosa;

        }

        public class RunScriptResult
        {
            /// <summary>
            /// 成功したか
            /// </summary>
            public bool isSuccess = false;
            /// <summary>
            /// 成功時は戻り値、
            /// 失敗時はエラーメッセージ
            /// </summary>
            public string value;

            public RunScriptResult(bool isSuccess, string value)
            {
                this.isSuccess = isSuccess;
                this.value = value;
            }
        }

        public async static Task<RunScriptResult> RunCSharpScript(string str, string currentDir = null)
        {
            return await Task.Run(async () => {
                try
                {
                    Script<string> script;
                    if (currentDir != null) {
                        var ssr = ScriptSourceResolver.Default.WithBaseDirectory(currentDir);
                        var smr = ScriptMetadataResolver.Default.WithBaseDirectory(currentDir);
                        var so = ScriptOptions.Default.WithSourceResolver(ssr).WithMetadataResolver(smr);
                        script = CSharpScript.Create<string>(str, so);
                    }
                    else
                    {
                        script = CSharpScript.Create<string>(str);
                    }

                    return new RunScriptResult(true, (await script.RunAsync()).ReturnValue);
                }
                catch (Exception e)
                {
                    return new RunScriptResult(false, e.Message);
                }
            });
        }
    }
}
