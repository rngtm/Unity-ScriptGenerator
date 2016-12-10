///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Linq;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Callbacks;

    /// <summary>
    /// スクリプト作成を行うウィンドウ
    /// </summary>
    public class ScriptGenerateWindow : EditorWindow
    {
        /// <summary>
        /// デフォルトでのスクリプト保存フォルダパス
        /// </summary>
        public const string DefaultSaveFolderPath = "Assets";
        
        /// <summary>
        /// 作成スクリプトの言語
        /// </summary>
        private const Language ScriptLanguage = Language.CSharp;
        
        /// <summary>
        /// スクリプト保存先のフォルダパス
        /// </summary>
        public string saveFolderPath = DefaultSaveFolderPath;

        /// <summary>
        /// スクロール位置
        /// </summary>
        public Vector2 scrollPosition = Vector2.zero;

        /// <summary>
        /// AdvancedなUIを表示させるかどうか
        /// </summary>
        public bool showAdvancedUI = false;

        [SerializeField] private ModelEntity modelEntity;
        [SerializeField] private ScriptViewModel ScriptViewModel = new ScriptViewModel(); 
        [SerializeField] private RuleViewModel RuleViewModel = new RuleViewModel();

        public ModelEntity ModelEntity { get { return this.modelEntity ?? (this.modelEntity = new ModelEntity()); } }

        private static bool _needReload = false;
        private static ScriptGenerateWindow _instance;
        public static ScriptGenerateWindow Instance { get { return _instance ?? (_instance = EditorWindow.GetWindow<ScriptGenerateWindow>()); } }

        /// <summary>
        /// ロード時に呼ばれる 
        /// </summary>
        [InitializeOnLoadMethodAttribute]
        [DidReloadScripts]
        public static void OnLoad()
        {
            _needReload = true;
        }

        /// <summary>
        /// ウィンドウを開く
        /// </summary>
        [MenuItem("Tools/Script Generator")]
        static void Open()
        {
            var window = GetWindow<ScriptGenerateWindow>();
            window.RuleViewModel.OnCreateWindow();
            window.ScriptViewModel.OnCreateWindow();
        }

        /// <summary>
        /// ウィンドウの描画処理
        /// </summary>
        private void OnGUI()
        {
            if (Event.current.rawType == EventType.KeyDown) 
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.Escape:
                        GUI.FocusControl(""); 
                        Repaint(); 
                        break;
                    case KeyCode.RightArrow:
                        this.RuleViewModel.NextRule();
                        this.Repaint();
                        break;
                    case KeyCode.LeftArrow:
                        this.RuleViewModel.PreviousRule();
                        this.Repaint();
                        break;
                        
                } 
            }

            if (_needReload )
            {
                _needReload = false;
                this.RuleViewModel.OnReload();
                this.ScriptViewModel.OnReload();
                this.Repaint();
            }

            CustomUI.VersionLabel();
            
            this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);

            if (GUILayout.Button("スクリプト生成"))
            {
                this.DoCreateScriptButton();
            }

            this.ScriptViewModel.DoLayoutBasicUI();
            
            if (this.showAdvancedUI = EditorGUILayout.Foldout(this.showAdvancedUI, "Advanced Settings"))
            {
                AdvancedSettingsGUI();
            }

            this.ScriptViewModel.DoLayoutFormatterList();

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// AdvancedなUIの表示
        /// </summary>
        private void AdvancedSettingsGUI()
        {
            this.RuleViewModel.ShowRuleGUI();
        }

        /// <summary>
        /// スクリプト作成実行
        /// </summary>
        private void DoCreateScriptButton()
        {
            if (!Directory.Exists(this.saveFolderPath)) { this.saveFolderPath = DefaultSaveFolderPath; }
            string saveFolderPath = EditorUtility.SaveFolderPanel("保存先のフォルダを指定してください", this.saveFolderPath, "");
            if (saveFolderPath.Length == 0) { return; }
            this.saveFolderPath = saveFolderPath;

            // スクリプト一括作成
            this.CreateScripts();
        }

        /// <summary>
        /// 選択しているルールからスクリプトを作成
        /// </summary>
        private void CreateScripts()
        {
            for (int i = 0; i < this.ModelEntity.RuleModel.GetSelectionCount(); i++)
            {
                this.CreateScript(i);
            }
        }

        /// <summary>
        /// 選択しているルールからスクリプトを作成する
        /// </summary>
        private void CreateScript(int selectionIndex)
        {            
            string relativePath = "Assets" + this.saveFolderPath.Substring(Application.dataPath.Length);
            string extension = GetExntension(ScriptLanguage);

            var currentRule = this.RuleViewModel.GetSelectedRule(selectionIndex);
            var scriptNameSuffix = this.RuleViewModel.GetScriptNameSuffix(selectionIndex);

            //　スクリプト 一括作成
            this.ScriptViewModel.ScriptNames()
            .Select(scriptName => scriptName + scriptNameSuffix)
            .Where(scriptName => !string.IsNullOrEmpty(scriptName))
            .ToList()
            .ForEach(scriptName =>
            {
                var match = currentRule.ScriptGenerationRules.FirstOrDefault(r => Regex.Match(scriptName, r.Regex).Success);
                var template = (match != null) ? match.Template : currentRule.DefaultTemplateAsset;
                var templatePath = AssetDatabase.GetAssetPath(template);
                var scriptPath = relativePath + Path.DirectorySeparatorChar + scriptName + extension;
                if (AssetDatabase.LoadAllAssetsAtPath(scriptPath).Length != 0) 
                {
                     Debug.LogWarning("Exists: " + scriptPath, AssetDatabase.LoadAssetAtPath(scriptPath, typeof(UnityEngine.Object))); 
                     return; 
                }
                var scriptAsset = CreateScriptAssetFromTemplate(scriptName, scriptPath, templatePath, currentRule.GetParameterDatas(template.name));
                Debug.Log("Create: " + scriptPath + "\nRule: " + currentRule.name + "\nTemplate:" + template.name + "\n", scriptAsset);
            });
        }

        /// <summary>
        /// 拡張子を取得
        /// </summary>
        private static string GetExntension(Language language)
        {
            string ext = string.Empty;
            switch (language)
            {
                case Language.CSharp:
                    ext = ".cs";
                    break;
                default:
                    Debug.LogError("Unknown launguage:" + language.ToString());
                    break;
            }
            return ext;
        }

        /// <summary>
        /// テンプレートファイルからスクリプトを生成
        /// </summary>
        private UnityEngine.Object CreateScriptAssetFromTemplate(string scriptName, string scriptPath, string templatePath, ParameterData[] parameterDatas)
        {
            StreamReader streamReader = new StreamReader(templatePath);
            string text = streamReader.ReadToEnd();
            streamReader.Close();

            // パラメーターを使った置換
            parameterDatas.ToList().ForEach(data =>
            {
                var src = scriptName;
                var replacement = "";
                switch (data.ParameterSourceType)
                {
                    case ParameterSourceType.Text:
                        // テキストそのままで置換する
                        replacement = data.Text;
                        break;
                    case ParameterSourceType.RegexMatchScriptNameLower:
                        // スクリプト名を使って置換する
                        src = char.ToLower(src[0]) + src.Substring(1);
                        replacement  = Regex.Match(src, data.Text).Value;
                        break;
                    case ParameterSourceType.RegexMatchScriptNameUpper:
                        // スクリプト名を使って置換する
                        src = char.ToUpper(src[0]) + src.Substring(1);
                        replacement  = Regex.Match(src, data.Text).Value;
                        break;
                    default:
                        Debug.LogError("Unknown type: " + data.ParameterSourceType.ToString());
                        break;
                }

                text = text.Replace("#{" + data.ParameterName + "}#", replacement);
            });

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(scriptPath);
            text = Regex.Replace(text, "#NAME#", fileNameWithoutExtension);
            string text2 = Regex.Replace(fileNameWithoutExtension, " ", string.Empty);
            text = Regex.Replace(text, "#NAMESPACE#", this.ModelEntity.ScriptModel.@namespace);
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
            
        /// <summary>
        /// 言語
        /// </summary>
        public enum Language
        {
            CSharp,
        }
    }
}