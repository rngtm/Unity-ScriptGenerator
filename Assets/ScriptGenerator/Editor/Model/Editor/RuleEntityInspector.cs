///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEditorInternal;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(RuleEntity))]
    public class RuleEntityInspector : Editor
    {
        /// <summary>
        /// ラベルの大きさ
        /// </summary>
        private const float LabelWidth = 70f;

        /// <summary>
        /// ポップアップの大きさ
        /// </summary>
        private const float PopupWidth = 230f;

        /// <summary>
        /// パラメーター表示用のReorderableList
        /// </summary>
        private ReorderableList[] parameterLists;

        /// <summary>
        /// ポップアップ表示用のテキスト
        /// </summary>
        static readonly private string[] PopupTexts = System.Enum
        .GetValues(typeof(ParameterSourceType))
        .ToEnumerable<ParameterSourceType>()
        .Select(type => ConvertEnumToText(type))
        .ToArray();

        private static bool _needReset = false;

        /// <summary>
        /// インスペクターの表示
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (_needReset)
            {
                _needReset = false;
                this.Initialize();
            }

            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (this.parameterLists == null) { return; }

            this.parameterLists.ToList().ForEach(list =>
            {
                list.DoLayoutList();
            });
        }

        /// <summary>
        /// 開始時に呼ばれる
        /// </summary>
        void OnEnable()
        {
            this.Initialize();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        void Initialize()
        {
            var ruleEntity = (target as RuleEntity);
            ruleEntity.RebuildParameterDatas();

            var lists = new List<ReorderableList>();
            var keys = ruleEntity.ParameterDatas.Select(d => d.TemplateName).Distinct();
            foreach (var key in keys)
            {
                var datas = ruleEntity.ParameterDatas.Where(p => p.TemplateName == key).ToArray();
                lists.Add(CreateParameterList(key, datas));
            }
            this.parameterLists = lists.ToArray();
        }


        /// <summary>
        /// ReorderableListを表示
        /// </summary>
        ReorderableList CreateParameterList(string templateName, ParameterData[] parameterDatas)
        {
            var list = new ReorderableList(parameterDatas, typeof(ParameterData));

            // ヘッダー
            list.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, templateName);
            };

            list.elementHeight *= 3f;

            // 要素
            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.x += 8f;
                rect.height /= 3f;
                rect.width -= 14f;

                rect.y += 2;
                rect.height -= 4;

                float widthL = 100f;
                float widthR = rect.width - widthL;
                float space = 35f;

                var labelRect = new Rect(rect);
                var labelRectL = new Rect(labelRect) { width = widthL };
                var labelRectR = new Rect(labelRect) { width = widthR, x = widthL + space };

                var popupRect = new Rect(rect);
                popupRect.y = labelRect.y + rect.height;
                var popupRectL = new Rect(popupRect) { width = widthL };
                var popupRectR = new Rect(popupRect) { width = widthR, x = widthL + space };

                var textRect = new Rect(rect);
                textRect.y = popupRect.y + rect.height;
                var textRectL = new Rect(textRect) { width = widthL };
                var textRectR = new Rect(textRect) { width = widthR, x = widthL + space };

                var parameterData = parameterDatas[index];

                // パラメータ名の表示
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.LabelField(labelRectL, "パラメータ名");
                EditorGUI.TextField(labelRectR, parameterData.ParameterName);
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginChangeCheck();

                // ポップアップ
                EditorGUI.LabelField(popupRectL, "パラメータ");
                parameterData.ParameterSourceType = (ParameterSourceType)EditorGUI.Popup(popupRectR, (int)parameterData.ParameterSourceType, PopupTexts);

                // テキスト入力
                EditorGUI.LabelField(textRectL, GetLabel(parameterData.ParameterSourceType));
                parameterData.Text = EditorGUI.TextField(textRectR, parameterData.Text);

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(target);
                }
            };

            // フッター
            list.drawFooterCallback = (rect) => { };

            list.draggable = false;
            list.onSelectCallback = (index) => { };

            list.drawElementBackgroundCallback = (index, rect, isActive, isFocused) => { };

            return list;
        }

        /// <summary>
        /// ロード時に呼ばれる 
        /// </summary>
        [InitializeOnLoadMethodAttribute]
        [DidReloadScripts]
        public static void OnLoad()
        {
            _needReset = true;
        }

        /// <summary>
        /// ラベル取得
        /// </summary>
        static private string GetLabel(ParameterSourceType type)
        {
            string text = "";
            switch (type)
            {
                case ParameterSourceType.RegexMatchScriptNameUpper:
                case ParameterSourceType.RegexMatchScriptNameLower:
                    text = "正規表現(Regex)";
                    break;
                case ParameterSourceType.Text:
                    text = "テキスト";
                    break;
                default:
                    Debug.LogError("Unknown type: " + type.ToString());
                    break;
            }
            return text;
        }

        /// <summary>
        /// ParameterSourceTypeをテキストへ変換
        /// </summary>
        static private string ConvertEnumToText(ParameterSourceType type)
        {
            string text = "";
            switch (type)
            {
                case ParameterSourceType.RegexMatchScriptNameUpper:
                    text = "Upper case - 正規表現(Regex)にマッチする文字列をスクリプト名から取り出す(先頭:大文字)";
                    break;
                case ParameterSourceType.RegexMatchScriptNameLower:
                    text = "Lower case - 正規表現(Regex)にマッチする文字列をスクリプト名から取り出す(先頭:小文字)";
                    break;
                case ParameterSourceType.Text:
                    text = "テキストそのまま";
                    break;
                default:
                    Debug.LogError("Unknown type: " + type.ToString());
                    break;
            }
            return text;
        }
    }
}