///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System;
    using System.Linq;
    using UnityEngine;
    using UnityEditor;

    [SerializableAttribute]
    public class RuleViewModel : ViewModelBase
    {
        private RuleModel Model { get { return base.ModelEntity.RuleModel; } }

        private string currentRuleName = "";

        /// <summary>
        /// ウィンドウ生成時に呼ばれる
        /// </summary>
        public void OnCreateWindow()
        {
            this.ReloadRules();
        }
        
        /// <summary>
        /// ロード時に呼ばれる
        /// </summary>
        public void OnReload()
        {
            this.ReloadRules();
        }

        /// <summary>
        /// 別のルールが選択されたら呼ばれる
        /// </summary>
        private void ResetSelection()
        {
            var defaultRule = this.Model.rules
            .Select((rule, i) => new { rule = rule, index = i })
            .FirstOrDefault(d => d.rule.name == RuleModel.DefaultRuleName);
            if (defaultRule == null)
            {
                this.Model.currentRuleIndex = 0;
            }
            else
            {
                this.Model.currentRuleIndex = defaultRule.index;
            }
        }

        /// <summary>
        /// ルールのリロード
        /// </summary>
        public void ReloadRules()
        {
            this.Model.rules = RuleLoader.LoadRules();
            if (!this.Model.rules.Any(rule => rule.name == this.currentRuleName))
            {
                this.ResetSelection();
            }

            this.currentRuleName = this.GetCurrentRule().name;
        }

        /// <summary>
        /// ルール選択のGUI表示
        /// </summary>
        public void ShowRuleGUI()
        {
            var ruleNames = this.Model.rules.Select(rule => rule.name).ToArray();
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            this.Model.currentRuleIndex = EditorGUILayout.Popup("スクリプト生成ルール", this.Model.currentRuleIndex, ruleNames);

            if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(44f)))
            {
                EditorGUIUtility.PingObject(this.Model.rules[this.Model.currentRuleIndex]);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 現在選択中のルールを取得
        /// </summary>
        public RuleEntity GetCurrentRule()
        {
            return this.Model.rules[this.Model.currentRuleIndex];
        }
    }
}
