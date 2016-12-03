///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    public class ModelLoader
    {
        /// <summary>
        /// ルールの読み込み
        /// </summary>
        public static ModelEntity LoadModel()
        {
            return ScriptGenerateWindow.Instance.ModelEntity;
        }
    }
}
