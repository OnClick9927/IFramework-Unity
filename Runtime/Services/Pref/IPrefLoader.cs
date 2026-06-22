namespace IFramework
{
    public interface IPrefLoader
    {
        string Load(string key);
        void Save(string key, string json);
    }

}



