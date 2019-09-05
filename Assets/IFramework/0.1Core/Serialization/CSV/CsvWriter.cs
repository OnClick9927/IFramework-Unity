/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
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
    public class CsvWriter : IDisposable
    {
        private ICsvExplainer explainer;
        public StreamWriter streamWriter { get; private set; }
        private ICsvRow rowWriter;
        public CsvWriter(StreamWriter streamWriter, ICsvRow rowWriter, ICsvExplainer explainer)
        {
            this.streamWriter = streamWriter;
            this.rowWriter = rowWriter;
            this.explainer = explainer;
        }
        public void Dispose()
        {
            streamWriter.Flush();
            streamWriter.Close();
        }

        public void Write<T>(List<T> ts, string path)
        {
            List<List<CsvColumn>> rows = new List<List<CsvColumn>>();
            var members = CsvTool.GetMemberInfo(typeof(T));
            ts.ForEach((t) => {
                var row = explainer.GetColumns<T>(t, members);
                rows.Add(row);
            });
            string result = string.Empty;
            result = result.Append(rowWriter.WriteHeadLine(members.Values.ToList())).Append("\r\n");
            rows.ForEach((row) => {
                result = result.Append(rowWriter.WriteLine(row)).Append("\r\n");
            });
            Log.E(result);
            streamWriter.Write(result);
        }
    }

}
