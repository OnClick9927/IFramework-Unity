/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using IFramework.Serialization;
using IFramework.GUITool;

namespace IFramework
{
    [EditorWindowCache("IFramework.Language")]
    public partial class LanWindow : EditorWindow
    {

        private class Styles
        {
            public static GUIStyle Title = "IN BigTitle";
            public static GUIStyle TitleTxt = "IN BigTitle Inner";
            public static GUIStyle BoldLabel = EditorStyles.boldLabel;
            public static GUIStyle toolbarButton = EditorStyles.toolbarButton;
            public static GUIStyle toolbar = EditorStyles.toolbar;
            public static GUIStyle Fold = new GUIStyle(GUI.skin.FindStyle("ToolbarDropDown"));
            public static GUIStyle FoldOut = EditorStyles.foldout;
            public static GUIStyle CloseBtn = "WinBtnClose";
            public static GUIStyle BG = "box";


            static Styles()
            {
                Fold.fixedHeight = BoldLabel.fixedHeight;
            }
        }
        private class Contents
        {

            public static GUIContent CreateViewTitle = new GUIContent("Create", EditorGUIUtility.IconContent("tree_icon_leaf").image);
            public static GUIContent GroupByLanViewTitle = new GUIContent("GroupByLan", EditorGUIUtility.IconContent("d_tree_icon_frond").image);
            public static GUIContent GroupByKeyViewTitle = new GUIContent("GroupByKey", EditorGUIUtility.IconContent("d_tree_icon_frond").image);
            public static GUIContent CopyBtn = new GUIContent("C", "Copy");
            public static GUIContent OK = EditorGUIUtility.IconContent("vcs_add");
            public static GUIContent Warnning = EditorGUIUtility.IconContent("console.warnicon.sml");

        }
        private const string CreateViewNmae = "CreateView";
        private const string GroupByKeyNmae = "GroupByKey";
        private const string GroupByLanNmae = "GroupByLan";
        private CreateView createView = new CreateView();
        private GroupByLanView groupByLanView = new GroupByLanView();
        private GroupByKeyView groupByKeyView = new GroupByKeyView();

        [SerializeField]
        private bool mask = true;
        private Color maskColor = new Color(0.2f, 0.2f, 0.2f, 0.2f);
        [SerializeField]
        private string tmpLayout;
        private const float ToolBarHeight = 17;
        private Rect localPosition { get { return new Rect(Vector2.zero, position.size); } }
        private SubWinTree sunwin;
        private ToolBarTree ToolBarTree;


        private abstract class LanwindowItem : ILayoutGUIDrawer, IRectGUIDrawer
        {
            public static LanWindow window;
            protected Rect position;
            
            protected float TitleHeight { get { return Styles.Title.CalcHeight(titleContent, position.width); } }
            protected float smallBtnSize = 20;
            protected float describeWidth = 30;
            protected virtual GUIContent titleContent { get; }
            public void OnGUI(Rect position)
            {
                this.position = position;
                position.DrawOutLine(2, Color.black);
                this.DrawClip(() => {
                    Rect[] rs = position.HorizontalSplit(TitleHeight);
                    this.Box(rs[0]);
                    this.Box(rs[0], titleContent, Styles.Title);
                    DrawContent(rs[1]);
                }, position);

            }
            protected abstract void DrawContent(Rect rect);


        }
    }
    public partial class LanWindow : EditorWindow
    {
        private LanGroup lanGroup;
        private List<LanPair> lanPairs { get { return lanGroup.lanPairs; } }
        private List<string> lanKeys { get { return lanGroup.Keys; } }

        private string stoPath;
        private void OnEnable()
        {
            LanwindowItem.window = this;
            stoPath = FrameworkConfig.FrameworkPath.CombinePath("Lan/Resources/LanGroup.asset");
            LoadLanGroup();
            this.titleContent = new GUIContent("Lan", EditorGUIUtility.IconContent("d_WelcomeScreen.AssetStoreLogo").image);
            SubwinInit();
        }
        private void LoadLanGroup()
        {
            if (File.Exists(stoPath))
                lanGroup = ScriptableObj.Load<LanGroup>(stoPath);
            else
                lanGroup = ScriptableObj.Create<LanGroup>(stoPath);
            Fresh();
        }
        private void UpdateLanGroup()
        {
            ScriptableObj.Update(lanGroup);
            //LanGroup = ScriptableObj.Load<LanGroup>(stoPath);
            Fresh();
        }
        private void OnDisable()
        {
            tmpLayout = sunwin.Serialize();
            UpdateLanGroup();
        }

