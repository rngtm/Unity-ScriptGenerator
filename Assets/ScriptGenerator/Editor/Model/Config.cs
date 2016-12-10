///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using UnityEngine;

    /// <summary>
    /// 設定ファイル
    /// </summary>
    public class Config : ScriptableObject
    {
        [HeaderAttribute("ウィンドウを開いた時に選択されるルール")]
        [SerializeField] private RuleEntity defaultRule;

        /// <summary>
        /// ウィンドウを開いた時に選択されるルール
        /// </summary>
        public RuleEntity DefaultRule { get { return this.defaultRule; } }
    }
}
