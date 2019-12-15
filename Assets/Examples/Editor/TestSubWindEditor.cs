using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

    /// <summary>
    /// 测试Editor 类
    /// </summary>
public class TestSubWindEditor:MonoBehaviour
{
    
   [MenuItem("Qkuang/test")]
     static void TrueF()
    {
        IFramework.ExampleAssetProcessor.logEnable = true;

    }
   
   [MenuItem("Qkuang/test2")]

    public static void FlaseF()
    {
        IFramework.ExampleAssetProcessor.logEnable = false;

    }

    
}
