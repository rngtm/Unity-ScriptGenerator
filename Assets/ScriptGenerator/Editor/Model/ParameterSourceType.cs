///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    public enum ParameterSourceType
    {
        /// <summary>
        /// BaseScriptNameをRegexで加工した文字列をソースとして使う
        /// </summary>
        RegexMatchScriptNameUpper,
        
        /// <summary>
        /// BaseScriptNameをRegexで加工した文字列をソースとして使う (先頭小文字)
        /// </summary>
        RegexMatchScriptNameLower,

        /// <summary>
        /// テキストをそのまま使う
        /// </summary>
        Text,
    }
}