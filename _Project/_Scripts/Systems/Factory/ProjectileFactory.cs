using UnityEngine;

public static class ProjectileFactory
{
    public static GameObject Create(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return ObjectPool.Instance.Get(prefab, position, rotation);
    }
}