/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;

namespace IFramework.Localization
{
    class CSVHelper
    {
        private static string FieldsToLine(IEnumerable<string> fields)
        {
            if (fields == null) return string.Empty;
            fields = fields.Select(field =>
            {
                if (field == null) field = string.Empty;
                return field;
            });
            string line = string.Format("{0}{1}", string.Join(",", fields), Environment.NewLine);
            return line;
        }
        public static void Write(string path, List<string[]> fields)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < fields.Count; i++)
            {
                sb.Append(FieldsToLine(fields[i]));
            }
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }


        static int index = 0;
        static string txt;
        static StringBuilder sbr;
        static Regex lineReg;
        static Regex fieldReg;
        static Regex quotesReg;
        public static void BeginRead(string path)
        {
            lineReg = new Regex(LocalizationSetting.lineReg);
            fieldReg = new Regex(LocalizationSetting.fieldReg);
            quotesReg = new Regex(LocalizationSetting.quotesReg);
            sbr = new StringBuilder();
            index = 0;
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Dispose();
            txt = Encoding.UTF8.GetString(bytes);
        }

        public static string[] ReadFields()
        {
            while (true)
            {
                var line = ReadLine(txt, ref index);
                if (line == null) break;
                sbr.Append(line);
                //一个完整的CSV记录行，它的双引号一定是偶数
                if (lineReg.Matches(sbr.ToString()).Count % 2 == 0)
                {
                    var fields = ParseCsvLine(sbr.ToString(), fieldReg, quotesReg).ToArray();
                    sbr.Clear();
                    return fields;
                }
                else
                    sbr.Append(Environment.NewLine);
            }
            return null;
        }
        private static string ReadLine(string txt, ref int index)
        {
            StringBuilder sbr = new StringBuilder();
            char c;
            while (txt.Length > index)
            {
                c = (char)txt[index];
                index++;
                if (c == '\n' && sbr.Length > 0 && sbr[sbr.Length - 1] == '\r')
                {
                    sbr.Remove(sbr.Length - 1, 1);
                    return sbr.ToString();
                }
                else
                {
                    sbr.Append(c);
                }
            }
            return sbr.Length > 0 ? sbr.ToString() : null;
        }
        private static List<string> ParseCsvLine(string line, Regex fieldReg, Regex quotesReg)
        {
            var fieldMath = fieldReg.Match(line);
            List<string> fields = new List<string>();
            while (fieldMath.Success)
            {
                string field;
                if (fieldMath.Groups[1].Success)
                {
                    field = quotesReg.Replace(fieldMath.Groups[1].Value, "\"");
                }
                else
                {
                    field = fieldMath.Groups[2].Value;
                }
                fields.Add(field);
                fieldMath = fieldMath.NextMatch();
            }
            return fields;
        }
    }
}
