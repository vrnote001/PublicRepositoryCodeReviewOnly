using UnityEditor;
using UnityEngine;

namespace _ProjectTool
{
    public class OpenPersistentFolder
    {
        [MenuItem("ProjectTool/Open Persistent Folder")]
        public static void OpenFolder()
        {
            string folderPath = Application.persistentDataPath;
#if UNITY_IOS && UNITY_EDITOR
            EditorUtility.RevealInFinder(folderPath); // macOSの場合
#else 
        EditorUtility.RevealInFinder(folderPath.Replace("/", "\\")); // Windowsの場合
#endif
        }
    }
}