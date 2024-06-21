using UnityEditor;
using UnityEngine;

namespace _ProjectTool
{
    public class GitIgnoreEditorWindow : EditorWindow
    {
        private string _path;
        private string _gitNotePath;
    
        [MenuItem("ProjectTool/GitIgnoreEditorWindow ")]
        public static void ShowWindow()
        {
            GetWindow<GitIgnoreEditorWindow >();
        }

        private void OnGUI()
        {
            GUILayout.Label("Drag and Drop a file from Project tab here:");

            Event evt = Event.current;
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "Drop a file here!　Only directories can be dragged");

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                    {
                        break;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    DragAndDrop.AcceptDrag();

                    if (evt.type == EventType.DragPerform)
                    {
                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject.GetType() == typeof(DefaultAsset))
                            {
                                var get= AssetDatabase.GetAssetPath(draggedObject);
                                _path = $"{get}/";
                            }
                        }
                    }
                    break;
            }
        
            GUILayout.Label(_path);
            EditorGUILayout.Space(25);
        
            GUILayout.Label("隠しファイルの場合[cmd+shift+.]で表示されます");
            GUILayout.BeginHorizontal();
            _gitNotePath = EditorGUILayout.TextField("GitNotePath", _gitNotePath);
            if (GUILayout.Button("Read",GUILayout.Width(120)))
            {
                string get = EditorUtility.OpenFilePanel("Select Data", "", "");
                if (!string.IsNullOrEmpty(get))
                {
                    _gitNotePath = get;
                }
            }
            GUILayout.EndHorizontal();
        
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("AddGitIgnore!!",GUILayout.Width(300)))
            {
                GitIgnoreWriter.AddGitIgnore(_path,_gitNotePath);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
