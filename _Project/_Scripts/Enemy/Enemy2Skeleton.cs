using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkeleton : Enemy
{
    [Header("настройка уникальных параметров Tower")]
    [SerializeField] protected float speedMuveSkeleton;    
    [SerializeField] protected int damageSkeleton;
    [SerializeField] protected float xpSkeleton = 400f;
    
    protected override void Start()
    {
        base.Start();

        _moveSpeed =speedMuveSkeleton;
        _damage= damageSkeleton;
        _health = xpSkeleton;
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
