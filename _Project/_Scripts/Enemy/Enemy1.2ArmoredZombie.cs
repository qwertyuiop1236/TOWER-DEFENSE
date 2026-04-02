using UnityEngine;

public class EnemyArmoredZombie: EnemyZombie
{  
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
    }
}