///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Linq;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEditorInternal;

    /// <summary>
    /// スクリプト作成を行うウィンドウ
    /// </summary>
    public class ScriptGenerateWindow : EditorWindow
    {
        /// <summary>
        /// デフォルトでのスクリプト保存フォルダパス
        /// </summary>
        private const string DefaultSaveFolderPath = "Assets";

        /// <summary>
        /// 作成スクリプトの言語
        /// </summary>
        private const Language ScriptLanguage = Language.CSharp;

        /// <summary>
        /// 名前空間
        /// </summary>
        [SerializeField] private string @namespace = "hoge";

        /// <summary>
        /// ベースとなるスクリプト名
        /// </summary>
        [SerializeField] private string baseScriptName = "NewBehaviourScript";

        /// <summary>
          /// 書式指定項目のリスト
        /// </summary>
        [SerializeField] private List<FormatItemList> formatItemLists = new List<FormatItemList>();

        /// <summary>
        /// スクリプト情報表示用のReorderableList
        /// </summary>
        [SerializeField] private ReorderableList[] lists = null;

        /// <summary>
        /// スクロール位置
        /// </summary>
        [SerializeField] private Vector2 scrollPosition = Vector2.zero;

        /// <summary>
        /// スクリプト保存先のフォルダパス
        /// </summary>
        [SerializeField] private string saveFolderPath = DefaultSaveFolderPath;

        private static bool _needReset = false;

        /// <summary>
        /// ウィンドウを開く
        /// </summary>
        [MenuItem("Tools/Script Generator")]
        // [MenuItem("Assets/Script Generator", false, 1)]
        static void Open()
        {
            GetWindow<ScriptGenerateWindow>();
        }

        /// <summary>
        /// スクリプトがリロードされたときに呼ばれる 
        /// </summary>
        [DidReloadScripts]
        static void OnLoadScript()
        {
            _needReset = true;
        }

        /// <summary>
        /// ウィンドウがアクティブになったときに呼ばれる
        /// </summary>
        void OnEnable()
        {
            this.RebuildList();
            this.Repaint();
        }

        /// <summary>
        /// ウィンドウの描画処理
        /// </summary>
        void OnGUI()
        {
            if (Event.current.keyCode == KeyCode.Escape) { GUI.FocusControl(""); Repaint(); }

            if (_needReset)
            {
                _needReset = false;
                this.RebuildList();
            }

            this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
            this.CreateScriptButton();
            this.@namespace = EditorGUILayout.TextField("namespace", this.@namespace);
            EditorGUI.BeginChangeCheck();
            this.baseScriptName = EditorGUILayout.TextField("Base Script Name", this.baseScriptName);
            if (EditorGUI.EndChangeCheck())
            {
                this.Reset();
                this.RebuildList();
            }

            // ReorderableListを表示
            if (this.lists != null)
            {
                this.lists.ToList().ForEach(list => list.DoLayoutList());
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// リセット
        /// </summary>
        void Reset()
        {
            this.formatItemLists.Clear();
            MatchCollection matches = Regex.Matches(this.baseScriptName, @"(?<={)+[^\{\}]*(?=})"); // {}で囲まれた文字列を抽出

            var keys = new List<int>();
            for (int i = 0; i < matches.Count; i++)
            {
                int key;
                bool success = int.TryParse(matches[i].Value, out key);
                if (!success) { Debug.LogError("invalid format"); continue; }
                if (keys.Contains(key)) { continue; }
                keys.Add(key);
                this.formatItemLists.Add(new FormatItemList { Id = key, FormatItems = new List<FormatItem> { new FormatItem() } });
            }
        }

        /// <summary>
        /// ReorderableListを作成しなおす
        /// </summary>
        void RebuildList()
        {
            this.lists = this.formatItemLists.Select(d => CreateReorderableList(d)).ToArray();
        }

        /// <summary>
        /// string配列から直積を作成
        /// </summary>
        static IEnumerable<string[]> CreateDP(IEnumerable<string>[] strs)
        {
            const char split = ',';
            var result = strs[0];
            if (strs.Length > 1)
            {
                for (int i = 0; i < strs.Length - 1; i++)
                {
                    var s2 = result.SelectMany(a => strs[i + 1], (a, b) => a + split + b).ToList();
                    result = s2;
                }
            }
            return result.Select(s => s.Split(split));
        }

        /// <summary>
        /// スクリプト作成ボタン
        /// </summary>
        private void CreateScriptButton()
        {
            if (GUILayout.Button("スクリプト生成"))
            {
                if (!Directory.Exists(this.saveFolderPath)) { this.saveFolderPath = DefaultSaveFolderPath; }
                string saveFolderPath = EditorUtility.SaveFolderPanel("保存先のフォルダを指定してください", this.saveFolderPath, "");
                if (saveFolderPath.Length == 0) { return; }
                this.saveFolderPath = saveFolderPath;

                string relativePath = "Assets" + this.saveFolderPath.Substring(Application.dataPath.Length);
                string extension = GetExntension(ScriptLanguage);

                if (this.formatItemLists.Count == 0)
                {
                    // スクリプト作成
                    var scriptPath = relativePath + Path.DirectorySeparatorChar + this.baseScriptName + extension;
                    var exists = AssetDatabase.LoadAllAssetsAtPath(scriptPath).Length != 0;
                    if (exists) { Debug.LogWarning("Exists: " + scriptPath); return; }

                    var scriptAsset = CreateScriptAssetFromTemplate(scriptPath, TemplateUtility.TemplatePath(ScriptLanguage));
                    Debug.Log("Create: " + scriptPath, scriptAsset);
                }
                else
                {
                    // スクリプト一括作成
                    CreateDP(this.formatItemLists.Select(ds => ds.FormatItems.Select(d => d.Name)).ToArray())
                    .ToList()
                    .ForEach(strs =>
                    {
                        var scriptName = string.Format(baseScriptName, strs);
                        if (scriptName.Length == 0) { return; }
                        var scriptPath = relativePath + Path.DirectorySeparatorChar + scriptName + extension;
                        var exists = AssetDatabase.LoadAllAssetsAtPath(scriptPath).Length != 0;
                        if (exists) { Debug.LogWarning("Exists: " + scriptPath); return; }

                        var scriptAsset = CreateScriptAssetFromTemplate(scriptPath, TemplateUtility.TemplatePath(ScriptLanguage));
                        Debug.Log("Create: " + scriptPath, scriptAsset);
                    });
                }
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// 拡張子を取得
        /// </summary>
        private static string GetExntension(Language language)
        {
            switch (language)
            {
                case Language.CSharp:
                    return ".cs";
                case Language.JavaScript: 
                    return ".js";
                default:
                    throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// ReorderableListを作成する
        /// </summary>
        private static ReorderableList CreateReorderableList(FormatItemList formatList)
        {
            var list = new ReorderableList((IList)formatList.FormatItems, typeof(string));

            // ヘッダー描画
            list.drawHeaderCallback += (rect) =>
            {
                EditorGUI.LabelField(rect, "{" + formatList.Id + "}");
            };

            // 要素の描画
            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.y += 2;
                rect.height -= 4;

                var scriptData = (FormatItem)list.list[index];
                scriptData.Name = EditorGUI.TextField(rect, scriptData.Name);
            };

            // フッター描画
            list.drawFooterCallback = (rect) =>
            {
                if (list.count == 0)
                {
                    rect.position -= new Vector2(0f, list.elementHeight + list.headerHeight + 3f);
                }
                else
                {
                    rect.position -= new Vector2(0f, list.elementHeight * list.count + list.headerHeight + 3f);
                }
                ReorderableList.defaultBehaviours.DrawFooter(rect, list);
            };

            return list;
        }

        /// <summary>
        /// テンプレートファイルからスクリプトを生成
        /// </summary>
        private UnityEngine.Object CreateScriptAssetFromTemplate(string scriptPath, string templatePath)
        {
            StreamReader streamReader = new StreamReader(templatePath);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(scriptPath);
            text = Regex.Replace(text, "#NAME#", fileNameWithoutExtension);
            string text2 = Regex.Replace(fileNameWithoutExtension, " ", string.Empty);
            text = Regex.Replace(text, "#NAMESPACE#", this.@namespace);
            text = Regex.Replace(text, "#SCRIPTNAME#", text2);
            if (char.IsUpper(text2, 0))
            {
                text2 = char.ToLower(text2[0]) + text2.Substring(1);
                text = Regex.Replace(text, "#SCRIPTNAME_LOWER#", text2);
            }
            else
            {
                text2 = "my" + char.ToUpper(text2[0]) + text2.Substring(1);
                text = Regex.Replace(text, "#SCRIPTNAME_LOWER#", text2);
            }
            bool encoderShouldEmitUTF8Identifier = true;
            bool throwOnInvalidBytes = false;
            UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
            bool append = false;
            StreamWriter streamWriter = new StreamWriter(Path.GetFullPath(scriptPath), append, encoding);
            streamWriter.Write(text);
            streamWriter.Close();
            AssetDatabase.ImportAsset(scriptPath);
            return AssetDatabase.LoadAssetAtPath(scriptPath, typeof(UnityEngine.Object));
        }
    }
}