using System;

namespace IFramework
{
    public interface IPrefService : IService
    {
        void ClearAll();
        PrefContext<T> FindContext<T>() where T : class, new();
        void SetContext<T>(PrefContext<T> context) where T : class, new();
        object Load(Type type, string key);
        void SaveAll();
        void Save(string key, object obj);
    }
}