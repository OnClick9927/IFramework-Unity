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
using System.Linq;
using System.Reflection;

namespace IFramework.Serialization
{
    public class CsvTool
    {
        public static Dictionary<MemberInfo, string> GetMemberInfo(Type type)
        {
            Dictionary<MemberInfo, string> members = new Dictionary<MemberInfo, string>();
            type.GetFields(/*BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static*/)
                .ToList().FindAll((field) => { return !field.IsDefined(typeof(CsvIgnoreAttribute), false); })
                 .ForEach((field) => {
                     if (field.IsDefined(typeof(CsvColumnNameAttribute), false))
                         members.Add(field, (field.GetCustomAttributes(false)
                                     .ToList()
                                     .Find((o) => { return o.GetType() == typeof(CsvColumnNameAttribute); })
                                     as CsvColumnNameAttribute)
                                     .Name);
                     else
                         members.Add(field, field.Name);
                 });
            type.GetProperties(/*BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static*/)
               .ToList().FindAll((property) => { return !property.IsDefined(typeof(CsvIgnoreAttribute), false); })
               .ForEach((property) => {
                   if (property.IsDefined(typeof(CsvColumnNameAttribute), false))
                       members.Add(property, (property.GetCustomAttributes(false)
                                   .ToList()
                                   .Find((o) => { return o.GetType() == typeof(CsvColumnNameAttribute); })
                                   as CsvColumnNameAttribute)
                                   .Name);
                   else
                       members.Add(property, property.Name);
               });
            return members;
        }
    }

}
