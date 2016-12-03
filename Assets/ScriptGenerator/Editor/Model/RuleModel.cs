///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System;
    using System.Collections;
    using UnityEngine;

    [SerializableAttribute]
    public class RuleModel
    {
        /// <summary>
        /// 初期状態でのルール
        /// </summary>
        public const string DefaultRuleName = "Default";

        /// <summary>
        /// 現在選択中のルール
        /// </summary>
        public int currentRuleIndex = 0;

        /// <summary>
        /// スクリプトの生成ルール
        /// </summary>
        public RuleEntity[] rules;
    }
}
