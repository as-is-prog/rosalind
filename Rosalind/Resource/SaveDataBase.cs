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
    /// <summary>
    /// セーブデータを管理するクラス
    /// </summary>
    public static class SaveDataManager
    {
        /// <summary>
        /// セーブデータのファイル名
        /// </summary>
        public static string SaveFileName { get; set; } = "savedata.json";

        /// <summary>
        /// セーブデータの読み込みを行う
        /// </summary>
        /// <typeparam name="T">セーブデータのクラス(BaseSaveDataを継承したクラス)</typeparam>
        /// <returns>セーブデータのインスタンス</returns>
        public static T Load<T>() where T : BaseSaveData, new()
        {
            if (!File.Exists(Rosalind.ShioriDir + SaveFileName)) return new T();

            using (var stream = File.OpenRead(Rosalind.ShioriDir + SaveFileName))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// セーブデータの保存を行う
        /// </summary>
        /// <typeparam name="T">セーブデータのクラス(BaseSaveDataを継承したクラス)</typeparam>
        /// <param name="saveData">セーブデータのインスタンス</param>
        public static void Save<T>(T saveData) where T : BaseSaveData, new()
        {
            using (var stream = File.Create(Rosalind.ShioriDir + SaveFileName))
            {
                var serializer = new DataContractJsonSerializer(saveData.GetType());
                serializer.WriteObject(stream, saveData);
            }
        }
    }

    /// <summary>
    /// セーブデータのクラス。ゴースト開発者はこのクラスを継承して保存したい項目を増やすと良い。
    /// </summary>
    [DataContract]
    public class BaseSaveData
    {
        /// <summary>
        /// ランダムトークの間隔（秒）
        /// </summary>
        [DataMember]
        public int TalkInterval { get; set; }

        /// <summary>
        /// ユーザ名
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// セーブデータをファイルに書き込む
        /// </summary>
        public void Save()
        {
            SaveDataManager.Save(this);
        }
    }

}
