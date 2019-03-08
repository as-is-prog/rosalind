using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

            var jsonStr = File.ReadAllText(Rosalind.ShioriDir + SaveFileName);
            return (T)JsonConvert.DeserializeObject(jsonStr);
        }

        /// <summary>
        /// セーブデータの保存を行う
        /// </summary>
        /// <typeparam name="T">セーブデータのクラス(BaseSaveDataを継承したクラス)</typeparam>
        /// <param name="saveData">セーブデータのインスタンス</param>
        public static void Save<T>(T saveData) where T : BaseSaveData, new()
        {
            using (var streamWriter = new StreamWriter(File.Create(Rosalind.ShioriDir + SaveFileName)))
            {
                var jsonStr = JsonConvert.SerializeObject(saveData);
                streamWriter.Write(jsonStr);
            }
        }
    }

    /// <summary>
    /// セーブデータのクラス。ゴースト開発者はこのクラスを継承して保存したい項目を増やすと良い。
    /// </summary>
    public class BaseSaveData
    {
        /// <summary>
        /// ランダムトークの間隔（秒）
        /// </summary>
        public int TalkInterval { get; set; }

        /// <summary>
        /// ユーザ名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// セーブデータをファイルに書き込む
        /// </summary>
        public virtual void Save()
        {
            SaveDataManager.Save(this);
        }
    }

    internal class MockSaveData : BaseSaveData
    {
        public MockSaveData()
        {
            TalkInterval = 0;
            UserName = "";
        }

        public override void Save()
        {

        }
    }


}
