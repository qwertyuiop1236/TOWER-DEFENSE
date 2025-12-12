using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : Enemy
{
    [SerializeField] private Transform PositionSpavner;
    [SerializeField] private float x=10f;
    [SerializeField] private float y=0f;
    [SerializeField] private float z=0f;

    [SerializeField] private Vector3 Vector3Move;


    

    private void Update()
    {
        Move();
    }

    protected override void Move()
    {
        Vector3Move=new Vector3(x,y,z);
        transform.position= transform.position - Vector3Move*(_speedMuve * Time.deltaTime);
    }

    public override void Attack()
    {
        
    }
}
