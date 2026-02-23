using System.IO;
using Newtonsoft.Json;

namespace Shiorose.Decafe
{
    public class DecafeConfig
    {
        public bool Enabled { get; set; } = false;
        public string ServerUrl { get; set; } = "http://localhost:3000";
        public string InhabitantId { get; set; } = "";

        public static DecafeConfig Load(string shioriDir)
        {
            var path = Path.Combine(shioriDir, "decafe.json");
            if (!File.Exists(path))
                return new DecafeConfig();

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<DecafeConfig>(json) ?? new DecafeConfig();
        }
    }
}
