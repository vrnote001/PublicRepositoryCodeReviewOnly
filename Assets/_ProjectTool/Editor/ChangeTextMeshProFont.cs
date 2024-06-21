using TMPro;
using UnityEditor;
using UnityEngine;

namespace _ProjectTool
{
    /// <summary>
    /// TextMeshProフォントの一括変更を行うエディタ拡張ウィンドウクラス
    /// </summary>
    public class ChangeTextMeshProFont : EditorWindow
    {
        // 現在のタグの一覧
        private string[] _tagOptions;
        // 選択されたタグのインデックス
        private int _selectedTagIndex = 0;
        // 選択されたタグ
        private string _selectedTag;
        // 変更先のフォント
        private TMP_FontAsset _font;
        // 変更対象のPrefabが格納されているフォルダのパス
        private string _folderPath = "Assets";

        [MenuItem("ProjectTool/Change TextMeshPro Font")]
        public static void ShowWindow()
        {
            GetWindow<ChangeTextMeshProFont>();
        }

        private void OnGUI()
        {
            // Unity内部のタグ一覧を取得
            _tagOptions = UnityEditorInternal.InternalEditorUtility.tags;
            // Tagの選択用のPopupを表示する
            _selectedTagIndex = EditorGUILayout.Popup("Tag", _selectedTagIndex, _tagOptions);
            // 選択されたタグを更新
            _selectedTag = _tagOptions[_selectedTagIndex];
            // 変更するFontを指定するObjectFieldを表示する
            _font = (TMP_FontAsset)EditorGUILayout.ObjectField("Font Asset", _font, typeof(TMP_FontAsset), false);
            // 対象のフォルダパスを指定するTextFieldを表示する
            _folderPath = EditorGUILayout.TextField("Folder Path", _folderPath);

            // 変更ボタンが押されたとき
            if (GUILayout.Button("Replace"))
            {
                ReplaceFont();
            }
        }
    
        private void ReplaceFont()
        {
            // 指定されたフォルダ内のPrefabのGUIDを取得
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { _folderPath });
        
            // 取得したPrefabを検索
            foreach (string guid in prefabGuids)
            {
                // Prefabのパスを取得
                string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                // Prefab内のTextMeshProコンポーネントを検索
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                TMP_Text[] textMeshPros = prefab.GetComponentsInChildren<TMP_Text>(true);
   
                foreach (var textMeshPro in textMeshPros)
                {
                    if (textMeshPro.gameObject.CompareTag(_selectedTag))
                    {
                        textMeshPro.font = _font;
                    }
                }
            
                //変更されたprefabに対して"dirty"状態にすることで、Unityエディタ上での変更を示す
                //https://indie-du.com/entry/2017/06/27/200000
                EditorUtility.SetDirty(prefab);
                //"dirty"状態が付与された全てのアセットを保存する
                AssetDatabase.SaveAssets();
            }
        }
    }
}