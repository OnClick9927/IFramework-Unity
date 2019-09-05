/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-09-08
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace IFramework.Example
{
	public class CsvExample
	{
        class cc
        {
            [CsvIgnore]
            public int age;
            [CsvColumnName("性别")]
            public string sex;
            public string Name;
        }

        //ICsvRow 需要根据具体情况改写一下分离字符串
        //ICsvExplainer 基本不用改
        //读取时候 具体的类型需要有一个公共的无参数构造函数，否则ICsvExplainer 需要改动
        void Read() {
            CsvReader r =
                new CsvReader(new System.IO.StreamReader(Application.dataPath.CombinePath("cc.csv"), System.Text.Encoding.UTF8),
                new CsvRow(),
                new CsvExplainer());
            var cc = r.Get<cc>();
            Log.E(cc.Count);
            r.Dispose();
        }
        void Write() {
            List<cc> cs = new List<cc>()
            {
            new cc(){ age=1,sex="m",Name="xm"},
            new cc(){ age=2,sex="m1",Name="xm1"},
            new cc(){ age=3,sex="m2",Name="xm2"},
            };
            CsvWriter w = new CsvWriter(new System.IO.StreamWriter(Application.dataPath.CombinePath("cc.csv"), false),
                new CsvRow(),
                new CsvExplainer());
            w.Write(cs, "");
            w.Dispose();
        }
    }
}
