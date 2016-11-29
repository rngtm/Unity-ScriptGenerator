///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 書式指定項目リスト
    /// </summary>
    [SerializableAttribute]
    public class FormatItemList
    {
        public int Id;
        public List<FormatItem> FormatItems;
    }
}