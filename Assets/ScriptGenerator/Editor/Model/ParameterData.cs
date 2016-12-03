///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System;

    /// <summary>
    /// パラメーター情報
    /// </summary>
    [SerializableAttribute]
    public class ParameterData
    {
        /// <summary>
        /// スクリプトテンプレート名
        /// </summary>
        public string TemplateName = string.Empty;

        /// <summary>
        /// パラメーター名
        /// </summary>
        public string ParameterName = string.Empty;

        /// <summary>
        /// テキスト
        /// </summary>
        public string Text = ".*";

        /// <summary>
        /// パラメーターのタイプ
        /// </summary>
        public ParameterSourceType ParameterSourceType = ParameterSourceType.RegexMatchScriptNameUpper;
    }
}