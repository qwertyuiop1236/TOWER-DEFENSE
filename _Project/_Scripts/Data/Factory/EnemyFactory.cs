using UnityEngine;

public static class EnemyFactory
{
    public static Enemy Create(EnemyDataSO data, Vector3 position, Quaternion rotation)
    {
        if (data == null || data.prefab == null)
        {
            Debug.LogError("EnemyData или префаб не заданы!");
            return null;
        }

        GameObject obj = ObjectPool.Instance.Get(data.prefab, position, rotation);
        Enemy enemy = obj.GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError($"Префаб {data.prefab.name} не содержит компонента Enemy!");
            ObjectPool.Instance.Return(obj);
            return null;
        }

        // Передаём данные врагу через специальный метод
        enemy.SetData(data);
        
        return enemy;
    }
}