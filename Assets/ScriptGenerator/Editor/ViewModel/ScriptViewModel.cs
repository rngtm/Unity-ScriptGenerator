///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;

    [SerializableAttribute]
    public class ScriptViewModel : ViewModelBase
    {
        private ScriptModel model { get { return base.ModelEntity.ScriptModel; } }

        private ScriptView view = new ScriptView();

        /// <summary>
        /// ウィンドウ生成時に呼ばれる
        /// </summary>
        public void OnCreateWindow()
        {
            this.model.baseScriptName = ScriptModel.DefaultBaseScriptName;
            this.ExtractScriptFormatItems();
            this.RebuildScriptList();
        }

        /// <summary>
        /// リロード時に呼ばれる
        /// </summary>
        public void OnReload()
        {
            if (this.model.formatItems == null || this.model.formatItems.Length == 0)
            {
               this.ExtractScriptFormatItems();
            }
            this.RebuildScriptList();
        }

        /// <summary>
        /// スクリプト情報の入力UIを標示
        /// </summary>
        public void DoLayoutBasicUI()
        {
            this.model.@namespace = EditorGUILayout.TextField("namespace", this.model.@namespace);
            EditorGUI.BeginChangeCheck();

            this.model.baseScriptName = EditorGUILayout.TextField("Base Script Name", this.model.baseScriptName);
            if (EditorGUI.EndChangeCheck())
            {
                this.ExtractScriptFormatItems();
            }
        }
        
        /// <summary>
        /// ReorderableListを表示
        /// </summary>
        public void DoLayoutFormatterList()
        {
            if (this.view.scriptLists != null)
            {
                // 書式指定項目を表示
                this.view.scriptLists.ToList().ForEach(list => list.DoLayoutList());
            }
        }

        /// <summary>
        /// スクリプト名の書式指定項目を取得
        /// </summary>
        public FormatItemList[] GetFormatItems()
        {
            return this.model.formatItems;
        }

        public string[] ScriptNames()
        {
            if (this.model.formatItems == null || this.model.formatItems.Length == 0)
            {
                return new string[] { this.model.baseScriptName };
            }
            else
            {
                return CreateDP(this.model.formatItems.Select(ds => ds.FormatItems.Select(d => d.Name)).ToArray())
                .Select(strs => string.Format(this.model.baseScriptName, strs))
                .ToArray();
            }
        }

        /// <summary>
        /// スクリプト一覧のReorderableListを作成しなおす
        /// </summary>
        public void RebuildScriptList()
        {
            this.view.scriptLists = this.model.formatItems
            .Where(d => d != null)
            .Select(d => CreateScriptList(d))
            .ToArray();
        }

        /// <summary>
        /// 書式指定項目が変化したら呼ばれる
        /// </summary>
        private void OnFormatterIdChanged()
        {
            this.RebuildScriptList();
        }

        /// <summary>
        /// スクリプト情報のReorderableListを作成する
        /// </summary>
        private static ReorderableList CreateScriptList(FormatItemList formatList)
        {
            var list = new ReorderableList((IList)formatList.FormatItems, typeof(string));

            // ヘッダー描画
            list.drawHeaderCallback += (rect) =>
            {
                EditorGUI.LabelField(rect, "{" + formatList.Id + "}");
            };

            // 要素の描画
            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.y += 2;
                rect.height -= 4;

                var scriptData = (FormatItem)list.list[index];
                scriptData.Name = EditorGUI.TextField(rect, scriptData.Name);
            };

            // フッター描画
            list.drawFooterCallback = (rect) =>
            {
                if (list.count == 0)
                {
                    rect.position -= new Vector2(0f, list.elementHeight + list.headerHeight + 3f);
                }
                else
                {
                    rect.position -= new Vector2(0f, list.elementHeight * list.count + list.headerHeight + 3f);
                }
                ReorderableList.defaultBehaviours.DrawFooter(rect, list);
            };

            return list;
        }

        /// <summary>
        /// 書式指定項目を抽出
        /// </summary>
        public void ExtractScriptFormatItems()
        {
            var list = new List<FormatItemList>();
            var matches = Regex.Matches(this.model.baseScriptName, @"(?<={)+[^\{\}]*(?=})"); // {}で囲まれた部分を抽出
            var keys = new List<int>();
            for (int i = 0; i < matches.Count; i++)
            {
                int key;
                bool success = int.TryParse(matches[i].Value, out key);
                if (!success) { Debug.LogError("invalid format: {" + matches[i].Value + "}"); continue; }
                if (keys.Contains(key)) { continue; }
                keys.Add(key);
                list.Add(new FormatItemList { Id = key, FormatItems = new List<FormatItem> { new FormatItem() } });
            }

            bool isChanged = false;
            if (this.model.formatItems == null || this.model.formatItems.Length != list.Count)
            {
                isChanged = true;
            }
            else
            {
                isChanged = list
                .Select((item, i) => new {index = i, item = item} )
                .Any(x => x.item.Id != this.model.formatItems[x.index].Id);
            }

            this.model.formatItems = list.ToArray();
            if (isChanged)
            {
                this.OnFormatterIdChanged();
            }
        }

        /// <summary>
        /// string配列から直積を作成
        /// </summary>
        private static IEnumerable<string[]> CreateDP(IEnumerable<string>[] strs)
        {
            const char split = ',';
            var result = strs[0];
            if (strs.Length > 1)
            {
                for (int i = 0; i < strs.Length - 1; i++)
                {
                    var s2 = result.SelectMany(a => strs[i + 1], (a, b) => a + split + b).ToList();
                    result = s2;
                }
            }
            return result.Select(s => s.Split(split));
        }
    }
}
