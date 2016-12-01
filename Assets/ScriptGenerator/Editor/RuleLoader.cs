///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using UnityEditor;

    public class RuleLoader : AssetPostprocessor
    {
		/// <summary>
        /// アセットのインポート完了時に呼ばれる
        /// </summary>
		static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) 
		{
			ScriptGenerateWindow.OnLoad();
		}
    }
}