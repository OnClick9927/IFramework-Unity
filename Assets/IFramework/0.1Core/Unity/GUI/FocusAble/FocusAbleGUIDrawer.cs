/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-27
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IFramework
{
    public class GUIFocusControl : ISingleton
    {
        private GUIFocusControl() { }
        private static GUIFocusControl Instance { get { return SingletonProperty<GUIFocusControl>.Instance; } }
        private List<IFocusAbleGUIDrawer> GUIs;
        private List<string> FocusIDs;
        public static string CurFocusID { get; private set; }
        public static void Focus(string FocusID, bool foucused)
        {
            CurFocusID = FocusID;
            if (foucused)
                GUI.FocusControl(null);
            for (int i = 0; i < Instance.GUIs.Count; i++)
            {
                if (Instance.GUIs[i].FocusID == FocusID) Instance.GUIs[i].Focused = true;
                else { Instance.GUIs[i].Focused = false; }
            }
            GUI.FocusControl(null);
            GUI.FocusControl(FocusID);
        }
        public static string GetFocusID(IFocusAbleGUIDrawer t)
        {
            if (!Instance.GUIs.Contains(t)) Instance.GUIs.Add(t);
            string id = t.GetHashCode().ToString();
            Instance.FocusIDs.Add(id);
            return id;
        }
        public static void Remove(IFocusAbleGUIDrawer t)
        {
            if (Instance.GUIs.Contains(t))
            {
                Instance.GUIs.Remove(t);
            }
        }
        public static bool ContansFocusID(string FocusID)
        {
            return Instance.FocusIDs.Contains(FocusID);
        }
        private void Calc()
        {
            string gfocusID = GUI.GetNameOfFocusedControl();
            for (int i = 0; i < GUIs.Count; i++)
            {
                if (GUIs[i].FocusID == gfocusID) GUIs[i].Focused = true;
                else { GUIs[i].Focused = false; }
            }
            GUI.FocusControl(null);
            GUI.FocusControl(gfocusID);
        }

        void ISingleton.OnSingletonInit()
        {
            GUIs = new List<IFocusAbleGUIDrawer>();
            FocusIDs = new List<string>();
        }

        public void Dispose()
        {
            GUIs.Clear();
            FocusIDs.Clear();
        }
    }

    public interface IFocusAbleGUIDrawer
    {
        string FocusID { get; }
        bool Focused { get; set; }
    }
    public abstract class FocusAbleGUIDrawer : IFocusAbleGUIDrawer,IDisposable
    {
        public string FocusID { get; private set; }
        public bool Focused { get; set; }
        public FocusAbleGUIDrawer()
        {
            FocusID = GUIFocusControl.GetFocusID(this);
        }
        public Rect position;
        public virtual void OnGUI(Rect position)
        {
            this.position = position;
        }

        public virtual void Dispose()
        {
            GUIFocusControl.Remove(this);
        }
    }

}
