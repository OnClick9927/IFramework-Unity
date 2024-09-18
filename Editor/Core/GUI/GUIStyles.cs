/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1.885
 *UnityVersion:   2018.4.17f1
 *Date:           2020-06-25
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace IFramework
{
    public static class GUIStyles
    {
        private static Dictionary<string, GUIStyle> styles;

        public static GUIStyle Get(string name)
        {
            GUIStyle style;
            if (styles == null)
            {
                styles = new Dictionary<string, GUIStyle>();
            }
            if (!styles.TryGetValue(name, out style))
            {
                style = new GUIStyle(name);
                styles.Add(name, style);
            }
            return style;
        }

        public static GUIStyle Get(GUIStyle style)
        {
            GUIStyle _style;
            if (styles == null)
            {
                styles = new Dictionary<string, GUIStyle>();
            }
            if (!styles.TryGetValue(style.name, out _style))
            {
                _style = new GUIStyle(style);
                styles.Add(style.name, _style);
            }
            return _style;
        }
        public static GUIStyle PreDropDown = Get("PreDropDown");
        public static GUIStyle IN_title = Get("IN Title");

        public static GUIStyle titleStyle = Get("IN BigTitle");
        public static GUIStyle minus = Get("OL Minus");
        public static GUIStyle plus = Get("OL Plus");
        public static GUIStyle BoldLabel = Get("BoldLabel");
        public static GUIStyle entryBackodd = Get("CN EntryBackodd");
        public static GUIStyle entryBackEven = Get("CN EntryBackEven");

        public static GUIStyle header = new GUIStyle("BoldLabel")
        {
            fontSize = 20
        };

        public static GUIStyle toolbarbutton = Get("toolbarbutton");

        public static GUIStyle ToolbarDropDown = Get("ToolbarDropDown");

        public static GUIStyle ToolBar = Get("ToolBar");

        public static GUIStyle Tooltip = Get("tooltip");

        public static GUIStyle dragtabdropwindow= Get("dragtabdropwindow");
        public static GUIStyle WinBtnClose = Get("WinBtnClose");
        public static GUIStyle IN_BigTitle_Inner= Get("IN BigTitle Inner");
        public static GUIStyle SelectRect = Get("SelectionRect");
        public static GUIStyle DockArea = Get("LODBlackBox");
        public static GUIStyle scrollView= Get("scrollView");
        public static GUIStyle verticalScrollbarDownButton = Get("verticalScrollbarDownButton");
        public static GUIStyle verticalScrollbarUpButton = Get("verticalScrollbarUpButton");
        public static GUIStyle verticalScrollbarThumb = Get("verticalScrollbarThumb");
        public static GUIStyle verticalScrollbar =Get("verticalScrollbar");
        public static GUIStyle horizontalScrollbarRightButton = Get("horizontalScrollbarRightButton");
        public static GUIStyle horizontalScrollbarLeftButton = Get("horizontalScrollbarLeftButton");
        public static GUIStyle horizontalScrollbarThumb = Get("horizontalScrollbarThumb");
        public static GUIStyle horizontalScrollbar = Get("horizontalScrollbar");
        public static GUIStyle verticalSliderThumb = Get("verticalSliderThumb");
        public static GUIStyle verticalSlider = Get("verticalSlider");
        public static GUIStyle horizontalSliderThumb = Get("horizontalSliderThumb");
        public static GUIStyle window = Get("window");
        public static GUIStyle button = Get("button");
        public static GUIStyle label= Get("label");
        public static GUIStyle label_rich = new GUIStyle("label") { richText = true };

        public static GUIStyle box = Get("box");
        public static GUIStyle horizontalSlider = Get("horizontalSlider");
        public static GUIStyle centeredGreyMiniLabel = Get(EditorStyles.centeredGreyMiniLabel);
        public static GUIStyle colorField = Get(EditorStyles.colorField);
        public static GUIStyle foldout = Get(EditorStyles.foldout);
        public static GUIStyle foldoutPreDrop= Get(EditorStyles.foldoutPreDrop);
        public static GUIStyle helpBox= Get(EditorStyles.helpBox);
        public static GUIStyle helpBox_bold12 = new GUIStyle(GUIStyles.helpBox)
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
        };
        public static GUIStyle inspectorDefaultMargins = Get(EditorStyles.inspectorDefaultMargins);
        public static GUIStyle inspectorFullWidthMargins = Get(EditorStyles.inspectorFullWidthMargins);
        public static GUIStyle largeLabel = Get(EditorStyles.largeLabel);
        public static GUIStyle layerMaskField = Get(EditorStyles.layerMaskField);
        public static GUIStyle miniBoldLabel = Get(EditorStyles.miniBoldLabel);
        public static GUIStyle miniButton = Get(EditorStyles.miniButton);
        public static GUIStyle miniButtonLeft = Get(EditorStyles.miniButtonLeft);
        public static GUIStyle miniButtonMid = Get(EditorStyles.miniButtonMid);
        public static GUIStyle miniButtonRight= Get(EditorStyles.miniButtonRight);
        public static GUIStyle miniLabel = Get(EditorStyles.miniLabel);
        public static GUIStyle miniPullDown = Get(EditorStyles.miniPullDown);
        public static GUIStyle miniTextField = Get(EditorStyles.miniTextField);
        public static GUIStyle numberField= Get(EditorStyles.numberField);
        public static GUIStyle objectField = Get(EditorStyles.objectField);
        public static GUIStyle objectFieldMiniThumb = Get(EditorStyles.objectFieldMiniThumb);
        public static GUIStyle objectFieldThumb = Get(EditorStyles.objectFieldThumb);
        public static GUIStyle popup = Get(EditorStyles.popup);
        public static GUIStyle radioButton= Get(EditorStyles.radioButton);
        public static GUIStyle textArea = Get(EditorStyles.textArea);
        public static GUIStyle textArea_rich = new GUIStyle(EditorStyles.textArea) { richText = true };

        public static GUIStyle textField = Get(EditorStyles.textField);
        public static GUIStyle toggle = Get(EditorStyles.toggle);
        public static GUIStyle toggleGroup = Get(EditorStyles.toggleGroup);
        public static GUIStyle toolbar= Get(EditorStyles.toolbar);
        public static GUIStyle toolbarButton = Get(EditorStyles.toolbarButton);
        public static GUIStyle toolbarDropDown = Get(EditorStyles.toolbarDropDown);
        public static GUIStyle toolbarPopup = Get(EditorStyles.toolbarPopup);
        public static GUIStyle toolbarTextField = Get(EditorStyles.toolbarTextField);
        public static GUIStyle whiteBoldLabel = Get(EditorStyles.whiteBoldLabel);
        public static GUIStyle whiteLabel= Get(EditorStyles.whiteLabel);
        public static GUIStyle whiteLargeLabel= Get(EditorStyles.whiteLargeLabel);
        public static GUIStyle whiteMiniLabel = Get(EditorStyles.whiteMiniLabel);
        public static GUIStyle wordWrappedLabel = Get(EditorStyles.wordWrappedLabel);
        public static GUIStyle wordWrappedMiniLabel = Get(EditorStyles.wordWrappedMiniLabel);
        public static GUIStyle SelectionRect = Get("SelectionRect");
    }
}
