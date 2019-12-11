/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-27
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IFramework.GUITool
{
    public class GUIFocusControl : SingletonPropertyClass<GUIFocusControl>
    {
        private GUIFocusControl() { }
        private List<IFocusAbleGUIDrawer> drawers;
        private List<string> UUIDs;
        public static string CurFocusID
        {
            get
            {
                if (CurFocusDrawer == null)
                    return string.Empty;
                return CurFocusDrawer.FocusID;
            }
        }
        public static IFocusAbleGUIDrawer CurFocusDrawer { get; private set; }

        public static string Subscribe(IFocusAbleGUIDrawer drawer)
        {
            if (!Instance.drawers.Contains(drawer)) Instance.drawers.Add(drawer);
            string uuid = drawer.GetHashCode().ToString();
            if (Instance.UUIDs.Contains(uuid))
                uuid += "1";
            Instance.UUIDs.Add(uuid);
            return uuid;
        }
        public static void UnSubscribe(IFocusAbleGUIDrawer drawer)
        {
            if (Instance.drawers.Contains(drawer))
            {
                Instance.drawers.Remove(drawer);
            }
            if (Instance.UUIDs.Contains(drawer.FocusID))
            {
                Instance.UUIDs.Remove(drawer.FocusID);
            }
        }

        public static bool Contans(IFocusAbleGUIDrawer drawer)
        {
            return Instance.drawers.Contains(drawer);
        }

        public static void Focus(IFocusAbleGUIDrawer drawer)
        {
            if (CurFocusDrawer == drawer)
                GUI.FocusControl(null);
            CurFocusDrawer = drawer;

            for (int i = 0; i < Instance.drawers.Count; i++)
            {
                if (Instance.drawers[i] == drawer) Instance.drawers[i].Focused = true;
                else { Instance.drawers[i].Focused = false; }
            }
            GUI.FocusControl(null);
            GUI.FocusControl(drawer.FocusID);
        }
        public static void Diffuse(IFocusAbleGUIDrawer drawer)
        {
            if (CurFocusDrawer == drawer)
            {
                for (int i = 0; i < Instance.drawers.Count; i++)
                {
                    Instance.drawers[i].Focused = false;
                }
                GUI.FocusControl(null);
                CurFocusDrawer = null;
            }
        }


        protected override void OnSingletonInit()
        {
            drawers = new List<IFocusAbleGUIDrawer>();
            UUIDs = new List<string>();
        }

        public override void Dispose()
        {
            drawers.Clear();
            UUIDs.Clear();
        }
    }

    public interface IFocusAbleGUIDrawer
    {
        string FocusID { get; }
        bool Focused { get; set; }
        void Focus();
        void Difuse();
    }
    public abstract class FocusAbleGUIDrawer : IFocusAbleGUIDrawer,IDisposable
    {
        public string FocusID { get; private set; }
        protected bool focused;
        public bool Focused
        {
            get
            {
                return focused;
            }
            set
            {
                if (value)
                    OnFcous();
                else
                {
                    OnFocusOther();
                    if (!focused)
                        OnLostFous();
                }
                focused = value;
            }
        }
        public FocusAbleGUIDrawer()
        {
            FocusID = GUIFocusControl.Subscribe(this);
        }
        public Rect position { get; private set; }
        protected virtual void OnFcous() { }
        protected virtual void OnFocusOther() { }
        protected virtual void OnLostFous() { }


        public virtual void OnGUI(Rect position)
        {
            this.position = position;
        }

        public virtual void Dispose()
        {
            GUIFocusControl.UnSubscribe(this);
        }

        public virtual void Focus()
        {
            GUIFocusControl.Focus(this);
        }

        public virtual void Difuse()
        {
            GUIFocusControl.Diffuse(this);
        }
    }

}
