using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    // ОБЩИЕ ДЛЯ ВСЕХ ВРАГОВ
    [Header("Общие параметры для всех врагов")]
    [SerializeField] protected float _speedMuve =1f;
    [SerializeField] protected int _cost =100;
    [SerializeField ]protected float _xp=100;


    [SerializeField] protected Canvas canvasText;

    protected int _damage;

    private Transform[] waypoints; // Массив точек пути
    private int currentIndex = 0;  // Текущая точка

    public float Cost => _cost;
    public float XP =>_xp;

    protected virtual void Attack(int Damage)
    {
        // TODO: Нанести урон игроку
        Debug.Log("Враг достиг конца пути!");
        
        SistemUITest._xpPoint-=Damage;

        // Уничтожаем врага
        Destroy(gameObject);
    }
    protected virtual void Start()
    {
        // Получаем точки пути от PathManager
        waypoints = PathManager.Instance.waypoints;
        
        // Проверяем что точки есть
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("Нет точек пути! Добавьте Waypoints в PathManager.");
        }
    }
    
    protected virtual void Update()
    {
        // Если точек нет или дошли до конца - выходим
        if (waypoints == null || currentIndex >= waypoints.Length) return;
        
        // Получаем текущую цель
        Transform target = waypoints[currentIndex];
        
        // Двигаемся к цели
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            _speedMuve * Time.deltaTime
        );
        
        // Проверяем достигли ли цели
        float distance = Vector2.Distance(transform.position, target.position);
        if (distance < 0.1f) // Если близко к точке
        {
            // Переходим к следующей точке
            currentIndex++;
            
            // Если это была последняя точка
            if (currentIndex >= waypoints.Length)
            {
                Attack(_damage);
            }
        }
    }
}
