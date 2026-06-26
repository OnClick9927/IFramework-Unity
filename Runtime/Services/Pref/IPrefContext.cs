namespace IFramework
{
    internal interface IPrefContext
    {
        string key { get; set; }

        void ClearValue();
        void SetValue(object pref);
    }

}



