///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System;
    using UnityEngine;

    /// <summary>
    /// スクリプト生成ルール
    /// </summary>
    [SerializableAttribute]
    public class ScriptGenerationRule
    {
        /// <summary>
        /// 正規表現
        /// </summary>
        public string Regex;

        /// <summary>
        /// テンプレートファイル
        /// </summary>
        public TextAsset Template;
    }

}