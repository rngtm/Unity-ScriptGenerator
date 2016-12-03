///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using UnityEngine;

    public class CustomUI
    {
        private const string VersionLabelText = "ScriptGenerator v2.0";

        private static GUIStyle _versionLabelStyle;

        private static GUIStyle VersionLabelStyle { get { return _versionLabelStyle ?? (_versionLabelStyle = CreateVersionLabelStyle()); } }

        public static void VersionLabel()
        {
            int width = Screen.width;
            GUI.Label(new Rect(Screen.width  - width - 2, Screen.height - 72, width, 50), VersionLabelText, VersionLabelStyle);
        }
        
        private static GUIStyle CreateVersionLabelStyle()
        {
            var style = new GUIStyle(GUI.skin.GetStyle("Label"));
            var color = new Color(style.normal.textColor.r, style.normal.textColor.g, style.normal.textColor.b, 0.4f);

            style.alignment = TextAnchor.LowerRight;
            style.normal.textColor = color;

            return style;
        }
    }
}
