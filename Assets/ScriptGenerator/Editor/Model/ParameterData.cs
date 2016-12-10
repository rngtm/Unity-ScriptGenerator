///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System;
    using UnityEngine;

    /// <summary>
    /// パラメーター情報
    /// </summary>
    [SerializableAttribute]
    public class ParameterData
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ParameterData(string templateName, string parameterName)
        {
            this.templateName = templateName;
            this.parameterName = parameterName;
        }

        public const string ParameterPrefix = "___";

        [SerializeField] private string templateName = string.Empty;
        [SerializeField] private string parameterName = string.Empty;
        [SerializeField] private string text = ".*";
        [SerializeField] private ParameterSourceType parameterSourceType = ParameterSourceType.RegexMatchScriptNameUpper;

        /// <summary>
        /// スクリプトテンプレート名
        /// </summary>
        public string TemplateName { get { return this.templateName; } set { this.templateName = value; } }

        /// <summary>
        /// スクリプトテンプレート名
        /// </summary>
        public string ParameterName { get { return this.parameterName; } set { this.parameterName = value; } }

        /// <summary>
        /// テキスト
        /// </summary>
        public string Text { get { return this.text; } set { this.text = value; } }

        /// <summary>
        /// パラメーターのタイプ
        /// </summary>
        public ParameterSourceType ParameterSourceType { get { return this.parameterSourceType; } set { this.parameterSourceType = value;} }

    }
}