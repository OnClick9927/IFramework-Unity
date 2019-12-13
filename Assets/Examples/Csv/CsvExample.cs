/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-09-08
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework;
using IFramework.Serialization.Csv;
using System.Collections.Generic;
using UnityEngine;

namespace IFramework_Demo
{
    /// <summary>
    /// 将表 和 对象 相互转化。 中间 牵涉到序列化。
    /// </summary>
    public class CsvExample : MonoBehaviour
    {
        struct dataMan{

            [CsvIgnore]
            public int age;             //被忽视的列
            [CsvColumnName("第一列")]         // 为列取别名
            public string sex;
            public string Name;             //正常的列名

            public override string ToString()
            {
                return $"{age}  {sex}  {Name}";
            }
        }

        // 用于建表的 类。三个字段：表达 一个表的 字段：也就是 列。
        class Man
        {

            [CsvIgnore]
            public int age;             //被忽视的列
            [CsvColumnName("性别")]         // 为列取别名
            public string sex;
            public string Name;             //正常的列名

            public override string ToString()
            {
                return $"{age}  {sex}  {Name}";
            }
        }
        string path;        //指定路径
        private void Awake()
        {
            //这是一个 String的扩展函数
            print (Application.dataPath);
            path = Application.dataPath.CombinePath("Examples/Csv/Mans3.csv");
           // Write();
           // Read();
           Write2();
        }
        //ICsvRow 需要根据具体情况改写一下分离字符串— —— 行
        //ICsvExplainer 基本不用改          —— 讲解
        //读取时候 具体的类型需要有一个公共的无参数构造函数，否则ICsvExplainer 需要改动
        void Read()
        {
            CsvReader r =
                new CsvReader(new System.IO.StreamReader(path, System.Text.Encoding.UTF8),
                new CsvRow(),
                new CsvExplainer());
            var cc = r.Get<Man>();

            foreach (var man in cc)
            {
                print(man.ToString());
            }
            Log.E(cc.Count);
            r.Dispose();
        }
        
        void Write()
        {
            List<Man> cs = new List<Man>()
            {
            new Man(){ age=1,sex="m",Name="xm0"},
            new Man(){ age=2,sex="m1",Name="xm1"},
            new Man(){ age=3,sex="m2",Name="xm2"},
            };

            CsvWriter w = new CsvWriter(new System.IO.StreamWriter(path, false),
                new CsvRow(),
                new CsvExplainer());
                // 参数二 应该是个路径，但是 io流已经获取过 一次路径了。
            w.Write(cs, "");
            w.Dispose();
        }

          void Write2()
        {
            List<dataMan> cs = new List<dataMan>()
            {
            new dataMan(){ age=1,sex="m",Name="xm0"},
            new dataMan(){ age=2,sex="m1",Name="xm1"},
            new dataMan(){ age=3,sex="m2",Name="xm2"},
            };

            CsvWriter w = new CsvWriter(new System.IO.StreamWriter(path, false),
                new CsvRow(),
                new CsvExplainer());
                // 参数二 应该是个路径，但是 io流已经获取过 一次路径了。
            w.Write(cs, "");
            w.Dispose();
        }
    }
}
