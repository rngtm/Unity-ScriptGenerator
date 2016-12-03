///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// パラメーター情報リスト
    /// </summary>
    [SerializableAttribute]
    public class ParameterDataList
    {
        /// <summary>
        /// パラメーター情報
        /// </summary>
        public List<ParameterData> ParameterDatas;
    }
}