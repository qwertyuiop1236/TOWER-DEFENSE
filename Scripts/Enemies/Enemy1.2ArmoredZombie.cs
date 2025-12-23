using UnityEngine;

public class EnemyArmoredZombie: EnemyZombie
{
    [Header("настройка уникальных параметров ArmoredZombie")]
    [SerializeField] protected float speedMuveFastZombie = 0.5f;    
    [SerializeField] protected int damageFastZombie = 3;
    [SerializeField] protected float xpFastZombie = 350f;
    [SerializeField] protected float armorFastZombie = 150f;

    
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
        if (armorFastZombie - damage > 0)
        {
            armorFastZombie -= damage;
            Debug.Log("Урона по бране " + damage);
        }
        else
        {
            armorFastZombie=-1;
            base.TakeDamage(damage);
            Debug.Log("нанесенный урон " + damage);
        }
    }
}