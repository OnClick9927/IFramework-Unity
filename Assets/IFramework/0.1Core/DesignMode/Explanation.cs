/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework
{
    public interface IExplainer<T, V>
    {
        V ExplainToV(T t, IEventArgs arg, params object[] param);
        T ExplainToT(V v, IEventArgs arg, params object[] param);
    }
    public class Explanation<T, V>
    {
        private Explanation() { }
        public IExplainer<T, V> Explainer { get; set; }
        public static Explanation<T, V> CreateInstance()
        {
            return new Explanation<T, V>();
        }
        public Explanation<T, V> SetExplainer(IExplainer<T, V> Explainer)
        {
            this.Explainer = Explainer;
            return this;
        }
        public V Explain(T t, IEventArgs arg, params object[] param)
        {
            return Explainer.ExplainToV(t, arg, param);
        }
        public T Explain(V v, IEventArgs arg, params object[] param)
        {
            return Explainer.ExplainToT(v, arg, param);
        }
    }
}
