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
using System.IO;
using System.Linq;

namespace IFramework.Serialization
{
    public class CsvReader : IDisposable
    {
        private List<List<CsvColumn>> rows;
        private ICsvRow rowReader;
        private ICsvExplainer explainer;
        public StreamReader streamReader { get; private set; }

        public CsvReader(StreamReader streamReader, ICsvRow rowReader, ICsvExplainer explainer)
        {
            this.explainer = explainer;
            this.streamReader = streamReader;
            this.rowReader = rowReader;
            rows = new List<List<CsvColumn>>();
            Read();
        }
        private void Read()
        {
            string str = streamReader.ReadToEnd();
            List<string> rowValues = str.Replace("\r\n", "\n").Split('\n').ToList();
            string HeadStr = rowValues[0];
            rowValues.RemoveAt(0);
            List<string> headNames = rowReader.ReadHeadLine(HeadStr);


            if (string.IsNullOrEmpty(rowValues.Last())) rowValues.RemoveAt(rowValues.Count - 1);
            rowValues.ForEach((row) =>
            {
                List<CsvColumn> cols = rowReader.ReadLine(row, headNames);
                rows.Add(cols);
            });
        }
        public void Dispose()
        {
            rows.Clear();
            streamReader.Close();
            streamReader.Dispose();
        }

        public List<T> Get<T>()
        {
            List<T> ts = new List<T>();
            var members = CsvTool.GetMemberInfo(typeof(T));

            rows.ForEach((cols) =>
            {
                T t = explainer.CreatInstance<T>(cols, members);
                if (t != null) ts.Add(t);
            });
            return ts;
        }
    }

}
