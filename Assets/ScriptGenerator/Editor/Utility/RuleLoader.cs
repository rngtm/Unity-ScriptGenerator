///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System.Linq;
    using UnityEditor;

    public class RuleLoader : AssetPostprocessor
    {
        /// <summary>
        /// アセットのインポート完了時に呼ばれる
        /// </summary>
        static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) 
        {
            ScriptGenerateWindow.OnLoad();
            RuleEntityInspector.OnLoad();
        }

        /// <summary>
        /// ルールの読み込み
        /// </summary>
        public static RuleEntity[] LoadRules()
        {
             return (RuleEntity[])AssetDatabase.FindAssets("t:RuleEntity")
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(RuleEntity)))
            .Select(obj => (RuleEntity)obj)
            .Where(rule => rule.DefaultTemplateAsset != null)
            .ToArray();
        }
    }
}