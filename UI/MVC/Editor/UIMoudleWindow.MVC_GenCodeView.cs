/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2020.3.3f1c1
 *Date:           2022-08-03
 *Description:    Description
 *History:        2022-08-03--
*********************************************************************************/
using UnityEngine;
using System;
using static IFramework.UI.UIMoudleWindow;

namespace IFramework.UI.MVC
{
    public partial class UIMoudleWindow
    {
        [Serializable]
        public class MVC_GenCodeView : UIGenCode<UIPanel>
        {
            public override string name { get { return "CS/MVC_Gen_CS"; } }

            protected override GameObject gameobject => panel.gameObject;
            protected string designScriptName { get { return $"{viewName}.Design.cs"; } }
            protected override string viewScriptName { get { return $"{viewName}.cs"; } }

            protected override void OnFindDirSuccess()
            {
               
            }

            protected override void LoadLastData()
            {
                var last = EditorTools.GetFromPrefs<MVC_GenCodeView>(name);
                if (last != null)
                {
                    this.panel = last.panel;
                    this.UIdir = last.UIdir;
                    this.state = last.state;
                }
            }

            protected override void WriteView()
            {

                GenItemCodeCS.Write(creater, UIdir.CombinePath(viewScriptName), UIdir.CombinePath(designScriptName),
                   viewDesignScriptOrigin, viewScriptOrigin);

            }

            private const string viewDesignScriptOrigin = GenItemCodeCS.head +
            "namespace #UserNameSpace#\n" +
            "{\n" +
            "\tpublic partial class #UserSCRIPTNAME# : IFramework.UI.MVC.UIView \n" +
            "\t{\n" +
             "#field#\n" +
            "\t\tprotected override void InitComponents()\n" +
            "\t\t{\n" +
            "#findfield#\n" +
            "\t\t}\n" +
            "\t}\n" +
            "}";
            private const string viewScriptOrigin = GenItemCodeCS.head +
            "namespace #UserNameSpace#\n" +
            "{\n" +
            "\tpublic partial class #UserSCRIPTNAME#\n" +
            "\t{\n" +
            "\t\tprotected override void OnLoad()\n" +
            "\t\t{\n" +

            "\t\t}\n" +
            "\n" +
            "\t\tprotected override void OnShow()\n" +
            "\t\t{\n" +
            "\t\t}\n" +
            "\n" +
             "\t\tprotected override void OnHide()\n" +
            "\t\t{\n" +
            "\t\t}\n" +
            "\n" +
            "\t\tprotected override void OnClose()\n" +
            "\t\t{\n" +
            "\t\t}\n" +
            "\n" +
            "\t}\n" +
            "}";


        }
    }
}
