///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using UnityEngine;

    [System.SerializableAttribute]
    public class ModelEntity
    {
        [SerializeField] private ScriptModel scriptModel = new ScriptModel();
        [SerializeField] private RuleModel ruleModel = new RuleModel();
        
        public ScriptModel ScriptModel { get { return this.scriptModel; } }
        public RuleModel RuleModel { get { return this.ruleModel; } }
    }
}
