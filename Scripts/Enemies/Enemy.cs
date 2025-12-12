using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    // ОБЩИЕ ДЛЯ ВСЕХ ВРАГОВ
    [Header("Общие параметры для всех врагов")]
    [SerializeField] protected float _speedMuve =1f;
    [SerializeField] protected int _cost =100;
    [SerializeField ]protected float _xp=100;

    protected float _damage;


    public float Cost => _cost;
    public float XP =>_xp;

    public abstract void Attack();
    protected abstract void Move();

}
