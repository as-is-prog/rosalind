namespace Shiorose.Shiolink
{

    internal class Load : Protocol
    {
        public string ShioriDir { get; private set; }

        public Load(string shioriDir)
        {
            ShioriDir = shioriDir;
        }

        public static new Load Parse(System.IO.TextReader stdIn)
        {
            return Protocol.Parse(stdIn) as Load;
        }
    }
}
