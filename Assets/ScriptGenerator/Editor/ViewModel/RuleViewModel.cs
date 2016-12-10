///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;

    [SerializableAttribute]
    public class RuleViewModel : ViewModelBase
    {
        private RuleModel Model { get { return base.ModelEntity.RuleModel; } }
        [SerializeField]
        private RuleView view = new RuleView();

        /// <summary>
        /// ウィンドウ生成時に呼ばれる
        /// </summary>
        public void OnCreateWindow()
        {
            this.ReloadRules();
            this.InitializeRuleSelection();
            this.RebuildRuleSelectionList();
        }

        /// <summary>
        /// ロード時に呼ばれる
        /// </summary>
        public void OnReload()
        {
            this.ReloadRules();
            if (this.Model.GetSelectionCount() == 0)
            {
                this.InitializeRuleSelection();
            }
            this.RebuildRuleSelectionList();
        }


        /// <summary>
        /// ルール再読み込み
        /// </summary>
        public void ReloadRules()
        {
            this.Model.Rules = RuleLoader.LoadRules();

            for (int i = 0; i < this.Model.GetSelectionCount(); i++)
            {
                this.ReloadRulesByIndex(i);
            }
        }

        /// <summary>
        /// ReorderableList上でフォーカスしている項目のルールを次へ切り替える
        /// </summary>
        public void NextRule()
        {
            if (!this.view.ruleSelectionList.HasKeyboardControl()) { return; }
            
            var selectionIndex = this.view.ruleSelectionList.index;
            var selection = this.Model.GetSelection(selectionIndex);
            if (selection.RuleIndex < this.Model.Rules.Length - 1) 
            {
                this.Model.SetSelection(selectionIndex, selection.RuleIndex + 1);
            } 
        }
        
        /// <summary>
        /// ReorderableList上でフォーカスしている項目のルールを手前へ切り替える
        /// </summary>
        public void PreviousRule()
        {
            if (!this.view.ruleSelectionList.HasKeyboardControl()) { return; }

            var selectionIndex = this.view.ruleSelectionList.index;
            var selection = this.Model.GetSelection(selectionIndex);
            if (selection.RuleIndex > 0) 
            {
                this.Model.SetSelection(selectionIndex, selection.RuleIndex - 1);
            }
        }

        /// <summary>
        /// ルール選択まわりの初期化
        /// </summary>
        private void InitializeRuleSelection()
        {
            var defaultRuleIndex = this.GetDefaultRuleIndex();
            var defaultRule = this.Model.Rules[defaultRuleIndex];
            this.Model.SetSelection(new List<RuleSelectionData>() { new RuleSelectionData(defaultRuleIndex, defaultRule) });
        }

        /// <summary>
        /// ルールのリロード
        /// </summary>
        private void ReloadRulesByIndex(int selectionIndex)
        {
            // 選択していたルールが見つからなかった場合は選択状態をリセット
            if (!this.Model.Rules.Any(rule => rule.name == this.Model.GetSelectedRule(selectionIndex).name))
            {
                for (int i = 0; i < this.Model.GetSelectionCount(); i++)
                {
                    this.ResetSelection(i);
                }
            }
        }

        /// <summary>
        /// ルール選択のGUI表示
        /// </summary>
        public void ShowRuleGUI()
        {
            var reorderableList = this.view.ruleSelectionList;

            var elementsHeight = 0f;
            if (reorderableList.count < 1)
            {
                elementsHeight = reorderableList.elementHeight;
            }
            else
            {
                elementsHeight = reorderableList.count * reorderableList.elementHeight;
            }

            var height = reorderableList.headerHeight + elementsHeight + reorderableList.footerHeight;
            var rect = GUILayoutUtility.GetRect(0f, height);
            rect.x += 20f;
            rect.width -= 24f;

            // ルール選択GUIの表示
            this.view.ruleSelectionList.DoList(rect);
        }

        /// <summary>
        /// 現在選択中のルールを取得
        /// </summary>
        public RuleEntity GetSelectedRule(int selectionIndex)
        {
            return this.Model.GetSelectedRule(selectionIndex);
        }

        /// <summary>
        /// スクリプト名サフィックスを取得
        /// </summary>
        public string GetScriptNameSuffix(int selectionIndex)
        {
            return this.Model.GetSelection(selectionIndex).ScriptNameSuffix;
        }

        /// <summary>
        /// デフォルトでのルールのインデックス
        /// </summary>
        public int GetDefaultRuleIndex()
        {
            int defaultRuleIndex = 0;
            var config = ConfigLoader.LoadConfig();
            if (config != null && config.DefaultRule != null)
            {
                var rules = this.Model.Rules;
                var defaultRule = rules
                .Select((r, i) => new { Rule = r, Index = i })
                .FirstOrDefault(item => item.Rule == config.DefaultRule);

                if (defaultRule != null)
                {
                    defaultRuleIndex = defaultRule.Index;
                }
            }
            return defaultRuleIndex;
        }

        /// <summary>
        /// 選択のリセット
        /// </summary>
        private void ResetSelection(int selectionIndex)
        {
            var defaultRuleIndex = this.GetDefaultRuleIndex();
            var defaultRule = this.Model.Rules[defaultRuleIndex];
            this.Model.GetSelection(selectionIndex).Initialize(defaultRuleIndex, defaultRule);
        }

        /// <summary>
        /// ルール選択GUI用のReorderableListを作成
        /// </summary>
        private void RebuildRuleSelectionList()
        {
            this.view.ruleSelectionList = this.CreateRuleSelectionList();
        }

        /// <summary>
        /// ReorderableListの作成
        /// </summary>
        private ReorderableList CreateRuleSelectionList()
        {
            var list = new ReorderableList(this.Model.GetSelections(), typeof(int));

            // ヘッダーの描画
            var headerRect = default(Rect);
            list.drawHeaderCallback = (rect) =>
            {
                headerRect = rect;
                EditorGUI.LabelField(rect, "設定");
            };

            // フッターの描画
            list.drawFooterCallback = (rect) =>
            {
                rect.y = headerRect.y + 3;
                ReorderableList.defaultBehaviours.DrawFooter(rect, list);
            };

            list.onAddCallback = (l) =>
            {
                ReorderableList.defaultBehaviours.DoAddButton(list);
                
                var ruleIndex = this.Model.GetSelection(l.index - 1).RuleIndex;
                var rule = this.Model.Rules[ruleIndex];
                this.Model.GetSelection(l.index).Initialize(ruleIndex, rule);
            };

            list.onCanRemoveCallback = (l) =>
            {
                return l.count > 1;
            };

            // 要素の描画
            float elementSpace = 1f;
            list.elementHeight *= 2f;
            list.elementHeight += elementSpace;
            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.height -= elementSpace;

                var rectU = new Rect(rect);
                rectU.height /= 2f;
                rectU.y += 3f;
                rectU.height -= 6f;

                var rectD = new Rect(rectU);
                rectD.y += rect.height / 2f;

                // 上 ////////////////////////////////////////////////
                int buttonWidth = 47;
                int labelWidth = 140;
                var labelRect = new Rect(rectU);
                labelRect.width = labelWidth;
                EditorGUI.LabelField(labelRect, "スクリプト 生成ルール");

                var popupRect = new Rect(rectU);
                popupRect.width = rectU.width - labelRect.width - buttonWidth;
                popupRect.x = labelRect.x + labelRect.width - 4f;

                // Selectボタン
                var buttonRect = new Rect(rectU);
                buttonRect.x = popupRect.x + popupRect.width + 4f;
                buttonRect.width = buttonWidth;
                if (GUI.Button(buttonRect, "Select"))
                {
                    EditorGUIUtility.PingObject(this.Model.GetSelectedRule(index));
                }

                var ruleNames = this.Model.Rules
                .Select(rule => 
                {
                    if (string.IsNullOrEmpty(rule.Category))
                    {
                        return rule.name;
                    }
                    else
                    {
                        return rule.Category + "/" + rule.name;
                    }
                })
                .ToArray();
                
                EditorGUI.BeginChangeCheck();
                // ポップアップ
                var ruleIndex = EditorGUI.Popup(popupRect, this.Model.GetSelection(index).RuleIndex, ruleNames);
                if (EditorGUI.EndChangeCheck())
                {
                    this.Model.SetSelection(index, ruleIndex);
                }

                // 下 ////////////////////////////////////////////////
                labelRect.y += rect.height / 2f;
                EditorGUI.LabelField(labelRect, "スクリプト名 Suffix");

                var textRect = new Rect(rectD);
                textRect.x += labelRect.width - 4;
                textRect.width -= labelRect.width - 4;

                this.Model.GetSelection(index).ScriptNameSuffix = EditorGUI.TextField(textRect, this.Model.GetSelection(index).ScriptNameSuffix);

                //////////////////////////////////////////////////////

            };

            return list;
        }
    }
}
