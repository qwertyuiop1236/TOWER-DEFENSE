using UnityEngine;

public class EnemyFastZombie : EnemyZombie
{
    [Header("настройка уникальных параметров FastZombie")]
    [SerializeField] protected float speedMuveFastZombie = 1.5f;    
    [SerializeField] protected int damageFastZombie = 1;
    [SerializeField] protected float xpFastZombie = 150f;
    
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