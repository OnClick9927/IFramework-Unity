/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-07-31
*********************************************************************************/
using System;
using UnityEngine;
using static IFramework.UI.UnityEventHelper;
namespace IFramework
{
	public class PanelOneItemView : IFramework.UI.UIItemView 
	{
//FieldsStart
		private UnityEngine.UI.Image PanelOneItem;


        //FieldsEnd
        protected override void InitComponents()
		{
		//InitComponentsStart
			PanelOneItem = GetComponent<UnityEngine.UI.Image>("");

		//InitComponentsEnd
		}

        public override void OnSet()
        {
            Log.L("OnSet");
        }

        protected override void OnGet()
        {
            Log.L("OnGet");
        }

        internal void SetColor(Color color)
        {
            this.PanelOneItem.color = color;
        }
    }
}