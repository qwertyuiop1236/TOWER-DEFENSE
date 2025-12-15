using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : Enemy
{
    [SerializeField] protected float speedMuve;    
    [SerializeField] protected int damage;
    
    protected override void Start()
    {
        _speedMuve =speedMuve;
        _damage= damage;
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
    }

}
