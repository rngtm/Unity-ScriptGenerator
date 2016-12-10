﻿///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    public class ModelLoader
    {
        /// <summary>
        /// モデルの取得
        /// </summary>
        public static ModelEntity LoadModel()
        {
            return ScriptGenerateWindow.Instance.ModelEntity;
        }
    }
}
