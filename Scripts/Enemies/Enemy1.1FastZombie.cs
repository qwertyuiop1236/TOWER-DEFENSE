using UnityEngine;

public class EnemyFastZombie : EnemyZombie
{
    [Header("настройка уникальных параметров FastZombie")]
    [SerializeField] protected float speedMuveFastZombie = 1.5f;    
    
    protected override void Start()
    {
        base.Start();

        _speedMuve =speedMuveFastZombie;

        
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