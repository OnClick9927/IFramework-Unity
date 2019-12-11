/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-06-27
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework;
using IFramework.Pool;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IFramework_Demo
{
    public class PoolTest:MonoBehaviour,IPoolObjectOwner
	{
        class CubeObj : IPoolObject
        {
            public GameObject selfCube;
            public void OnCreate(IEventArgs arg, params object[] param)
            {
                selfCube = GameObject.Instantiate(cube);
            }
            public void OnGet(IEventArgs arg, params object[] param)
            {
                selfCube.SetActive(true);
            }
            public void OnSet(IEventArgs arg, params object[] param)
            {
                selfCube.SetActive(false);
            }
            public void OnClear(IEventArgs arg, params object[] param)
            {
                GameObject.Destroy(selfCube);
            }
        }

        public static GameObject cube;
        void Start()
        {
            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var pool = PoolManager.GetPool<CubeObj>();
            pool.SleepCapcity = 2;
            pool.AutoClear = true;
            //pool.AddCreater((type, arg, para) => {
            //    CubeObj gg = new CubeObj();
            //    return gg;
            //});
            PoolManager.AddCreaterDel<CubeObj>((type, arg,para) =>
            {
                CubeObj gg = new CubeObj();
                return gg;
            });


        }
        List<CubeObj> list = new List<CubeObj>();
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var a = PoolManager.Get<CubeObj>(this, null);
                list.Add(a as CubeObj);
            }
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (PoolManager.GetPool<CubeObj>().RunningCount>0)
                {
                   var obj=  PoolManager.GetPool<CubeObj>().RunningPoool.Peek();
                    PoolManager.Set<CubeObj>(obj as CubeObj, this, null);

                }
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                if (PoolManager.GetPool<CubeObj>().RunningCount > 0)
                {
                    var obj = PoolManager.GetPool<CubeObj>().RunningPoool.Peek();
                    //PoolManager.Set<CubeObj>(obj as CubeObj, this, null);
                    PoolManager.Clear(obj,null,false);

                }
            }
        }
    }
}

