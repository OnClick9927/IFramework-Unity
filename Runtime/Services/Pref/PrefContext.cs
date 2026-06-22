namespace IFramework
{
    public class PrefContext<T> : IPrefContext where T : class, new()
    {
        public T Value { get; set; }
        public string key { get; set; }

        void IPrefContext.ClearValue()
        {
            key = string.Empty;
            Value = default(T);
        }


        void IPrefContext.SetValue(object pref)
        {
            Value = pref as T;
        }
    }

}



