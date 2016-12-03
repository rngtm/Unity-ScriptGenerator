///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System;
    using UnityEditorInternal;

    [SerializableAttribute]
    public class ScriptView
    {
        /// <summary>
        /// スクリプト情報表示用のReorderableList
        /// </summary>
        public ReorderableList[] scriptLists = null;
    }
}
