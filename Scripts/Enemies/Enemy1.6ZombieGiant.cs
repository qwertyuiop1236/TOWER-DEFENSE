using UnityEngine;

public class EnemyZombieGiant : EnemyZombie
{
    [Header("настройка уникальных параметров ZombieGiant")]
    [SerializeField] protected float speedMuveFastZombie = 0.1f;    
    [SerializeField] protected int damageFastZombie = 10;
    [SerializeField] protected float xpFastZombie = 1500f;
    
    protected override void Start()
    {
        base.Start();

        _speedMuve =speedMuveFastZombie;
        _damage= damageFastZombie;
        _xp = xpFastZombie;
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        Debug.Log("нанесенный урон " + damage);
    }
}