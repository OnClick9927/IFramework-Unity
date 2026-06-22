using System;

namespace IFramework
{
    public interface IPrefConverter
    {
        object FromString(Type type, string str);
        string ToString(object obj, Type type);
    }

}



