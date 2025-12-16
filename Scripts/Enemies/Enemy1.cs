using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : Enemy
{
    [SerializeField] protected float speedMuve;    
    [SerializeField] protected int damage;
    
    
    protected override void Start()
    {
        base.Start();
        _speedMuve = speedMuve;
        _damage = damage;
        
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
