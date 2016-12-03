///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    public abstract class ViewModelBase
    {
        private static ModelEntity _model = null;
        
        protected ModelEntity ModelEntity { get { return _model ?? (_model = ModelLoader.LoadModel()); } }
    }
}
