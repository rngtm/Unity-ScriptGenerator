///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System;
    using UnityEditorInternal;

    [SerializableAttribute]
    public class RuleView
    {
        /// <summary>
        /// ルール選択GUIの表示用ReorderableList
        /// </summary>
        public ReorderableList ruleSelectionList = null;
    }
}
