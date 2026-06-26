/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
namespace IFramework
{
    public interface IRedTreeService:IService
    {
        void ClearAll();
        void ClearPath(string key);
        void FreshDots();
        int GetCount(string key);
        int GetDotCount(string key);
        void ReadPath(string key);
        void SetCount(string key, int count);
    }
}