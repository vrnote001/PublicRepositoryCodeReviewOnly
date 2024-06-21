using System.IO;
using UnityEngine;

//https://kiironomidori.hatenablog.com/entry/unity_save_json //Save機能のジェネリッククラス
//https://qiita.com/4_mio_11/items/145c658078a7fe5f36a7 //usingsステートメント

namespace _ProjectTool
{
    public static class JsonSaveUtility<T> 
    {
        private static string SavePath(string path)
            => $"{Application.persistentDataPath}/{path}.json";

        /// <summary>
        /// データのセーブを行います
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        public static void Save(T data,string path)
        {
            using StreamWriter sw = new StreamWriter(SavePath(path), false);
            var jsonStr = JsonUtility.ToJson(data, true);
            sw.Write(jsonStr);
            sw.Flush();
        }

        /// <summary>
        /// データのロードを行います
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T Load(string path)
        {
            //データが存在すれば返す
            if (!File.Exists(SavePath(path))) return default;
            using StreamReader sr = new StreamReader(SavePath(path));
            var dataStr = sr.ReadToEnd();
            return JsonUtility.FromJson<T>(dataStr);
            //存在しなければ返却
        }

    }
}
