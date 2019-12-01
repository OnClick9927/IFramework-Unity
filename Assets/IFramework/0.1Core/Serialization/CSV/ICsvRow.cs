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

namespace IFramework.Serialization
{
    //切割一行字符串
    public interface ICsvRow
    {
        List<CsvColumn> ReadLine(string val, List<string> headNames);
        List<string> ReadHeadLine(string val);
        string WriteLine(List<CsvColumn> cols);
        string WriteHeadLine(List<string> headNames);
    }
    public class CsvRow : ICsvRow
    {
        //private SortedList<int, string> GetLineItems(string src)
        //{
        //    if (string.IsNullOrEmpty(src)) return null;
        //    SortedList<int, string> sl = new SortedList<int, string>();
        //    src = src.Replace("\"\"", "'");
        //    var ie = Regex.Matches(src, "[,]?\"([^\"]+)\",", RegexOptions.ExplicitCapture).GetEnumerator();
        //    while (ie.MoveNext())
        //    {
        //        string patn = ie.Current.ToString();
        //        int key = src.Substring(0, src.IndexOf(patn)).Split(',').Length;
        //        if (sl.ContainsKey(key)) continue;
        //        if (src.IndexOf(patn) == 0 && !patn.StartsWith(","))
        //        {
        //            sl.Add(0, patn.Trim(new char[] { ',', '"' }).Replace("'", "\""));
        //            src = src.Replace(patn, ",");
        //        }
        //        else
        //        {
        //            sl.Add(key, patn.Trim(new char[] { ',', '"' }).Replace("'", "\""));
        //            src = src.Replace(patn, ",,");
        //        }
        //    }
        //    src = src.Replace(" ", "");
        //    src = src.Replace("'", "\"");
        //    ie = Regex.Matches(src, "\"[\\w]+\":\"[\\w]+\",", RegexOptions.ExplicitCapture).GetEnumerator();
        //    while (ie.MoveNext())
        //    {
        //        string patn = ie.Current.ToString();
        //        string temp = src.Substring(0, src.IndexOf(patn) + patn.Length - 1);
        //        src = src.Replace(temp, temp + "\"");
        //    }
        //    src = src.Replace("\",", "#");
        //    src = src.Replace("\"},{\"", "##");
        //    string[] arr = src.Split(',');
        //    for (int i = 0; i < arr.Length; i++)
        //    {
        //        if (sl.ContainsKey(i)) continue;
        //        sl.Add(i, arr[i].Trim(new char[] { ',', '"' }).Replace("#", ",").Replace("##", "\"},{\""));
        //    }
        //    return sl;
        //}

        public List<CsvColumn> ReadLine(string val, List<string> headNames)
        {
            List<string> strVals = SpilitRow(val);
            if (strVals.Count != headNames.Count) throw new Exception("Read Err Count Is different");

            List<CsvColumn> cols = new List<CsvColumn>();
            for (int i = 0; i < headNames.Count; i++)
            {
                cols.Add(new CsvColumn()
                {
                    StrValue = strVals[i],
                    HeadLineName = headNames[i]
                });
            }
            return cols;
        }
        protected virtual List<string> SpilitRow(string val)
        {
            var list = val.Split(',').ToList();
            list.RemoveAt(list.Count - 1);
            return list;
        }
        public virtual List<string> ReadHeadLine(string val)
        {
            List<string> headNames = val.Split(',').ToList();
            if (string.IsNullOrEmpty(headNames.Last()))
            {
                headNames.RemoveAt(headNames.Count - 1);
            }
            return headNames;
        }

        public string WriteLine(List<CsvColumn> cols)
        {
            string result = string.Empty;
            cols.ForEach((index, c) => {
                string val = c.StrValue;
                val = val.Replace("\"", "\"\"");
                if (val.Contains(",") || val.Contains("\"") || val.Contains('\r') || val.Contains('\n'))
                    val = string.Format("\"{0}\"", val);
                if (index == cols.Count - 1) result = result.Append(val).Append(",");
                else result = result.Append(val).Append(",");
            });
            return result;
        }
        public string WriteHeadLine(List<string> headNames)
        {
            string result = string.Empty;
            headNames.ForEach((index, val) => {
                val = val.Replace("\"", "\"\"");
                if (val.Contains(",") || val.Contains("\"") || val.Contains('\r') || val.Contains('\n'))
                    val = string.Format("\"{0}\"", val);
                if (index == headNames.Count - 1) result = result.Append(val).Append(",");
                else result = result.Append(val).Append(",");
            });
            return result;
        }
    }
}
