/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.158
 *UnityVersion:   2019.4.16f1
 *Date:           2021-07-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine.UI;
using UnityEngine;
using System;

namespace IFramework.UI
{
	public class AtlasExample:MonoBehaviour
	{
        public GuideMask mask;
		public Image iamge;
        public int index = 0;
        public Text txt;
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            var size = this.iamge.rectTransform.rect.size / 2;
            mask.Focus(this.iamge.rectTransform,new Vector2(size.x,-size.y));
        }

        public void Set(int index)
        {
            txt.text = index.ToString();
        }

	}
}
