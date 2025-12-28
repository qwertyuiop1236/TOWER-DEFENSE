using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZombieHerald : EnemyZombie
{
    [Header("Herald Settings - Speed Aura")]
    [SerializeField] private float auraRadius = 4f;
    [SerializeField] private float speedBoostMultiplier = 1.5f;
    [SerializeField] private float auraCheckInterval = 0.3f;
    
    [Header("2D Settings")]
    [SerializeField] private LayerMask zombieLayerMask = -1; // Маска слоев
    
    [Header("Debug")]
    [SerializeField] private bool showDebug = true;
    [SerializeField] private bool alwaysShowGizmo = false;
    private List<EnemyZombie> buffedZombies = new List<EnemyZombie>();
    private CircleCollider2D triggerCollider;

    protected override void Start()
    {
        base.Start();
        
        // Создаем или получаем CircleCollider2D для триггера
        triggerCollider = GetComponent<CircleCollider2D>();
        if (triggerCollider == null)
        {
            triggerCollider = gameObject.AddComponent<CircleCollider2D>();
            triggerCollider.isTrigger = true;
        }
        
        triggerCollider.radius = auraRadius;
        
        // Настраиваем маску слоев
        if (zombieLayerMask == -1)
        {
            zombieLayerMask = LayerMask.GetMask("Default"); // Или ваш слой зомби
        }
        
        StartCoroutine(AuraUpdateCoroutine());
        
        if (showDebug)
            Debug.Log($"2D Герольд {name} запущен. Радиус: {auraRadius}");
    }

    private IEnumerator AuraUpdateCoroutine()
    {
        while (true)
        {
            UpdateAura2D();
            yield return new WaitForSeconds(auraCheckInterval);
        }
    }

    private void UpdateAura2D()
    {
        // 1. Ищем зомби в радиусе с помощью Physics2D
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
            transform.position, 
            auraRadius, 
            zombieLayerMask
        );
        
        List<EnemyZombie> currentZombies = new List<EnemyZombie>();
        
        foreach (Collider2D collider in hitColliders)
        {
            EnemyZombie zombie = collider.GetComponent<EnemyZombie>();
            if (zombie != null && zombie != this && zombie.enabled)
            {
                currentZombies.Add(zombie);
            }
        }
        
        if (showDebug && currentZombies.Count > 0)
            Debug.Log($"Найдено {currentZombies.Count} зомби в радиусе");

        // 2. Убираем буст с тех, кто вышел из радиуса
        for (int i = buffedZombies.Count - 1; i >= 0; i--)
        {
            EnemyZombie zombie = buffedZombies[i];
            
            if (zombie == null)
            {
                buffedZombies.RemoveAt(i);
                continue;
            }
            
            if (!currentZombies.Contains(zombie))
            {
                zombie.ResetSpeed();
                buffedZombies.RemoveAt(i);
            }
        }

        // 3. Добавляем буст новым зомби
        foreach (EnemyZombie zombie in currentZombies)
        {
            if (!buffedZombies.Contains(zombie))
            {
                zombie.ApplySpeedBoost(speedBoostMultiplier);
                buffedZombies.Add(zombie);
            }
        }
    }

    // Альтернативный вариант: использование триггеров 2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (showDebug) Debug.Log($"OnTriggerEnter2D: {other.name}");
        
        EnemyZombie zombie = other.GetComponent<EnemyZombie>();
        if (zombie != null && zombie != this)
        {
            zombie.ApplySpeedBoost(speedBoostMultiplier);
            if (!buffedZombies.Contains(zombie))
            {
                buffedZombies.Add(zombie);
                if (showDebug) Debug.Log($"{zombie.name} вошел в ауру");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        EnemyZombie zombie = other.GetComponent<EnemyZombie>();
        if (zombie != null && buffedZombies.Contains(zombie))
        {
            zombie.ResetSpeed();
            buffedZombies.Remove(zombie);
            if (showDebug) Debug.Log($"{zombie.name} вышел из ауры");
        }
    }

    protected override void Death()
    {
        if (showDebug)
            Debug.Log($"Герольд умирает, снимаем ускорение");
            
        // Снимаем ускорение со всех зомби
        foreach (EnemyZombie zombie in buffedZombies)
        {
            if (zombie != null) zombie.ResetSpeed();
        }
        
        base.Death();
    }

    // Визуализация в редакторе
    void OnDrawGizmosSelected()
    {
        if (!alwaysShowGizmo)
        {
            Gizmos.color = new Color(0, 1, 1, 0.3f);
            Gizmos.DrawWireSphere(transform.position, auraRadius);
        }
    }
    
    void OnDrawGizmos()
    {
        if (alwaysShowGizmo)
        {
            Gizmos.color = new Color(0, 1, 1, 0.1f);
            Gizmos.DrawSphere(transform.position, auraRadius);
        }
    }
}