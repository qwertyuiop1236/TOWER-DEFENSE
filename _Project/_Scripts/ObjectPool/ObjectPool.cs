using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Пул объектов для переиспользования экземпляров (враги, снаряды).
/// Реализует синглтон и словарь очередей для каждого префаба.
/// </summary>
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(prefab))
            poolDictionary[prefab] = new Queue<GameObject>();

        GameObject obj;
        if (poolDictionary[prefab].Count > 0)
        {
            obj = poolDictionary[prefab].Dequeue();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(prefab, position, rotation);
            var marker = obj.GetComponent<PooledObject>();
            if (marker == null) marker = obj.AddComponent<PooledObject>();
            marker.Prefab = prefab;
        }

        // Вызываем интерфейс у объекта
        IPoolable poolable = obj.GetComponent<IPoolable>();
        poolable?.OnGetFromPool();

        return obj;
    }

    public void Return(GameObject obj)
    {
        if (obj == null) return;

        // Уведомляем о возврате (сброс состояния)
        IPoolable poolable = obj.GetComponent<IPoolable>();
        poolable?.OnReturnToPool();

        obj.SetActive(false);

        // Находим маркер, чтобы положить в правильную очередь
        PooledObject marker = obj.GetComponent<PooledObject>();
        if (marker != null && marker.Prefab != null)
        {
            if (!poolDictionary.ContainsKey(marker.Prefab))
                poolDictionary[marker.Prefab] = new Queue<GameObject>();
            poolDictionary[marker.Prefab].Enqueue(obj);
        }
        else
        {
            // Если маркера нет – уничтожаем (защита от ошибок)
            Destroy(obj);
        }
    }
}
