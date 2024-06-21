using System;
using System.IO;
using UnityEngine;

namespace _ProjectTool
{
    public static class GitIgnoreWriter
    {
        public static void AddGitIgnore(string ignoreText,string gitNotePath)
        {
            try
            {
                // .gitignoreファイルを読み込みます
                string[] lines = File.ReadAllLines(gitNotePath);

                // 追記するテキストが既に存在するかチェックします
                bool alreadyExists = false;
                foreach (string line in lines)
                {
                    if (line == ignoreText)
                    {
                        alreadyExists = true;
                        break;
                    }
                }

                // 追記するテキストが存在しない場合、ファイルの末尾に追記します
                if (!alreadyExists)
                {
                    using StreamWriter writer = File.AppendText(gitNotePath);
                    var meta = $"{ignoreText.Remove(ignoreText.Length-1)}.meta";

                    writer.WriteLine(meta);
                    writer.WriteLine(ignoreText); 
                    writer.Flush();

                    var t =new  FileStream(gitNotePath, FileMode.Open,FileAccess.Read);
                    var r = new StreamReader(t);
                
                    Debug.Log(r.ReadToEnd());
                    Debug.Log("Text added successfully.");
                }
                else
                {
                    Debug.Log("Text already exists.");
                }
            }
            catch (Exception ex)
            {
                Debug.Log("An error occurred: " + ex.Message);
            }

            Console.ReadLine();
        }
    
    }
}



# region GitNote御作法
// # コメントは "#" で始めます
//
// # 特定のファイルを無視する場合は、ファイル名を指定します
//     file.txt
//
// # ワイルドカードを使って、複数のファイルを無視することもできます
//         *.log
//
// # 特定のディレクトリを無視する場合は、ディレクトリ名を指定します
//         directory/
//
// # ディレクトリの中身を無視する場合は、ディレクトリ名にスラッシュを追加します
//     directory/*
//
// # 複数のディレクトリを無視する場合は、改行して指定します
// directory1/
// directory2/
//
// # 例外を指定する場合は、先頭に "!" を付けます
// !important-file.txt
//
// 
#endregion
