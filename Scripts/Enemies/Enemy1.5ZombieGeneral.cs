using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZombieGeneral : EnemyZombie
{
    [Header("настройка уникальных параметров ZombieGeneral")]
    [SerializeField] protected float speedMuveZombieGeneral;    
    [SerializeField] protected int damageZombieGeneral;
    [SerializeField] protected float xpZombieGeneral = 400f;

    [Header("Общие параметры для всех врагов")]
    [SerializeField] private GameObject zoneArmor; // Зона, где работает щит
    [SerializeField] private float shieldDuration = 5f; // Длительность щита
    [SerializeField] private float shieldCooldown = 10f; // Время между наложениями щита
    [SerializeField] private float shieldDamageReduction = 0.5f; // Снижение урона (50%)
    
    private float shieldTimer = 0f;
    private List<EnemyZombie> zombiesInZone = new List<EnemyZombie>();
    private Dictionary<EnemyZombie, Coroutine> activeShields = new Dictionary<EnemyZombie, Coroutine>();

    protected override void Start()
    {
        base.Start();

        _speedMuve = speedMuveZombieGeneral;
        _damage = damageZombieGeneral;
        _xp = xpZombieGeneral;
        
        // Начинаем отсчет таймера
        shieldTimer = shieldCooldown;
        
        // Если зона не назначена, используем текущий объект
        if (zoneArmor == null)
            zoneArmor = gameObject;
    }

    protected override void Update()
    {
        base.Update();
        
        // Обновляем таймер
        if (shieldTimer > 0)
        {
            shieldTimer -= Time.deltaTime;
            
            // Когда таймер достигает 0 - активируем щит
            if (shieldTimer <= 0)
            {
                ApplyShieldToZone();
                shieldTimer = shieldCooldown; // Сбрасываем таймер
            }
        }
    }

    // Метод для наложения щита на всех зомби в зоне
    private void ApplyShieldToZone()
    {
        // Находим всех зомби в зоне
        FindZombiesInZone();
        
        // Накладываем щит на каждого зомби
        foreach (var zombie in zombiesInZone)
        {
            if (zombie != null && zombie != this)
            {
                // Если у зомби уже есть активный щит, останавливаем его
                if (activeShields.ContainsKey(zombie))
                {
                    if (activeShields[zombie] != null)
                        StopCoroutine(activeShields[zombie]);
                }
                
                // Запускаем новый щит
                Coroutine shieldCoroutine = StartCoroutine(ApplyShieldToZombie(zombie));
                activeShields[zombie] = shieldCoroutine;
            }
        }
        
        Debug.Log($"Щит применен на {zombiesInZone.Count} зомби в зоне");
    }

    // Поиск зомби в указанной зоне
    private void FindZombiesInZone()
    {
        zombiesInZone.Clear();
        
        // Определяем радиус зоны (если используется сфера)
        float zoneRadius = 10f; // Можно сделать настраиваемым
        
        // Находим всех врагов в радиусе
        Collider[] colliders = Physics.OverlapSphere(zoneArmor.transform.position, zoneRadius);
        
        foreach (var collider in colliders)
        {
            EnemyZombie zombie = collider.GetComponent<EnemyZombie>();
            if (zombie != null)
            {
                zombiesInZone.Add(zombie);
            }
        }
    }

    // Корутина для применения щита к конкретному зомби
    private IEnumerator ApplyShieldToZombie(EnemyZombie zombie)
    {
        // Сохраняем оригинальный множитель урона
        float originalDamageMultiplier = 1f;
        
        // Если у зомби есть свойство для множителя урона, сохраняем его
        // Предполагаем, что у EnemyZombie есть свойство или метод для управления уроном
        // Если нет - нужно будет добавить в базовый класс
        
        // Устанавливаем снижение урона
        // Для этого нужен доступ к методу получения урона в базовом классе
        // Временно используем модификатор через рефлексию или добавим метод в базовый класс
        
        // Альтернативный вариант: использовать визуальный эффект щита
        GameObject shieldEffect = null;
        
        // Создаем визуальный эффект щита (если есть префаб)
        // shieldEffect = Instantiate(shieldPrefab, zombie.transform);
        
        Debug.Log($"Щит применен к {zombie.gameObject.name} на {shieldDuration} секунд");
        
        // Ждем длительность щита
        yield return new WaitForSeconds(shieldDuration);
        
        // Убираем щит
        RemoveShieldFromZombie(zombie);
        
        if (shieldEffect != null)
            Destroy(shieldEffect);
            
        Debug.Log($"Щит снят с {zombie.gameObject.name}");
    }

    // Метод для снятия щита
    private void RemoveShieldFromZombie(EnemyZombie zombie)
    {
        if (activeShields.ContainsKey(zombie))
        {
            activeShields.Remove(zombie);
        }
    }

    // Опционально: метод для проверки попадания в зону при спавне зомби
    private void OnTriggerEnter(Collider other)
    {
        // Если хотим добавлять зомби в зону динамически
        EnemyZombie zombie = other.GetComponent<EnemyZombie>();
        if (zombie != null && !zombiesInZone.Contains(zombie))
        {
            zombiesInZone.Add(zombie);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Удаляем зомби при выходе из зоны
        EnemyZombie zombie = other.GetComponent<EnemyZombie>();
        if (zombie != null && zombiesInZone.Contains(zombie))
        {
            zombiesInZone.Remove(zombie);
            RemoveShieldFromZombie(zombie);
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        Debug.Log("нанесенный урон " + damage);
    }
    
    // Визуализация зоны в редакторе
    private void OnDrawGizmosSelected()
    {
        if (zoneArmor != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawSphere(zoneArmor.transform.position, 10f);
        }
    }
}