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
        /// 初期状態でのルール
        /// </summary>
        private const string DefaultRuleName = "Default";

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
        [SerializeField] private string baseScriptName = "NewScript";

        /// <summary>
        /// 書式指定項目のリスト
        /// </summary>
        [SerializeField] private FormatItemList[] formatItems;

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

        /// <summary>
        /// 現在選択中のルール
        /// </summary>
        [SerializeField] private int currentRuleIndex = 0;

        /// <summary>
        /// AdvancedなUIを表示させるかどうか
        /// </summary>
        [SerializeField] private bool showAdvancedUI = false;

        /// <summary>
        /// スクリプトの生成ルール
        /// </summary>
        private RuleEntity[] rules;

        private static bool _needReset = false;

        /// <summary>
        /// ウィンドウを開く
        /// </summary>
        [MenuItem("Tools/Script Generator")]
        // [MenuItem("Assets/Script Generator", false, 1)]
        static void Open()
        {
            var window = GetWindow<ScriptGenerateWindow>();

            window.ReloadRules();

            var defaultRule = window.rules
            .Select((rule, i) => new { rule = rule, index = i })
            .FirstOrDefault(d => d.rule.name == DefaultRuleName);
            if (defaultRule == null)
            {
                window.currentRuleIndex = 0;
            }
            else
            {
                window.currentRuleIndex = defaultRule.index;
            }
        }

        /// <summary>
        /// ロード時に呼ばれる 
        /// </summary>
        [InitializeOnLoadMethodAttribute]
        [DidReloadScripts]
        public static void OnLoad()
        {
            _needReset = true;
        }

        /// <summary>
        /// ウィンドウがアクティブになったときに呼ばれる
        /// </summary>
        private void OnEnable()
        {
            // this.ExtractFormatItems();
            if (this.formatItems != null)
            {
                this.RebuildList();
                this.Repaint();
            }
        }

        /// <summary>
        /// ウィンドウの描画処理
        /// </summary>
        private void OnGUI()
        {
            if (Event.current.keyCode == KeyCode.Escape) { GUI.FocusControl(""); Repaint(); }

            if (_needReset)
            {
                _needReset = false;
                this.ReloadRules();

                if (this.lists != null)
                {
                    this.RebuildList();
                }
            }

            if (this.rules == null) { this.ReloadRules(); }

            this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);

            this.CreateScriptButton();

            this.@namespace = EditorGUILayout.TextField("namespace", this.@namespace);
            EditorGUI.BeginChangeCheck();

            this.baseScriptName = EditorGUILayout.TextField("Base Script Name", this.baseScriptName);
            if (EditorGUI.EndChangeCheck())
            {
                this.ExtractFormatItems();
                this.RebuildList();
            }

            if (this.showAdvancedUI = EditorGUILayout.Foldout(this.showAdvancedUI, "Advanced Settings"))
            {
                EditorGUI.indentLevel++;
                var ruleNames = this.rules.Select(rule => rule.name).ToArray();
                EditorGUILayout.BeginHorizontal();
                this.currentRuleIndex = EditorGUILayout.Popup("スクリプト生成ルール", this.currentRuleIndex, ruleNames);
                if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(44f)))
                {
                    EditorGUIUtility.PingObject(this.rules[this.currentRuleIndex]);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }

            // ReorderableListを表示
            if (this.lists != null)
            {
                this.lists.ToList().ForEach(list => list.DoLayoutList());
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 書式指定項目を抽出
        /// </summary>
        private void ExtractFormatItems()
        {
            var list = new List<FormatItemList>();
            var matches = Regex.Matches(this.baseScriptName, @"(?<={)+[^\{\}]*(?=})"); // {}で囲まれた文字列を抽出
            var keys = new List<int>();
            for (int i = 0; i < matches.Count; i++)
            {
                int key;
                bool success = int.TryParse(matches[i].Value, out key);
                if (!success) { Debug.LogError("invalid format"); continue; }
                if (keys.Contains(key)) { continue; }
                keys.Add(key);
                list.Add(new FormatItemList { Id = key, FormatItems = new List<FormatItem> { new FormatItem() } });
            }
            this.formatItems = list.ToArray();
        }

        /// <summary>
        /// ReorderableListを作成しなおす
        /// </summary>
        private void RebuildList()
        {
            this.lists = this.formatItems
            .Where(d => d != null)
            .Select(d => CreateReorderableList(d))
            .ToArray();
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

                var rule = this.rules[this.currentRuleIndex];
                if (this.formatItems == null || this.formatItems.Length == 0)
                {
                    // スクリプト作成
                    var scriptPath = relativePath + Path.DirectorySeparatorChar + this.baseScriptName + extension;
                    var exists = AssetDatabase.LoadAllAssetsAtPath(scriptPath).Length != 0;
                    if (exists) { Debug.LogWarning("Exists: " + scriptPath); return; }

                    var templatePath = AssetDatabase.GetAssetPath(rule.DefaultTemplateAsset);
                    var scriptAsset = CreateScriptAssetFromTemplate(scriptPath, templatePath);
                    Debug.Log("Create: " + scriptPath + "\nRule: " + this.rules[this.currentRuleIndex].name, scriptAsset);
                }
                else
                {
                    // スクリプト一括作成
                    CreateDP(this.formatItems.Select(ds => ds.FormatItems.Select(d => d.Name)).ToArray())
                    .ToList()
                    .ForEach(strs =>
                    {
                        var scriptName = string.Format(baseScriptName, strs);
                        if (scriptName.Length == 0) { return; }

                        var match = rule.ScriptGenerationRules.FirstOrDefault(r => Regex.Match(scriptName, r.Regex).Success);
                        var template = (match != null) ? match.Template : rule.DefaultTemplateAsset;
                        var templatePath = AssetDatabase.GetAssetPath(template);
                        var scriptPath = relativePath + Path.DirectorySeparatorChar + scriptName + extension;
                        var exists = AssetDatabase.LoadAllAssetsAtPath(scriptPath).Length != 0;
                        if (exists) { Debug.LogWarning("Exists: " + scriptPath); return; }

                        var scriptAsset = CreateScriptAssetFromTemplate(scriptPath, templatePath);
                        Debug.Log("Create: " + scriptPath + "\nRule: " + this.rules[this.currentRuleIndex].name, scriptAsset);
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
        /// ルールのリロード
        /// </summary>
        private void ReloadRules()
        {
            this.rules = (RuleEntity[])AssetDatabase.FindAssets("t:RuleEntity")
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(RuleEntity)))
            .Select(obj => (RuleEntity)obj)
            .ToArray();
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