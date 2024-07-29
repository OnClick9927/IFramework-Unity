/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2020.3.3f1c1
 *Date:           2022-08-07
 *Description:    Description
 *History:        2022-08-07--
*********************************************************************************/
using System.Collections.Generic;
using System.Linq;

namespace IFramework.UI
{
    [System.Serializable]
    public class PanelCollection
    {
        [System.Serializable]
        public class Data
        {
            public string name { get { return System.IO.Path.GetFileNameWithoutExtension(path); } }

            public string path;
            public bool isResourcePath;
            public UILayer layer;
            public bool fullScreen;
            public int order;
        }
        public List<Data> datas = new List<Data>();

        private Dictionary<string, Data> map;
        public void ListToMap()
        {
            map = datas.ToDictionary(x => x.path);
        }
        public Data GetData(string path)
        {
            map.TryGetValue(path, out Data data);
            return data;
        }
    }
}