        private void Views(Rect rect)
        {
            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < sunwin.allLeafCount; i++)
            {
                SubWinTree.TreeLeaf leaf = sunwin.allLeafs[i];
                menu.AddItem(leaf.titleContent, !sunwin.closedLeafs.Contains(leaf), () => {
                    if (sunwin.closedLeafs.Contains(leaf))
                        sunwin.DockLeaf(leaf, SubWinTree.DockType.Left);
                    else
                        sunwin.CloseLeaf(leaf);
                });
            }
            menu.DropDown(rect);
            Event.current.Use();
        }
        private void SubwinInit()
        {
            sunwin = new SubWinTree();
            sunwin.repaintEve += Repaint;
            sunwin.drawCursorEve += (rect, sp) =>
            {
                if (sp == SplitType.Vertical)
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);
                else
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeVertical);
            };
            if (string.IsNullOrEmpty(tmpLayout))
            {
                for (int i = 1; i <= 3; i++)
                {
                    string userdata = i == 1 ? "GroupByLan" : i == 2 ? "GroupByKey" : "CreateView";
                    SubWinTree.TreeLeaf L = sunwin.CreateLeaf(new GUIContent(userdata));
                    L.userData = userdata;
                    sunwin.DockLeaf(L, SubWinTree.DockType.Left);
                }
            }
            else
            {
                sunwin.DeSerialize(tmpLayout);
            }
            sunwin[GroupByKeyNmae].titleContent = new GUIContent(GroupByKeyNmae);
            sunwin[GroupByLanNmae].titleContent = new GUIContent(GroupByLanNmae);
            sunwin[CreateViewNmae].titleContent = new GUIContent(CreateViewNmae);
            sunwin[GroupByKeyNmae].minSize = new Vector2(250, 250);
            sunwin[GroupByLanNmae].minSize = new Vector2(250, 250);
            sunwin[CreateViewNmae].minSize = new Vector2(300, 300);
            sunwin[GroupByKeyNmae].paintDelegate += groupByKeyView.OnGUI;
            sunwin[GroupByLanNmae].paintDelegate += groupByLanView.OnGUI;
            sunwin[CreateViewNmae].paintDelegate += createView.OnGUI;


            ToolBarTree = new ToolBarTree();
            ToolBarTree.DropDownButton(new GUIContent("Views"), Views, 60)
                            .FlexibleSpace()
                            .Toggle(new GUIContent("mask"), m => mask = m, mask, 60)
                            .Delegate((r) => {
                                maskColor = EditorGUI.ColorField(r, maskColor);
                            }, 80)
                            .Toggle(new GUIContent("Title"), (bo) => { sunwin.isShowTitle = bo; }, sunwin.isShowTitle, 60)
                            .Toggle(new GUIContent("Lock"), (bo) => { sunwin.isLocked = bo; }, sunwin.isLocked, 60);

        }

        private void OnGUI()
        {
            var rs = localPosition.Zoom(AnchorType.MiddleCenter, -2).HorizontalSplit(ToolBarHeight, 4);
            ToolBarTree.OnGUI(rs[0]);
            sunwin.OnGUI(rs[1]);
            this.minSize = sunwin.minSize + new Vector2(0, ToolBarHeight);

            if (mask)
            {
                GUI.backgroundColor = maskColor;
                GUI.Box(localPosition, "");
            }
        }

        private Dictionary<string, List<LanPair>> keyDic;
        private Dictionary<SystemLanguage, List<LanPair>> lanDic;

        private void Fresh()
        {
            keyDic = lanPairs.GroupBy(lanPair => { return lanPair.key; }, (key, list) => { return new { key, list }; })
                             .ToDictionary((v) => { return v.key; }, (v) => { return v.list.ToList(); });
            lanDic = lanPairs.GroupBy(lanPair => { return lanPair.Lan; }, (key, list) => { return new { key, list }; })
                            .ToDictionary((v) => { return v.key; }, (v) => { return v.list.ToList(); });
        }
        private void DeletePairsByLan(SystemLanguage lan)
        {
            lanGroup.DeletePairsByLan(lan);
            UpdateLanGroup();
        }
        private void DeletePairsByKey(string key)
        {
            lanGroup.DeletePairsByKey(key);
            UpdateLanGroup();
        }
        private void DeleteLanPair(LanPair pair)
        {
            lanGroup.DeleteLanPair(pair);
            UpdateLanGroup();
        }
        private void AddLanPair(LanPair pair)
        {
            if (string.IsNullOrEmpty(pair.Value.Trim()))
            {
                ShowNotification(new GUIContent("Value Can't be Null"));
                return;
            }
            LanPair tmpPair = new LanPair()
            {
                Lan = pair.Lan,
                key = pair.key,
                Value = pair.Value
            };
            LanPair lp = lanPairs.Find((p) => { return p.Lan == tmpPair.Lan && p.key == tmpPair.key; });
            if (lp == null)
            {
                lanPairs.Add(tmpPair);
                UpdateLanGroup();
            }
            else
            {
                if (lp.Value == tmpPair.Value)
                    ShowNotification(new GUIContent("Don't Add Same"));
                else
                {
                    if (EditorUtility.DisplayDialog("Warn",
                        string.Format("Replace Old Value ?\n Old Value  {0}\n New Vlaue  {1}", lp.Value, tmpPair.Value), "Yes", "No"))
                    {
                        lp.Value = tmpPair.Value;
                    }
                }
            }
        }

        private void AddLanGroupKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                ShowNotification(new GUIContent("Err: key is Empty " + key));
                return;
            }
            if (!lanKeys.Contains(key))
            {
                lanKeys.Add(key);
                UpdateLanGroup();
            }
            else
            {
                ShowNotification(new GUIContent("Err: key Has Exist " + key));
            }
        }
        private void DeleteLanKey(string key)
        {
            if (lanKeys.Contains(key))
            {
                lanKeys.Remove(key);
                DeletePairsByKey(key);
            }
        }
        private void CleanData()
        {
            lanPairs.Clear();
            lanKeys.Clear();
            UpdateLanGroup();
        }
        private void WriteXml(string path)
        {
            path.WriteText(Xml.ToXmlString(lanPairs), Encoding.UTF8);
        }
        private void ReadXml(string path)
        {
            List<LanPair> ps = Xml.ToObject<List<LanPair>>(path.ReadText(Encoding.UTF8))
                .Distinct()
                .ToList().FindAll((p) => { return !string.IsNullOrEmpty(p.key) && !string.IsNullOrEmpty(p.Value); });
            if (ps == null || ps.Count == 0) return;
            for (int i = 0; i < ps.Count; i++)
            {
                var filePair = ps[i];
                if (!lanKeys.Contains(filePair.key)) lanKeys.Add(filePair.key);
                LanPair oldPair = lanPairs.Find((pair) => { return pair.key == filePair.key && pair.Lan == filePair.Lan; });
                if (oldPair == null) lanPairs.Add(filePair);
                else
                {
                    if (oldPair.Value != filePair.Value)
                    {
                        if (EditorUtility.DisplayDialog("Warning",
                                            "The LanPair Is Same Do You Want Replace \n"
                                            .Append(string.Format("Lan {0}\t\t Key {0}\t \n", oldPair.Lan, oldPair.key))
                                            .Append(string.Format("Old  Val\t\t {0}\n", oldPair.Value))
                                            .Append(string.Format("New  Val\t\t {0}\n", filePair.Value))
                                            , "Yes", "No"))
                        {
                            oldPair.Value = filePair.Value;
                        }
                    }
                }
            }
            Fresh();
            UpdateLanGroup();
        }
        private void WriteJson(string path)
        {
            path.WriteText(Json.ToJsonString(lanPairs), Encoding.UTF8);
        }
        private void ReadJson(string path)
        {
            List<LanPair> ps = Json.ToObject<List<LanPair>>(path.ReadText(Encoding.UTF8))
               .Distinct()
               .ToList().FindAll((p) => { return !string.IsNullOrEmpty(p.key) && !string.IsNullOrEmpty(p.Value); });
            if (ps == null || ps.Count == 0) return;
            for (int i = 0; i < ps.Count; i++)
            {
                var filePair = ps[i];
                if (!lanKeys.Contains(filePair.key)) lanKeys.Add(filePair.key);
                LanPair oldPair = lanPairs.Find((pair) => { return pair.key == filePair.key && pair.Lan == filePair.Lan; });
                if (oldPair == null) lanPairs.Add(filePair);
                else
                {
                    if (oldPair.Value != filePair.Value)
                    {
                        if (EditorUtility.DisplayDialog("Warning",
                                            "The LanPair Is Same Do You Want Replace \n"
                                            .Append(string.Format("Lan {0}\t\t Key {0}\t \n", oldPair.Lan, oldPair.key))
                                            .Append(string.Format("Old  Val\t\t {0}\n", oldPair.Value))
                                            .Append(string.Format("New  Val\t\t {0}\n", filePair.Value))
                                            , "Yes", "No"))
                        {
                            oldPair.Value = filePair.Value;
                        }
                    }
                }
            }
            Fresh();
            UpdateLanGroup();
        }
        private bool IsKeyInUse(string key)
        {
            return keyDic.ContainsKey(key);
        }
        [Serializable]
        private class CreateView : LanwindowItem
        {
            public CreateView()
            {
                searchField = new SearchFieldDrawer();
                searchField.onValueChange += (str) => {
                    keySearchStr = str;
                };
            }
            protected override GUIContent titleContent { get { return Contents.CreateViewTitle; } }
            [SerializeField] private bool toolFoldon;
            private void Tool()
            {
                Rect rect;
                this.EBeginHorizontal(out rect, Styles.Fold)
                    .Foldout(ref toolFoldon, "Tool", true);
                this.EEndHorizontal()
                    .Pan(() => {
                        if (!toolFoldon) return;
                        this.BeginHorizontal()
                                      .Button(() => {
                                          window.LoadLanGroup();
                                      }, "Fresh")
                                      .Button(() => {
                                          window.UpdateLanGroup();
                                      }, "Save")
                                      .Button(() => {
                                          window.CleanData();
                                      }, "Clear")
                                 .EndHorizontal()
                                 .BeginHorizontal()
                                     .Button(() => {
                                         string path = EditorUtility.OpenFilePanel("Xml", Application.dataPath, "xml");
                                         if (string.IsNullOrEmpty(path) || !path.EndsWith(".xml")) return;
                                         window.ReadXml(path);
                                     }, "Read Xml")
                                     .Button(() => {
                                         string path = EditorUtility.OpenFilePanel("Xml", Application.dataPath, "xml");
                                         if (string.IsNullOrEmpty(path) || !path.EndsWith(".xml")) return;
                                         window.WriteXml(path);
                                     }, "Write Xml")
                                 .EndHorizontal()
                                 .BeginHorizontal()
                                     .Button(() => {
                                         string path = EditorUtility.OpenFilePanel("Json", Application.dataPath, "json");
                                         if (string.IsNullOrEmpty(path) || !path.EndsWith(".json")) return;
                                         window.ReadJson(path);
                                     }, "Read Json")
                                     .Button(() => {
                                         string path = EditorUtility.OpenFilePanel("Json", Application.dataPath, "json");
                                         if (string.IsNullOrEmpty(path) || !path.EndsWith(".json")) return;
                                         window.WriteJson(path);
                                     }, "Write Json")
                                 .EndHorizontal();
                    });
            }
            [SerializeField] private bool creatingKeyFoldon;
            [SerializeField] private string tmpKey;
            private void CreateLanKey()
            {
                Rect rect;
                this.EBeginHorizontal(out rect, Styles.Fold)
                    .Foldout(ref creatingKeyFoldon, "Create Key", true)
                    .EEndHorizontal()
                    .Pan(() => {
                        if (!creatingKeyFoldon) return;
                        this.EBeginHorizontal(out rect, Styles.BG)
                                   .TextField(ref tmpKey)
                                   .Button(() =>
                                   {
                                       window.AddLanGroupKey(tmpKey);
                                       tmpKey = string.Empty;
                                   }, Contents.OK, GUILayout.Width(describeWidth))
                                  .EEndHorizontal();
                    });
            }

            [SerializeField] private bool createLanPairFlodon;
            [SerializeField] private LanPair tmpLanPair;
            [SerializeField] private int hashID;
            private void AddLanPairFunc()
            {
                if (window.lanKeys.Count == 0) return;
                Rect rect;
                this.EBeginHorizontal(out rect, Styles.Fold)
                    .Foldout(ref createLanPairFlodon, "Create LanPair", true)
                    .EEndHorizontal()
                    .Pan(() =>
                    {
                        if (!createLanPairFlodon) return;
                        if (tmpLanPair == null) tmpLanPair = new LanPair() { key = window.lanKeys[0] };
                        if (hashID == 0) hashID = "CreateView".GetHashCode();
                        this.DrawVertical(() =>
                        {
                            this.BeginHorizontal()
                                    .Label("Lan", GUILayout.Width(describeWidth))
                                    .Pan(() => { tmpLanPair.Lan = (SystemLanguage)EditorGUILayout.EnumPopup(tmpLanPair.Lan); })
                                .EndHorizontal()
                                .BeginHorizontal()
                                    .Label("Key", GUILayout.Width(describeWidth))
                                    .Label(tmpLanPair.key)
                                    .Label(EditorGUIUtility.IconContent("editicon.sml"), GUILayout.Width(smallBtnSize))
                                .EndHorizontal()
                                .Pan(() => {
                                    Rect pos = GUILayoutUtility.GetLastRect();
                                    int ctrlId = GUIUtility.GetControlID(hashID, FocusType.Keyboard, pos);
                                    {
                                        if (DropdownButton(ctrlId, pos, new GUIContent(string.Format("Key: {0}", tmpLanPair.key))))
                                        {
                                            int index = -1;
                                            for (int i = 0; i < window.lanKeys.Count; i++)
                                            {
                                                if (window.lanKeys[i] == tmpLanPair.key)
                                                {
                                                    index = i; break;
                                                }
                                            }
                                            SearchablePopup.Show(pos, window.lanKeys.ToArray(), index, (i, str) =>
                                            {
                                                tmpLanPair.key = str;
                                                window.Repaint();
                                            });
                                        }
                                    }
                                })
                                .BeginHorizontal()
                                    .Label("Val", GUILayout.Width(describeWidth))
                                    .TextField(ref tmpLanPair.Value)
                                    .EndHorizontal()
                                .BeginHorizontal()
                                    .FlexibleSpace()
                                    .Button(() => {
                                        //createLanPairFlodon = false;
                                        window.AddLanPair(tmpLanPair);
                                        //tmpLanPair = null;
                                    }, Contents.OK)
                                .EndHorizontal();
                        }, Styles.BG);
                    });
            }
            private bool DropdownButton(int id, Rect position, GUIContent content)
            {
                Event e = Event.current;
                switch (e.type)
                {
                    case EventType.MouseDown:
                        if (position.Contains(e.mousePosition) && e.button == 0)
                        {
                            Event.current.Use();
                            return true;
                        }
                        break;
                    case EventType.KeyDown:
                        if (GUIUtility.keyboardControl == id && e.character == '\n')
                        {
                            Event.current.Use();
                            return true;
                        }
                        break;
                    case EventType.Repaint:
                        //Styles.BoldLabel.Draw(position, content, id, false);
                        break;
                }
                return false;
            }

            [SerializeField] private bool keyFoldon;
            [SerializeField] private Vector2 scroll;
            [SerializeField] private string keySearchStr = string.Empty;
            private SearchFieldDrawer searchField;
            private void LanGroupKeysView()
            {
                this.DrawHorizontal(() => {
                    this.Foldout(ref keyFoldon, string.Format("Keys  Count: {0}", window.lanKeys.Count), true);
                    this.Label("");
                  searchField.OnGUI(GUILayoutUtility.GetLastRect());
                }, Styles.Fold);
                if (keyFoldon)
                {
                    this.DrawScrollView(() => {
                        window.lanKeys.ForEach((key) => {
                            if (key.ToLower().Contains(keySearchStr.ToLower()))
                            {
                                this.BeginHorizontal(Styles.BG)
                                    .Label(key)
                                    .Label(window.IsKeyInUse(key) ? GUIContent.none : Contents.Warnning, GUILayout.Width(smallBtnSize))
                                    .Button(() => {  { GUIUtility.systemCopyBuffer = key; ; } }, Contents.CopyBtn, GUILayout.Width(smallBtnSize))
                                    .Button(() => {  { window.DeleteLanKey(key); } }, string.Empty, Styles.CloseBtn, GUILayout.Width(smallBtnSize), GUILayout.Height(smallBtnSize))
                                    .EndHorizontal();
                            }
                        });
                    }, ref scroll);
                }
            }

            protected override void DrawContent(Rect rect)
            {
                this
                    .Pan(() =>
                    {
                        this.BeginArea(rect.Zoom(AnchorType.MiddleCenter, -10))
                            .Pan(Tool)
                            .Space(5)
                            .Pan(AddLanPairFunc)
                            .Space(5)
                            .Pan(CreateLanKey)
                            .Space(5)
                            .Pan(LanGroupKeysView)
                            .EndArea();
                    });
            }

            private class SearchablePopup : PopupWindowContent
            {
                public class InnerSearchField : FocusAbleGUIDrawer
                {
                    private class Styles
                    {
                        public static GUIStyle SearchTextFieldStyle = new GUIStyle("ToolbarSeachTextField");
                        public static GUIStyle SearchCancelButtonStyle = new GUIStyle("SearchCancelButton");
                        public static GUIStyle SearchCancelButtonEmptyStyle = new GUIStyle("SearchCancelButtonEmpty");
                    }

                    public string OnGUI(Rect position, string value)
                    {
                        GUIStyle cancelBtnStyle = string.IsNullOrEmpty(value) ? Styles.SearchCancelButtonEmptyStyle : Styles.SearchCancelButtonStyle;
                        position.width -= cancelBtnStyle.fixedWidth;

                        Styles.SearchTextFieldStyle.fixedHeight = position.height;
                        cancelBtnStyle.fixedHeight = position.height;

                        Styles.SearchTextFieldStyle.alignment = TextAnchor.MiddleLeft;
                        while (Styles.SearchTextFieldStyle.lineHeight < position.height - 15)
                        {
                            Styles.SearchTextFieldStyle.fontSize++;
                        }
                        GUI.SetNextControlName(FocusID);

                        value = GUI.TextField(new Rect(position.x,
                                                                         position.y + 1,
                                                                         position.width,
                                                                         position.height - 1),
                                                                         value,
                                                                         Styles.SearchTextFieldStyle);
                        if (GUI.Button(new Rect(position.x + position.width,
                                                position.y + 1,
                                                cancelBtnStyle.fixedWidth,
                                                cancelBtnStyle.fixedHeight
                                                ),
                                                GUIContent.none,
                                                cancelBtnStyle))
                        {
                            value = string.Empty;
                            GUI.changed = true;
                            GUIUtility.keyboardControl = 0;
                        }


                        Event e = Event.current;
                        if (position.Contains(e.mousePosition))
                        {
                            if (!Focused)
                                if ((e.type == EventType.MouseDown /*&& e.clickCount == 2*/) /*|| e.keyCode == KeyCode.F2*/)
                                {
                                    Focused = true;
                                    GUIFocusControl.Focus(this);
                                    if (e.type != EventType.Repaint && e.type != EventType.Layout)
                                        Event.current.Use();
                                }
                        }
                        //if ((/*e.keyCode == KeyCode.Return ||*/ e.keyCode == KeyCode.Escape || e.character == '\n'))
                        //{
                        //    GUIFocusControl.Focus(null, Focused);
                        //    Focused = false;
                        //    if (e.type != EventType.Repaint && e.type != EventType.Layout)
                        //        Event.current.Use();
                        //}
                        return value;
                    }

                }
                private const float rowHeight = 16.0f;
                private const float rowIndent = 8.0f;
                public static void Show(Rect activatorRect, string[] options, int current, Action<int, string> onSelectionMade)
                {
                    SearchablePopup win = new SearchablePopup(options, current, onSelectionMade);
                    PopupWindow.Show(activatorRect, win);
                }
                private static void Repaint() { EditorWindow.focusedWindow.Repaint(); }
                private static void DrawBox(Rect rect, Color tint)
                {
                    Color c = GUI.color;
                    GUI.color = tint;
                    GUI.Box(rect, "", Selection);
                    GUI.color = c;
                }
                private class FilteredList
                {
                    public struct Entry
                    {
                        public int Index;
                        public string Text;
                    }
                    private readonly string[] allItems;
                    public FilteredList(string[] items)
                    {
                        allItems = items;
                        Entries = new List<Entry>();
                        UpdateFilter("");
                    }
                    public string Filter { get; private set; }
                    public List<Entry> Entries { get; private set; }
                    public int Count { get { return allItems.Length; } }
                    public bool UpdateFilter(string filter)
                    {
                        if (Filter == filter)
                            return false;
                        Filter = filter;
                        Entries.Clear();
                        for (int i = 0; i < allItems.Length; i++)
                        {
                            if (string.IsNullOrEmpty(Filter) || allItems[i].ToLower().Contains(Filter.ToLower()))
                            {
                                Entry entry = new Entry
                                {
                                    Index = i,
                                    Text = allItems[i]
                                };
                                if (string.Equals(allItems[i], Filter, StringComparison.CurrentCultureIgnoreCase))
                                    Entries.Insert(0, entry);
                                else
                                    Entries.Add(entry);
                            }
                        }
                        return true;
                    }
                }

                private readonly Action<int, string> onSelectionMade;
                private readonly int currentIndex;
                private readonly FilteredList list;
                private Vector2 scroll;
                private int hoverIndex;
                private int scrollToIndex;
                private float scrollOffset;
                private static GUIStyle Selection = "SelectionRect";

                private SearchablePopup(string[] names, int currentIndex, Action<int, string> onSelectionMade)
                {
                    list = new FilteredList(names);
                    this.currentIndex = currentIndex;
                    this.onSelectionMade = onSelectionMade;
                    hoverIndex = currentIndex;
                    scrollToIndex = currentIndex;
                    scrollOffset = GetWindowSize().y - rowHeight * 2;
                }

                public override void OnOpen()
                {
                    base.OnOpen();
                    EditorApplication.update += Repaint;
                }
                public override void OnClose()
                {
                    base.OnClose();
                    EditorApplication.update -= Repaint;
                }

                public override Vector2 GetWindowSize()
                {
                    return new Vector2(base.GetWindowSize().x,
                        Mathf.Min(600, list.Count * rowHeight + EditorStyles.toolbar.fixedHeight));
                }

                public override void OnGUI(Rect rect)
                {
                    Rect searchRect = new Rect(0, 0, rect.width, EditorStyles.toolbar.fixedHeight);
                    Rect scrollRect = Rect.MinMaxRect(0, searchRect.yMax, rect.xMax, rect.yMax);

                    HandleKeyboard();
                    DrawSearch(searchRect);
                    DrawSelectionArea(scrollRect);
                }
                private InnerSearchField searchField = new InnerSearchField();
                private void DrawSearch(Rect rect)
                {
                    GUI.Label(rect, "", EditorStyles.toolbar);
                    if (list.UpdateFilter(searchField.OnGUI(rect.Zoom(AnchorType.MiddleCenter, -2), list.Filter)))
                    {
                        hoverIndex = 0;
                        scroll = Vector2.zero;
                    }
                }

                private void DrawSelectionArea(Rect scrollRect)
                {
                    Rect contentRect = new Rect(0, 0,
                        scrollRect.width - GUI.skin.verticalScrollbar.fixedWidth,
                        list.Entries.Count * rowHeight);

                    scroll = GUI.BeginScrollView(scrollRect, scroll, contentRect);

                    Rect rowRect = new Rect(0, 0, scrollRect.width, rowHeight);

                    for (int i = 0; i < list.Entries.Count; i++)
                    {
                        if (scrollToIndex == i &&
                            (Event.current.type == EventType.Repaint
                             || Event.current.type == EventType.Layout))
                        {
                            Rect r = new Rect(rowRect);
                            r.y += scrollOffset;
                            GUI.ScrollTo(r);
                            scrollToIndex = -1;
                            scroll.x = 0;
                        }

                        if (rowRect.Contains(Event.current.mousePosition))
                        {
                            if (Event.current.type == EventType.MouseMove ||
                                Event.current.type == EventType.ScrollWheel)
                                hoverIndex = i;
                            if (Event.current.type == EventType.MouseDown)
                            {
                                onSelectionMade(list.Entries[i].Index, list.Entries[i].Text);
                                EditorWindow.focusedWindow.Close();
                            }
                        }

                        DrawRow(rowRect, i);

                        rowRect.y = rowRect.yMax;
                    }

                    GUI.EndScrollView();
                }

                private void DrawRow(Rect rowRect, int i)
                {
                    if (list.Entries[i].Index == currentIndex)
                        DrawBox(rowRect, Color.cyan);
                    else if (i == hoverIndex)
                        DrawBox(rowRect, Color.white);
                    Rect labelRect = new Rect(rowRect);
                    labelRect.xMin += rowIndent;
                    GUI.Label(labelRect, list.Entries[i].Text);
                }
                private void HandleKeyboard()
                {
                    Event e = Event.current;
                    if (e.type == EventType.KeyDown)
                    {
                        if (e.keyCode == KeyCode.DownArrow)
                        {
                            hoverIndex = Mathf.Min(list.Entries.Count - 1, hoverIndex + 1);
                            Event.current.Use();
                            scrollToIndex = hoverIndex;
                            scrollOffset = rowHeight;
                        }
                        if (e.keyCode == KeyCode.UpArrow)
                        {
                            hoverIndex = Mathf.Max(0, hoverIndex - 1);
                            Event.current.Use();
                            scrollToIndex = hoverIndex;
                            scrollOffset = -rowHeight;
                        }
                        if (e.keyCode == KeyCode.Return || e.character == '\n')
                        {
                            if (hoverIndex >= 0 && hoverIndex < list.Entries.Count)
                            {
                                onSelectionMade(list.Entries[hoverIndex].Index, list.Entries[hoverIndex].Text);
                                EditorWindow.focusedWindow.Close();
                            }
                        }
                        if (e.keyCode == KeyCode.Escape)
                        {
                            EditorWindow.focusedWindow.Close();
                        }
                    }
                }
            }
        }
        [Serializable]
        private class GroupByLanView : LanwindowItem
        {
            protected override GUIContent titleContent { get { return Contents.GroupByLanViewTitle; } }
            public GroupByLanView()
            {
                SearchField = new SearchFieldDrawer();
                SearchField.onValueChange += (str) =>
                {
                    searchStr = str;

                };
            }
            [SerializeField] private Vector2 scroll;

            private SearchFieldDrawer SearchField;
            [SerializeField] private string searchStr = string.Empty;

            private void Calc()
            {
                while (items.Count < window.lanDic.Count) items.Add(new GroupByLanViewItem());
                while (items.Count > window.lanDic.Count) items.RemoveAt(items.Count - 1);
                int index = 0;
                foreach (var item in window.lanDic)
                {
                    items[index].Lan = item.Key;
                    items[index].lanPairs = item.Value;
                    index++;
                }
            }

            protected override void DrawContent(Rect rect)
            {
                Calc();
                this.DrawArea(() => {
                    this.Label("");
                    SearchField.OnGUI(GUILayoutUtility.GetLastRect());
                    this.Space(5);
                    this.DrawScrollView(() => {
                        items.ForEach((item) => {
                            if (item.Lan.ToString().ToLower().Contains(searchStr.ToLower()))
                            {
                                item.OnGUI(Rect.zero);
                                this.Space(3);
                            }
                        });
                    }, ref scroll);
                }, rect.Zoom(AnchorType.MiddleCenter, -20));
            }

            [SerializeField]
            private List<GroupByLanViewItem> items = new List<GroupByLanViewItem>();
            [Serializable]
            private class GroupByLanViewItem : ILayoutGUIDrawer
            {
                private float smallBtnSize = 20;

                public SystemLanguage Lan;
                public List<LanPair> lanPairs;
                [SerializeField] private bool foldon;
                public void OnGUI(Rect position)
                {
                    Rect rect;
                    this.DrawHorizontal(() => {
                        this.Label(string.Format("{0}", Lan.ToString(), lanPairs.Count), GUILayout.Width(100));
                        this.EBeginHorizontal(out rect, Styles.Fold, GUILayout.Height(smallBtnSize))
                                .Foldout(ref foldon, lanPairs.Count.ToString(), true)
                                .Button(() => { window.DeletePairsByLan(Lan); }, string.Empty, Styles.CloseBtn, GUILayout.Width(smallBtnSize), GUILayout.Height(smallBtnSize));
                        this.EEndHorizontal();

                    });

                    if (foldon)
                    {
                        this.DrawScrollView(() => {

                            lanPairs.ForEach((pair) => {
                                this.DrawHorizontal(() => {
                                    this.Space(110);
                                    this.DrawVertical(() => {
                                        this.BeginHorizontal()
                                                .FlexibleSpace()
                                                .Button(() => {  window.DeleteLanPair(pair); }, string.Empty, Styles.CloseBtn, GUILayout.Width(smallBtnSize), GUILayout.Height(smallBtnSize))
                                            .EndHorizontal()
                                            .BeginHorizontal()
                                                .Label(new GUIContent(string.Format("Key  \t{0}", pair.key), pair.key))
                                                .Button(() => {  GUIUtility.systemCopyBuffer = pair.key; }, Contents.CopyBtn, GUILayout.Width(smallBtnSize))
                                            .EndHorizontal()
                                            .BeginHorizontal()
                                                .Label(new GUIContent(string.Format("Val   \t{0}", pair.Value), pair.Value))
                                                .Button(() => {  GUIUtility.systemCopyBuffer = pair.Value; }, Contents.CopyBtn, GUILayout.Width(smallBtnSize))
                                            .EndHorizontal();
                                    }, Styles.BG);
                                });

                            });

                        }, ref scroll);

                    }
                }
                [SerializeField] private Vector2 scroll;

            }
        }
        [Serializable]
        private class GroupByKeyView : LanwindowItem
        {
            protected override GUIContent titleContent { get { return Contents.GroupByKeyViewTitle; } }
            [SerializeField] private Vector2 scroll;
            public GroupByKeyView()
            {
                SearchField = new SearchFieldDrawer();
                SearchField.onValueChange += (str) =>
                {
                    searchStr = str;
                };
            }
            private SearchFieldDrawer SearchField;
            [SerializeField] private string searchStr = string.Empty;
            private void Calc()
            {
                while (items.Count < window.keyDic.Count) items.Add(new GroupByKeyViewItem());
                while (items.Count > window.keyDic.Count) items.RemoveAt(items.Count - 1);
                int index = 0;
                foreach (var item in window.keyDic)
                {
                    items[index].lanPairs = item.Value;
                    items[index].key = item.Key;
                    ++index;
                }
            }

            protected override void DrawContent(Rect rect)
            {
                Calc();
                this.DrawArea(() => {
                    this.Label("");
                    SearchField.OnGUI(GUILayoutUtility.GetLastRect());
                    this.Space(5);
                    this.DrawScrollView(() => {
                        items.ForEach((item) => {
                            if (item.key.ToLower().Contains(searchStr.ToLower()))
                            {
                                item.OnGUI(Rect.zero);
                                this.Space(3);
                            }
                        });
                    }, ref scroll);
                }, rect.Zoom(AnchorType.MiddleCenter, -20));
            }

            [SerializeField]
            private List<GroupByKeyViewItem> items = new List<GroupByKeyViewItem>();

            [Serializable]
            private class GroupByKeyViewItem : ILayoutGUIDrawer
            {
                private float smallBtnSize = 20;
                private float describeWidth = 60;

                public string key;
                public List<LanPair> lanPairs;
                [SerializeField] private bool foldon;
                public void OnGUI(Rect position)
                {
                    this.DrawHorizontal(() => {
                        this.Label(new GUIContent(key, "Key"), GUILayout.Width(100));
                        Rect rect;
                        this.EBeginHorizontal(out rect, Styles.Fold, GUILayout.Height(smallBtnSize))
                            .Foldout(ref foldon, string.Format("{1}", key, lanPairs.Count), true)
                            .Button(() => {  GUIUtility.systemCopyBuffer = key; }, Contents.CopyBtn, GUILayout.Width(smallBtnSize), GUILayout.Height(16))
                            .Button(() => {  window.DeletePairsByKey(key); }, string.Empty, Styles.CloseBtn, GUILayout.Width(smallBtnSize), GUILayout.Height(smallBtnSize));
                        this.EEndHorizontal();
                    });
                    if (foldon)
                    {
                        this.DrawScrollView(() => {
                            lanPairs.ForEach((pair) => {
                                this.DrawHorizontal(() => {
                                    this.Space(120);
                                    this.DrawVertical(() => {
                                        this.BeginHorizontal()
                                               .FlexibleSpace()
                                               .Button(() => {window.DeleteLanPair(pair); }, string.Empty, Styles.CloseBtn, GUILayout.Width(smallBtnSize), GUILayout.Height(smallBtnSize))
                                            .EndHorizontal()
                                            .BeginHorizontal()
                                                .Label("Lan", GUILayout.Width(describeWidth));
                                        GUI.enabled = false;
                                        EditorGUILayout.EnumPopup(pair.Lan);
                                        GUI.enabled = true;
                                        this.EndHorizontal()
                                            .BeginHorizontal()
                                             .Label("Lan", GUILayout.Width(describeWidth))
                                             .Label(new GUIContent(pair.Value, pair.Value))
                                             .Button(() => {  GUIUtility.systemCopyBuffer = pair.Value; }, Contents.CopyBtn, GUILayout.Width(smallBtnSize))
                                         .EndHorizontal();
                                    }, Styles.BG);
                                });

                            });

                        }, ref scroll);
                    }
                }
                [SerializeField] private Vector2 scroll;
            }
        }
    }
}