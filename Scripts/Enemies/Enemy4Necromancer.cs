using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNecromancer : Enemy
{
    [Header("настройка уникальных параметров Tower")]
    [SerializeField] protected float speedMuveNecromancer;    
    [SerializeField] protected int damageNecromancer;
    [SerializeField] protected float xpNecromancer = 400f;
    

    [Header("Общие параметры для всех врагов")]
    [SerializeField] protected GameObject Skeleton;
    [SerializeField] protected float TimeSpavn=0;
    [SerializeField] protected float TimeSpavnMax=1;


    protected override void Start()
    {
        base.Start();

        _speedMuve =speedMuveNecromancer;
        _damage= damageNecromancer;
        _xp = xpNecromancer;

        TimeSpavn=TimeSpavnMax;
    }

    protected override void Update()
    {
        base.Update();

        TimeSpavn -= Time.deltaTime;
        if (TimeSpavn <= 0)
        {
            Spavner();
            TimeSpavn += TimeSpavnMax;
        }
    }

    protected void Spavner()
    {
        GameObject Skeleton1 = Instantiate(Skeleton,transform.position,Quaternion.identity);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        Debug.Log("нанесенный урон " + damage);
    }
}