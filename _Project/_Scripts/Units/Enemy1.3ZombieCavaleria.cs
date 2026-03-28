using UnityEngine;

public class EnemyZombieCavaleria : EnemyZombie
{
    [Header("настройка уникальных параметров ZombieCavaleria")]
    [SerializeField] protected float speedMuveFastZombie = 0.75f;    
    [SerializeField] protected float speedMuveHorse = 3f;  
    [SerializeField] protected float xpFastZombie = 150f;
    [SerializeField] protected float xpHorse = 200f;

    
    protected override void Start()
    {
        _xp= xpFastZombie;
        
        base.Start();

        _speedMuve =speedMuveHorse;

        // Проигрывание звука появления лошадей
        PlaySound(0);
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
            Debug.Log("Урона по лошади" + damage);
        }
        else
        {
            if (xpHorse >= 0)
            {
                // Проигрывание звука смерти лошади
                PlaySound(3, random : true);
            }
            
            xpHorse=-1;
            _speedMuve =speedMuveFastZombie;
            base.TakeDamage(damage);
            Debug.Log("нанесенный урон " + damage);
        }
    }
}