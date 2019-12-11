/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-11
 *Description:    Description
 *History:        2019-12-11--
*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using IFramework;

namespace IFramework_Demo
{
	public class InjectExample:UnityEngine.MonoBehaviour
	{
	    public interface INNN
        {
            void ToDo();
        }
        public class NNN : INNN
        {
            public void ToDo()
            {
                Log.L("13213213");
            }
        }
        [Inject("123")]
        static INNN nnn;
        private void Awake()
        {
            Framework.Container.RegisterInstance<INNN>(new NNN());
            //Framework.Container.onNotExistType += (type, name) =>
            //{
            //    return new NNN();
            //};
            Framework.Container.Inject(this);
            nnn.ToDo();
        }

    }
}
