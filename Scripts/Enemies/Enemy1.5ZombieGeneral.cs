using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZombieGeneral : EnemyZombie
{
    [Header("Настройка ауры защиты")]
    [SerializeField] private float auraRadius = 5f;
    [SerializeField] private float armorBonus = 50f;
    [SerializeField] private float auraCheckInterval = 0.5f;
    [SerializeField] private bool showDebug = true;
    
    [Header("2D настройки")]
    [SerializeField] private LayerMask zombieLayerMask = -1;
    
    private List<EnemyZombie> buffedZombies = new List<EnemyZombie>();
    private CircleCollider2D auraCollider;

    protected override void Start()
    {
        base.Start();
        
        // Создаем или получаем CircleCollider2D для ауры
        auraCollider = GetComponent<CircleCollider2D>();
        if (auraCollider == null)
        {
            auraCollider = gameObject.AddComponent<CircleCollider2D>();
            auraCollider.isTrigger = true;
        }
        
        auraCollider.radius = auraRadius;
        
        // Настраиваем маску слоев
        if (zombieLayerMask == -1)
        {
            zombieLayerMask = LayerMask.GetMask("Enemy", "Default");
        }
        
        StartCoroutine(AuraUpdateCoroutine());
        
        if (showDebug)
            Debug.Log($"2D Генерал {name} запущен. Радиус ауры: {auraRadius}, бонус брони: {armorBonus}");
    }
    
    private IEnumerator AuraUpdateCoroutine()
    {
        while (true)
        {
            UpdateArmorAura();
            yield return new WaitForSeconds(auraCheckInterval);
        }
    }
    
    private void UpdateArmorAura()
    {
        // Ищем зомби в радиусе с помощью Physics2D
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
        
        // Убираем бонус с тех, кто вышел из радиуса
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
                RemoveArmorBonus(zombie);
                buffedZombies.RemoveAt(i);
            }
        }
        
        // Добавляем бонус новым зомби
        foreach (EnemyZombie zombie in currentZombies)
        {
            if (!buffedZombies.Contains(zombie))
            {
                ApplyArmorBonus(zombie);
                buffedZombies.Add(zombie);
            }
        }
        
        if (showDebug && currentZombies.Count > 0)
            Debug.Log($"Аура защиты: {buffedZombies.Count} зомби в радиусе");
    }
    
    // Метод для применения бонуса брони
    private void ApplyArmorBonus(EnemyZombie zombie)
    {
        if (zombie == null) return;
        
        // Просто вызываем метод родительского класса
        zombie.AppArmor(armorBonus);
        
        if (showDebug)
            Debug.Log($"Броня {zombie.name} увеличена до {zombie._Armor}");
    }
    
    // Метод для снятия бонуса брони
    private void RemoveArmorBonus(EnemyZombie zombie)
    {
        if (zombie == null) return;
        
        // Уменьшаем броню на величину бонуса
        zombie.AppArmor(-armorBonus);
        
        // Защита от отрицательного значения брони
        if (zombie._Armor < 0)
            zombie.ArmorZero();
        
        if (showDebug)
            Debug.Log($"Броня {zombie.name} уменьшена до {zombie._Armor}");
    }
    
    // Использование триггеров 2D для динамического добавления/удаления
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled) return;
        
        EnemyZombie zombie = other.GetComponent<EnemyZombie>();
        if (zombie != null && zombie != this && !buffedZombies.Contains(zombie))
        {
            ApplyArmorBonus(zombie);
            buffedZombies.Add(zombie);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!enabled) return;
        
        EnemyZombie zombie = other.GetComponent<EnemyZombie>();
        if (zombie != null && buffedZombies.Contains(zombie))
        {
            RemoveArmorBonus(zombie);
            buffedZombies.Remove(zombie);
        }
    }
    
    protected override void Death()
    {
        if (showDebug)
            Debug.Log($"Генерал умирает, снимаем бонусы брони");
        
        // Снимаем бонусы со всех зомби перед смертью
        foreach (EnemyZombie zombie in buffedZombies)
        {
            if (zombie != null)
            {
                RemoveArmorBonus(zombie);
            }
        }
        
        buffedZombies.Clear();
        StopAllCoroutines();
        
        base.Death();
    }
    
    // Визуализация ауры в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, auraRadius);
    }
    
    void OnDrawGizmos()
    {
        if (showDebug && alwaysShowGizmo)
        {
            Gizmos.color = new Color(1, 0.5f, 0, 0.1f);
            Gizmos.DrawSphere(transform.position, auraRadius);
        }
    }
    
    // Для обратной совместимости
    [Header("Debug Options")]
    [SerializeField] private bool alwaysShowGizmo = false;
    
    // Метод для проверки сколько зомби сейчас под баффом
    public int GetBuffCount()
    {
        return buffedZombies.Count;
    }
    
    // Метод для получения списка зомби под баффом
    public List<EnemyZombie> GetBuffedZombies()
    {
        return new List<EnemyZombie>(buffedZombies);
    }
    
    // Метод для проверки находится ли зомби под баффом
    public bool IsZombieBuffed(EnemyZombie zombie)
    {
        return buffedZombies.Contains(zombie);
    }
}