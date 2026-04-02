using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDemon : Enemy
{
    [Header("настройка уникальных параметров Tower")]
    [SerializeField] protected float speedMuveDemon;    
    [SerializeField] protected int damageDemon;
    [SerializeField] protected float xpDemon = 400f;
    
    protected override void Start()
    {
        base.Start();

        _moveSpeed =speedMuveDemon;
        _damage= damageDemon;
        _health = xpDemon;
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
