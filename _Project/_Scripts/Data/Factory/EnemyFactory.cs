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

        // Присваиваем данные (если в префабе не заданы, то задаём сейчас)
        // Желательно, чтобы в префабе уже был ссылка на EnemyData, но на всякий случай:
        if (enemy.TryGetComponent<EnemyDataSO>(out var existingData) == false || existingData == null)
        {
            // можно присвоить через поле _data (но оно private serialized, поэтому нужен публичный метод)
            enemy.SetData(data); // добавим метод в Enemy
        }
        return enemy;
    }
}