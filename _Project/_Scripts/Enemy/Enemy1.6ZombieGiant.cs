using UnityEngine;

public class EnemyZombieGiant : EnemyZombie
{
    //[Header("настройка уникальных параметров ZombieGiant")]

    
    protected override void Start()
    {
        
        base.Start();

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