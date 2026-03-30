using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    [SerializeField] protected float _damageShells;
    [SerializeField] protected float _pierceChanceArrow;

    protected GameObject _owner; 


    public virtual void Initialize(float damage, float pierceChance, GameObject owner)
    {
        _damageShells = damage;
        _pierceChanceArrow = pierceChance;
        _owner = owner;
    }
}