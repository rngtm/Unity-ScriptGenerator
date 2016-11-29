///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System.Linq;
    using System.IO;
    using UnityEngine;
    using UnityEditor;
    public class TemplateUtility
    {
        /// <summary>
        /// スクリプトのTemplateファイルをロード
        /// </summary>
        public static string TemplatePath(Language language)
        {
            var monoScript = Resources.FindObjectsOfTypeAll<MonoScript>().FirstOrDefault(m => m.GetClass() == typeof(TemplateUtility));
            var path = AssetDatabase.GetAssetPath(monoScript);
            var dirPath = Path.GetDirectoryName(path);
            var templateDirPath = Path.Combine(dirPath, "Templates");
            string fileName = "";
            switch (language)
            {
                case Language.CSharp:
                    fileName = "CSharp.txt";
                    break;
                case Language.JavaScript: 
                    fileName = "JavaScript.txt"; 
                    break;
                default:
                    throw new System.NotImplementedException();
            }
            return Path.Combine(templateDirPath, fileName);
        }
    }
}
