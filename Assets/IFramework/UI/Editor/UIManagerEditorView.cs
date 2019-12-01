/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-03
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace IFramework
{
    [CustomEditor(typeof(UIManager))]
    internal class UIManagerEditorView : Editor
    {
        private UIManager UIMgr;

        [SerializeField] private bool ShowBGBG = true;
        [SerializeField] private bool ShowBackGround = true;
        [SerializeField] private bool ShowAnimationUnderPage = true;
        [SerializeField] private bool ShowCommon = true;
        [SerializeField] private bool ShowAnimationOnPage = true;
        [SerializeField] private bool ShowPopup = true;
        [SerializeField] private bool ShowGuide = true;
        [SerializeField] private bool ShowToast = true;
        [SerializeField] private bool ShowTop = true;
        [SerializeField] private bool ShowTopTop = true;


        [SerializeField] private bool IsStackOn = true;
        [SerializeField] private bool IsCacheOn = true;

        [SerializeField] private string SearchTxt = string.Empty;
        private const float typeWith = 200;
        private const float paneltypeWith = 100;

        private class Styles
        {
            public static GUIStyle BoldLabel = EditorStyles.boldLabel;
            public static GUIStyle toolbarButton = EditorStyles.toolbarButton;
            public static GUIStyle toolbar = EditorStyles.toolbar;
            public static GUIStyle searchField = GUI.skin.FindStyle("ToolbarSeachTextField");
            public static GUIStyle cancelBtn = GUI.skin.FindStyle("ToolbarSeachCancelButton");
            public static GUIStyle Fold =new GUIStyle( GUI.skin.FindStyle("ToolbarDropDown"));
            public static GUIStyle FoldOut = EditorStyles.foldout;

            static Styles()
            {
                Fold.fixedHeight = BoldLabel.fixedHeight;
            }
        }
        private class Contents
        {
            public static GUIContent BGBG = new GUIContent("BGBG", "BGBG");
            public static GUIContent BackGround = new GUIContent("BG", "BackGround");
            public static GUIContent AUP = new GUIContent("AUP", "AnimationUnderPage");
            public static GUIContent Common = new GUIContent("Com", "Common");
            public static GUIContent AOP = new GUIContent("AOP", "AnimationOnPage");
            public static GUIContent Popup = new GUIContent("Pop", "Popup");
            public static GUIContent Guide = new GUIContent("Guide", "Guide");
            public static GUIContent Toast = new GUIContent("Toast", "Toast");
            public static GUIContent Top = new GUIContent("Top", "Top");
            public static GUIContent TopTop = new GUIContent("TopTop", "TopTop");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (UIMgr == null) UIMgr = this.target as UIManager;
            if (UIManager.LoaderCount <= 0)
                EditorGUILayout.HelpBox("Must Have Loader ", UnityEditor.MessageType.Error);
            EditorGUILayout.LabelField("LoaderCount: " + UIManager.LoaderCount);
            GUILayout.BeginHorizontal(Styles.toolbar);
            {
                ShowBGBG = GUILayout.Toggle(ShowBGBG, Contents.BGBG, Styles.toolbarButton/*, GUILayout.Width(40)*/);
                ShowBackGround = GUILayout.Toggle(ShowBackGround, Contents.BackGround, Styles.toolbarButton/*, GUILayout.Width(80)*/);
                ShowAnimationUnderPage = GUILayout.Toggle(ShowAnimationUnderPage, Contents.AUP , Styles.toolbarButton/*, GUILayout.Width(100)*/);
                ShowCommon = GUILayout.Toggle(ShowCommon, Contents.Common, Styles.toolbarButton/*, GUILayout.Width(60)*/);
                ShowAnimationOnPage = GUILayout.Toggle(ShowAnimationOnPage, Contents.AOP, Styles.toolbarButton/*, GUILayout.Width(100)*/);
                ShowPopup = GUILayout.Toggle(ShowPopup,Contents.Popup, Styles.toolbarButton/*, GUILayout.Width(40)*/);
                ShowGuide = GUILayout.Toggle(ShowGuide, Contents.Guide, Styles.toolbarButton/*, GUILayout.Width(40)*/);
            }
            GUILayout.EndHorizontal();
           
            GUILayout.BeginHorizontal(Styles.toolbar);
            {
               
                ShowToast = GUILayout.Toggle(ShowToast, Contents.Toast , Styles.toolbarButton/*, GUILayout.Width(40)*/);
                ShowTop = GUILayout.Toggle(ShowTop, Contents.Top , Styles.toolbarButton/*, GUILayout.Width(40)*/);
                ShowTopTop = GUILayout.Toggle(ShowTopTop,Contents.TopTop, Styles.toolbarButton/*, GUILayout.Width(40)*/);
                GUILayout.FlexibleSpace();
                SearchTxt = GUILayout.TextField(SearchTxt, Styles.searchField, GUILayout.MaxWidth(300));
                if (GUILayout.Button("", Styles.cancelBtn))
                {
                    SearchTxt = "";
                    GUI.FocusControl(null);
                }
            }
            GUILayout.EndHorizontal();

          //  return;
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                GUILayout.Label("Name");

                GUILayout.Label("Type", GUILayout.MaxWidth(typeWith));
                GUILayout.Label("PanelType", GUILayout.Width(paneltypeWith));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal(Styles.Fold, GUILayout.Height(20));
                {
                    GUILayout.Space(10);
                    IsStackOn = EditorGUILayout.Foldout(IsStackOn, string.Format("Stack  Count:  {0}", UIManager.StackCount), true,Styles.FoldOut);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                if (UIMgr.UIStack.Count > 0)
                {
                    var o = UIMgr.UIStack.ToList();
                    GUI.enabled = false;
                    if (IsStackOn)
                        for (int i = UIMgr.UIStack.Count - 1; i >= 0; i--)
                        {
                            if (i == 0) GUI.enabled = true;
                            bool canshow = false;
                            switch (o[i].PanelLayer)
                            {
                                case UIPanelLayer.BGBG: canshow = ShowBGBG; break;
                                case UIPanelLayer.Background: canshow = ShowBackGround; break;
                                case UIPanelLayer.AnimationUnderPage: canshow = ShowAnimationUnderPage; break;
                                case UIPanelLayer.Common: canshow = ShowCommon; break;
                                case UIPanelLayer.AnimationOnPage: canshow = ShowAnimationOnPage; break;
                                case UIPanelLayer.PopUp: canshow = ShowPopup; break;
                                case UIPanelLayer.Guide: canshow = ShowGuide; break;
                                case UIPanelLayer.Toast: canshow = ShowToast; break;
                                case UIPanelLayer.Top: canshow = ShowTop; break;
                                case UIPanelLayer.TopTop: canshow = ShowTopTop; break;
                            }
                            canshow &= o[i].PanelName.Contains(SearchTxt);
                            if (!canshow) continue;
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Space(10);
                                Rect r= EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                                {
                                    GUILayout.Space(10);
                                    GUILayout.Label(o[i].PanelName/*, GUILayout.MaxWidth(nameWith)*/);
                                    GUILayout.Label(o[i].GetType().ToString(), GUILayout.MaxWidth(typeWith));
                                    GUILayout.Label(o[i].PanelLayer.ToString(), GUILayout.Width(paneltypeWith));
                                }
                                EditorGUILayout.EndHorizontal();
                                if (Event.current.clickCount==2 && r.Contains( Event.current.mousePosition)) Selection.activeGameObject = o[i].gameObject;
                            }
                            GUILayout.EndHorizontal();
                        }
                    GUI.enabled = true;
                }
                GUILayout.BeginHorizontal(Styles.Fold, GUILayout.Height(20)); 
                {
                    GUILayout.Space(10);
                    IsCacheOn = EditorGUILayout.Foldout(IsCacheOn, string.Format("Cache  Count:  {0}", UIManager.CacheCount), true,Styles.FoldOut);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                if (UIMgr.UICache.Count > 0)
                {
                    var o = UIMgr.UICache.ToList();
                    GUI.enabled = false;
                    if (IsCacheOn)
                        for (int i = 0; i < UIMgr.UICache.Count; i++)
                        {
                            bool canshow = false;
                            switch (o[i].PanelLayer)
                            {
                                case UIPanelLayer.BGBG: canshow = ShowBGBG; break;
                                case UIPanelLayer.Background: canshow = ShowBackGround; break;
                                case UIPanelLayer.AnimationUnderPage: canshow = ShowAnimationUnderPage; break;
                                case UIPanelLayer.Common: canshow = ShowCommon; break;
                                case UIPanelLayer.AnimationOnPage: canshow = ShowAnimationOnPage; break;
                                case UIPanelLayer.PopUp: canshow = ShowPopup; break;
                                case UIPanelLayer.Guide: canshow = ShowGuide; break;
                                case UIPanelLayer.Toast: canshow = ShowToast; break;
                                case UIPanelLayer.Top: canshow = ShowTop; break;
                                case UIPanelLayer.TopTop: canshow = ShowTopTop; break;
                            }
                            canshow &= o[i].PanelName.Contains(SearchTxt);
                            if (!canshow) continue;
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Space(10);
                                Rect r= EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                                {
                                    GUILayout.Space(10);
                                    GUILayout.Label(o[i].PanelName/*, GUILayout.MaxWidth(nameWith)*/);
                                    GUILayout.Label(o[i].GetType().ToString(), GUILayout.MaxWidth(typeWith));
                                    GUILayout.Label(o[i].PanelLayer.ToString(), GUILayout.Width(paneltypeWith));
                                }
                                EditorGUILayout.EndHorizontal();
                                if (Event.current.clickCount == 2 && r.Contains(Event.current.mousePosition)) Selection.activeGameObject = o[i].gameObject;
                            } 
                            GUILayout.EndHorizontal();

                        }
                    GUI.enabled = true;
                }
            }
            GUILayout.EndVertical();
            EditorGUILayout.Space();
            if (EditorApplication.isPlaying)
            {
                this.Repaint();
            }
            TestButton();
        }



        private void TestButton()
        {
            using (new EditorGUI.DisabledScope(!EditorApplication.isPlaying))
            {
                GUILayout.BeginHorizontal();
                using (new EditorGUI.DisabledScope(UIManager.StackCount <= 0))
                {
                    if (GUILayout.Button("GoBack", GUILayout.Height(30)))
                    {
                        UIManager.GoBack(new UIEventArgs() { isInspectorBtn = true });
                    }
                }
                using (new EditorGUI.DisabledScope(UIManager.CacheCount <= 0))
                {
                    if (GUILayout.Button("GoForWard", GUILayout.Height(30)))
                    {
                        UIManager.GoForWard(new UIEventArgs() { isInspectorBtn = true });
                    }
                }
                GUILayout.EndHorizontal();
                using (new EditorGUI.DisabledScope(UIManager.CacheCount <= 0))
                {
                    if (GUILayout.Button("ClearCache", GUILayout.Height(30)))
                    {
                        UIManager.ClearCache(new UIEventArgs() { isInspectorBtn = true });
                    }
                }
            }


        }
    }
}
