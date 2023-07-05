/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2020-01-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using UnityEditor;
using UnityEngine;
using System.Text;
using System.IO;

namespace IFramework.UI
{
    public partial class UIMoudleWindow
    {
        public class CSTab : UIMoudleWindowTab
        {
            private static void CS_BuildPanelNames()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("public class PanelNames");
                sb.AppendLine("{");
                foreach (var data in collect.datas)
                {
                    sb.AppendLine($"\t public static string {data.name} = \"{data.path}\";");
                }
                sb.AppendLine("}");
                File.WriteAllText(cs_path, sb.ToString());
                AssetDatabase.Refresh();
            }

            public override string name => "CS";

            public override void OnGUI()
            {
                if (GUILayout.Button("Build Panel Names"))
                {
                    Collect();
                    CS_BuildPanelNames();
                }
            }
        }
    }
}
