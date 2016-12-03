///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using UnityEngine;

    /// <summary>
    /// スクリプト生成ルール
    /// </summary>
    // [CreateAssetMenu(fileName = "NewRule", menuName = "ScriptGenerator/Rule")]
    public class RuleEntity : ScriptableObject
    {
        [HeaderAttribute("通常のスクリプトテンプレート")]
        [SerializeField] private TextAsset defaultTemplate;

        [HeaderAttribute("正規表現(Regex)にマッチする場合のスクリプトテンプレート")]
        [SerializeField] private List<ScriptGenerationRule> scriptGenerationRules = new List<ScriptGenerationRule>();
        [SerializeField, HideInInspector] private ParameterData[] parameterDatas = new ParameterData[0];

        /// <summary>
        /// パラメーター
        /// </summary>
        public ParameterData[] ParameterDatas { get { return this.parameterDatas; } }

        /// <summary>
        /// デフォルトのテンプレートファイル
        /// </summary>
        public TextAsset DefaultTemplateAsset { get { return this.defaultTemplate; } }
        
        /// <summary>
        /// スクリプト生成ルール
        /// </summary>
        public List<ScriptGenerationRule> ScriptGenerationRules { get { return this.scriptGenerationRules; } }

        /// <summary>
        /// パラメーターリストの再構築
        /// </summary>
        public void RebuildParameterDatas()
        {
            if (this.defaultTemplate == null) { return; }
            
            var templates = new List<TextAsset>();
            templates.Add(this.DefaultTemplateAsset);
            templates.AddRange(this.scriptGenerationRules.Where(rule => rule != null).Select(rule => rule.Template));
            
            var newParameterDatas = templates
            .SelectMany(template => ExtractParameterNames(template.text).Select(name => new {Name = name, Template = template}))
            .Distinct()
            .Select(data => this.parameterDatas.FirstOrDefault(d => d.ParameterName == data.Name) ?? new ParameterData { ParameterName = data.Name, TemplateName = data.Template.name})
            .ToArray();
            
            this.parameterDatas = newParameterDatas;
        }

        /// <summary>
        /// テンプレートファイルに対応するパラメータ一覧を取得
        /// </summary>
        public ParameterData[] GetParameterDatas(string templateName)
        {
            return this.parameterDatas.Where(d => d.TemplateName == templateName).ToArray();
        }

        /// <summary>
        /// テキストからパラメーター名を抽出
        /// </summary>
        private static IEnumerable<string> ExtractParameterNames(string text)
        {
            var matches = Regex.Matches(text, @"(?<=#{)+[^\#\#]*(?=}#)");  // #{}#で囲まれた部分を抽出
            for (int i = 0; i < matches.Count; i++)
            {
                yield return matches[i].Value;
            }
        }
    }
}