/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-19
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework.Example
{
    public class ExplainExample : IExplainer<string, int>
    {


        public int ExplainToV(string t, IEventArgs arg, params object[] param)
        {
            return int.Parse(t);
        }

        public string ExplainToT(int v, IEventArgs arg, params object[] param)
        {
            return v.ToString();
        }
       
        public void Test()
        {

          Log.L(  Explanation<string, int>.CreateInstance().SetExplainer(new ExplainExample()).Explain("17",null));
        }
    }
}
