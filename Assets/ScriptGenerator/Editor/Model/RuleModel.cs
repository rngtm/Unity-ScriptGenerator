///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [SerializableAttribute]
    public class RuleModel
    {
        [SerializeField] private List<RuleSelectionData> ruleSelection = new List<RuleSelectionData>();

        /// <summary>
        /// スクリプトの生成ルール
        /// </summary>
        public RuleEntity[] Rules { get; set; }

        public void SetSelection(List<RuleSelectionData> ruleSelection)
        {
            this.ruleSelection = ruleSelection;
        }

        /// <summary>
        /// 選択しているルールを取得
        /// </summary>
        public RuleEntity GetSelectedRule(int selectionIndex)
        {
            int ruleIndex = this.ruleSelection[selectionIndex].RuleIndex;
            return this.Rules[ruleIndex];
        }

        /// <summary>
        /// 選択ルールを変更
        /// </summary>
        public void SetSelection(int selectionIndex, int ruleIndex)
        {
            this.ruleSelection[selectionIndex].RuleIndex = ruleIndex;
            
            // 別のルールを選択したらサフィックスをデフォルトのものにする
            this.ruleSelection[selectionIndex].ScriptNameSuffix = this.Rules[ruleIndex].DefaultScriptNameSuffix;
        }

        /// <summary>
        /// 選択情報の個数を取得
        /// </summary>
        public int GetSelectionCount()
        {
            return (this.ruleSelection == null) ? 0 : this.ruleSelection.Count;
        }

        /// <summary>
        /// 選択情報を取得
        /// </summary>
        public List<RuleSelectionData> GetSelections()
        {
            return this.ruleSelection;
        }

        /// <summary>
        /// 選択情報を取得
        /// </summary>
        public RuleSelectionData GetSelection(int selectionIndex)
        {
            return this.ruleSelection[selectionIndex];
        }
    }
}
