///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System.Linq;
    using UnityEditor;

    public class ConfigLoader
    {
        /// <summary>
        /// 設定ファイルの読み込み
        /// </summary>
        public static Config LoadConfig()
        {
             return AssetDatabase.FindAssets("t:ScriptableObject")
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(Config)))
            .Where(obj => obj != null)
            .Select(obj => (Config)obj)
            .FirstOrDefault();
        }
    }
}