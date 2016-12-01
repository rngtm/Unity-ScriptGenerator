///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// スクリプト生成ルール
    /// </summary>
    public class RuleEntity : ScriptableObject
    {
        [HeaderAttribute("通常のスクリプトテンプレート")]
        [SerializeField] private TextAsset defaultTemplate;

        [HeaderAttribute("正規表現(Regex)にマッチする場合のスクリプトテンプレート")]
        [SerializeField] private List<ScriptGenerationRule> scriptGenerationRules = new List<ScriptGenerationRule>();

        /// <summary>
        /// デフォルトのテンプレートファイル
        /// </summary>
        public TextAsset DefaultTemplateAsset { get { return this.defaultTemplate; } }
        
        /// <summary>
        /// スクリプト生成ルール
        /// </summary>
        public List<ScriptGenerationRule> ScriptGenerationRules { get { return this.scriptGenerationRules; } }
    }
}