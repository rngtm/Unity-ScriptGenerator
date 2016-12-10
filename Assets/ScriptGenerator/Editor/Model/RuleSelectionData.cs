///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System;

    [SerializableAttribute]
    public class RuleSelectionData
    {
        /// <summary>
        /// 選択ルールのインデックス
        /// </summary>
        public int RuleIndex;
        
        /// <summary>
        /// 生成スクリプトの末尾につける文字列
        /// </summary>
        public string ScriptNameSuffix;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RuleSelectionData()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RuleSelectionData(int ruleIndex, RuleEntity rule)
        {
            this.Initialize(ruleIndex, rule);
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize(int ruleIndex, RuleEntity rule)
        {
            this.RuleIndex = ruleIndex;
            this.ScriptNameSuffix = rule.DefaultScriptNameSuffix;
        }
    }
}
