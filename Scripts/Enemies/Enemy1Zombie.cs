using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZombie : Enemy
{
    [SerializeField] protected float speedMuveZombie;    
    [SerializeField] protected int damageZombie;
    [SerializeField] protected float xpZombie = 200f;
    
    
    protected override void Start()
    {
        base.Start();
        
        _speedMuve = speedMuveZombie;
        _damage = damageZombie;
        _xp = xpZombie;
        
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
