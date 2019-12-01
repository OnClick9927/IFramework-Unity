/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-09-08
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;

namespace IFramework.Serialization
{
    //实例化From字符串
    //object=》字符串
    public interface ICsvExplainer
    {
        T CreatInstance<T>(List<CsvColumn> cols, Dictionary<MemberInfo, string> membersDic);
        List<CsvColumn> GetColumns<T>(T t, Dictionary<MemberInfo, string> membersDic);
    }
    public class CsvExplainer : ICsvExplainer
    {
        protected T CreatInstance<T>()
        {
            return Activator.CreateInstance<T>();
        }
        public T CreatInstance<T>(List<CsvColumn> cols, Dictionary<MemberInfo, string> membersDic)
        {
            T t = CreatInstance<T>();
            membersDic.ForEach((pair) => {
                MemberInfo m = pair.Key;
                string csvName = pair.Value;
                CsvColumn column = cols.Find((c) => { return c.HeadLineName == csvName; });
                if (m is PropertyInfo)
                {
                    PropertyInfo info = m as PropertyInfo;
                    object obj = default(object);
                    if (StringConvert.TryConvert(column.StrValue, info.PropertyType, ref obj))
                        info.SetValue(t, obj, null);
                    else
                        throw new Exception(string.Format("Convert Err Type {0} Name {1} Value {2}", typeof(T), csvName, column.StrValue));
                }
                else
                {
                    FieldInfo info = m as FieldInfo;
                    object obj = default(object);
                    if (StringConvert.TryConvert(column.StrValue, info.FieldType, ref obj))
                        info.SetValue(t, obj);
                    else
                        throw new Exception(string.Format("Convert Err Type {0} Name {1} Value {2}", typeof(T), csvName, column.StrValue));
                }
            });
            return t;
        }

        public List<CsvColumn> GetColumns<T>(T t, Dictionary<MemberInfo, string> membersDic)
        {
            List<CsvColumn> columns = new List<CsvColumn>();
            membersDic.ForEach((member) =>
            {
                string val = string.Empty;
                MemberInfo m = member.Key;
                if (m is PropertyInfo)
                {
                    PropertyInfo info = m as PropertyInfo;
                    val = StringConvert.ConvertToString(info.GetValue(t, null), info.PropertyType);
                }
                else
                {
                    FieldInfo info = m as FieldInfo;
                    val = StringConvert.ConvertToString(info.GetValue(t), info.FieldType);
                }
                columns.Add(new CsvColumn()
                {
                    HeadLineName = member.Value,
                    StrValue = val
                });
            });
            return columns;
        }
    }
}
