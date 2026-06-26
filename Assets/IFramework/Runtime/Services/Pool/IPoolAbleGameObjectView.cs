using UnityEngine;

namespace IFramework
{
    public interface IPoolAbleGameObjectView
    {
        string PoolKey { get; set; }
        GameObject gameObject { get; }
    }

}
