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
