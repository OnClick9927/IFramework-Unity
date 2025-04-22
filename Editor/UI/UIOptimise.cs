/*********************************************************************************
 *Author:         OnClick
 *Date:           2025-04-03
*********************************************************************************/
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace IFramework.UI
{
    class UIOptimize
    {
        [MenuItem("CONTEXT/Text/Remove Component")]
        [MenuItem("CONTEXT/Image/Remove Component")]
        [MenuItem("CONTEXT/RawImage/Remove Component")]
        [MenuItem("CONTEXT/Empty4Raycast/Remove Component")]
        [MenuItem("CONTEXT/PolygonRaycastImage/Remove Component")]

        static void RemnoveImage(MenuCommand cmd)
        {
            var context = cmd.context;
            var renderer = (context as MonoBehaviour).GetComponent<CanvasRenderer>();
            GameObject.DestroyImmediate(context);
            GameObject.DestroyImmediate(renderer);
        }

        [OnAddComponent(typeof(Text))]
        static void Text(Text txt)
        {
            txt.raycastTarget = false;
            txt.supportRichText = false;
            txt.resizeTextForBestFit = false;
        }
        [OnAddComponent(typeof(Image))]
        static void Image(Image img)
        {
            img.raycastTarget = false;
        }
        [OnAddComponent(typeof(RawImage))]
        static void RawImage(RawImage img)
        {
            img.raycastTarget = false;
        }
        [OnAddComponent(typeof(InputField))]
        static void InputField(InputField input)
        {
            if (input.targetGraphic)
                input.targetGraphic.raycastTarget = true;
        }

        [OnAddComponent(typeof(Dropdown))]
        static void Dropdown(Dropdown dropdown)
        {
            if (dropdown.targetGraphic)
                dropdown.targetGraphic.raycastTarget = true;
        }
        [OnAddComponent(typeof(Button))]
        static void Button(Button btn)
        {
            if (btn.targetGraphic)
                btn.targetGraphic.raycastTarget = true;
        }
        static void DelayCall(Action action)
        {
            EditorApplication.delayCall += () =>
            {

                action?.Invoke();
            };
        }

        [OnAddComponent(typeof(Slider))]
        static void Slider(Slider slider)
        {
            DelayCall(() =>
            {
                var graphic = slider.transform.GetChild(0)?.GetComponent<Graphic>();
                if (graphic)
                {
                    graphic.raycastTarget = true;

                }
                if (slider.targetGraphic)
                    slider.targetGraphic.raycastTarget = true;

            });
        }

        [OnAddComponent(typeof(Scrollbar))]
        static void Scrollbar(Scrollbar bar)
        {
            DelayCall(() =>
            {
                if (bar.targetGraphic)
                    bar.targetGraphic.raycastTarget = true;

            });

        }
        [OnAddComponent(typeof(ScrollRect))]
        static void ScrollRect(ScrollRect rect)
        {
            DelayCall(() =>
            {

                if (rect.viewport)
                {
                    var graphic = rect.viewport.GetComponent<Graphic>();
                    if (graphic)
                    {
                        graphic.raycastTarget = true;
                    }
                }
            });
        }


        [OnAddComponent(typeof(Toggle))]
        static void Toggle(Toggle toggle)
        {
            DelayCall(() =>
            {
                if (toggle.targetGraphic)
                    toggle.targetGraphic.raycastTarget = true;
                if (toggle.graphic)
                    toggle.graphic.raycastTarget = true;
            });
        }


        [OnAddComponent(typeof(UIPanel))]
        static void UIPanel(UIPanel panel)
        {
            RectTransform _rect = null;
            for (int i = 0; i < panel.transform.childCount; i++)
            {
                _rect = panel.transform.GetChild(i).GetComponent<RectTransform>();
                if (_rect && _rect.anchorMax == Vector2.one && _rect.anchorMin == Vector2.zero)
                {
                    panel.adaptRect = _rect;
                    break;
                }
            }
            if (!panel.adaptRect)
                panel.adaptRect = panel.GetComponent<RectTransform>();
        }
    }
}
