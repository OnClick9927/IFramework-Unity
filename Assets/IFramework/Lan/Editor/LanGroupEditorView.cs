/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-03
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
namespace IFramework
{
    [CustomEditor(typeof(LanGroup))]
	public class LanGroupEditorView:Editor
	{
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
