/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-06-27
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IFramework.Example
{
    public class PoolTest:MonoBehaviour,IPoolObjectOwner
	{
        public static GameObject go;
        void Start()
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            PoolManager.SetCreater<CubeObj>(gg);
            PoolManager.GetPool<CubeObj>().SleepCapcity=2;
            PoolManager.AddCreaterDel<CubeObj>((type, arg,para) =>
            {
                CubeObj gg = new CubeObj();
                return gg;
            });
            cag.AddCreater((type, arg,para) =>
            {
                CubeObj gg = new CubeObj();
                return gg;

            });

        }
        CubeCollecter gg = new CubeCollecter();
        List<CubeObj> list = new List<CubeObj>();
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var a = PoolManager.Get<CubeObj>(this, null);
                //var a = cag.Get( null);

                list.Add(a as CubeObj);
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                PoolManager.Set<CubeObj>(list[0], this, null);
                //cag.Set(list[0], null);
                   
                list.RemoveAt(0);
            }
        }
        class CubeCollecter : PoolObjPool.PoolObjCollecter
        {
            protected override IPoolObject CreatNew(IEventArgs arg, params object[] param)
            {
                //base.CreatNew(arg, param);
                CubeObj gg = new CubeObj();
                return gg;
            }
           
        }

        class CubeObj : IPoolObject
        {
            public GameObject selfGO;

   

            public void OnCreate(IEventArgs arg, params object[] param)
            {
                selfGO = GameObject.Instantiate(go);
                //selfGO.SetActive(true);

            }

            public void OnGet(IEventArgs arg, params object[] param)
            {
                 selfGO.SetActive(true);

            }

            public void OnSet(IEventArgs arg, params object[] param)
            {
                selfGO.SetActive(false);
            }

            public void OnClear(IEventArgs arg, params object[] param)
            {
                GameObject.Destroy(selfGO);
            }
        }
        PoolObjPool<CubeObj> cag = new PoolObjPool<CubeObj>(new PoolObjPool<CubeObj>.PoolObjCollecter<CubeObj>(),new PoolObjPool<CubeObj>.PoolObjCollecter<CubeObj>(),true,2);
    }
}

