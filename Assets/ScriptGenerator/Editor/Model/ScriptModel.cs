///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System;

    [SerializableAttribute]
    public class ScriptModel
    {
        /// <summary>
        /// デフォルトでのスクリプト保存フォルダパス
        /// </summary>
        public const string DefaultSaveFolderPath = "Assets";

        /// <summary>
        /// デフォルトでのBaseScriptName
        /// </summary>
        public const string DefaultBaseScriptName = "NewScript";
        
        /// <summary>
        /// デフォルトでのnamespace
        /// </summary>
        public const string DefaultNameSpace = "hoge";
        
        /// <summary>
        /// スクリプト保存先のフォルダパス
        /// </summary>
        public string saveFolderPath = DefaultSaveFolderPath;

        /// <summary>
        /// 名前空間
        /// </summary>
        public string @namespace = DefaultNameSpace;

        /// <summary>
        /// ベースとなるスクリプト名
        /// </summary>
        public string baseScriptName = DefaultBaseScriptName;
        
        /// <summary>
        /// 書式指定項目のリスト
        /// </summary>
        public FormatItemList[] formatItems;
    }
}
