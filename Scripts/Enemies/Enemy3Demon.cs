using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDemon : Enemy
{
    [SerializeField] protected float speedMuveDemon;    
    [SerializeField] protected int damageDemon;
    [SerializeField] protected float xpDemon = 400f;
    
    protected override void Start()
    {
        base.Start();

        _speedMuve =speedMuveDemon;
        _damage= damageDemon;
        _xp = xpDemon;
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
