using UnityEngine;

public class EnemyArmoredZombie: EnemyZombie
{
    //[Header("настройка уникальных параметров ArmoredZombie")]


    
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

        base.TakeDamage(damage - _armor);

    }
}