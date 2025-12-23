using UnityEngine;

public class EnemyZombieCavaleria : EnemyZombie
{
    [Header("настройка уникальных параметров ZombieCavaleria")]
    [SerializeField] protected float speedMuveFastZombie = 0.75f;    
    [SerializeField] protected float speedMuveHorse = 3f;  
    [SerializeField] protected int damageFastZombie = 3;
    [SerializeField] protected float xpFastZombie = 150f;
    [SerializeField] protected float xpHorse = 200f;

    
    protected override void Start()
    {
        base.Start();

        _speedMuve =speedMuveHorse;
        _damage= damageFastZombie;
        _xp = xpFastZombie;
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void TakeDamage(float damage)
    {
        if (xpHorse - damage > 0)
        {
            xpHorse -= damage;
            Debug.Log("Урона по бране " + damage);
        }
        else
        {
            xpHorse=-1;
            _speedMuve =speedMuveFastZombie;
            base.TakeDamage(damage);
            Debug.Log("нанесенный урон " + damage);
        }
    }
}