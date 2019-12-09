/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-19
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework;

namespace IFramework_Demo
{
    public class ExplainExample :UnityEngine.MonoBehaviour, IExplainer<string, int>
    {
        public int ExplainToV(string t, IEventArgs arg, params object[] param)
        {
            return int.Parse(t);
        }

        public string ExplainToT(int v, IEventArgs arg, params object[] param)
        {
            return v.ToString();
        }
        private void Awake()
        {
            Log.L(Explanation<string, int>.CreateInstance().SetExplainer(new ExplainExample()).Explain("17", null));

        }
    }
}
