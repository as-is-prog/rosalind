using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiorose.Resource
{
    public static class SaveDataManager
    {
        public static string SaveFileName { get; set; } = "savedata.json";

        public static T Load<T>() where T : BaseSaveData, new()
        {
            if (!File.Exists(Rosalind.ShioriDir + SaveFileName)) return new T();

            using (var stream = File.OpenRead(Rosalind.ShioriDir + SaveFileName))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }

        public static void Save<T>(T saveData) where T : BaseSaveData, new()
        {
            using (var stream = File.Create(Rosalind.ShioriDir + SaveFileName))
            {
                var serializer = new DataContractJsonSerializer(saveData.GetType());
                serializer.WriteObject(stream, saveData);
            }
        }
    }

    [DataContract]
    public class BaseSaveData
    {
        /// <summary>
        /// ランダムトークの間隔（秒）
        /// </summary>
        [DataMember]
        public int TalkInterval { get; set; }

        public void Save()
        {
            SaveDataManager.Save(this);
        }
    }

}
