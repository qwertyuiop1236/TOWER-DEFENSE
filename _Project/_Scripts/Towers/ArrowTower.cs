using System;
using UnityEngine;

public class ArrowTower : Tower
{
    [Header("Уникальные поля для арбалета")]
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _arrowSpeed;
    [SerializeField] private float _pierceChance = 0.2f;

    protected override void Start()
    {
        base.Start();
        AudioManager.Instance.PlaySound("tower_build", volume: 0.5f);
        Debug.Log("Арбалетная башня построена!");
    }

    public override void Attack()
    {
        if (_currentTarget == null) return;

        GameObject arrow = ProjectileFactory.Create(_arrowPrefab, _firePoint.position, Quaternion.identity);
        Vector3 direction = (_currentTarget.transform.position - _firePoint.position).normalized;
        arrow.transform.right = direction;

        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = direction * _arrowSpeed;

        AudioManager.Instance.PlaySound("arrow_shoot", randomPitch: true);
        arrow.GetComponent<ProjectileBase>().Initialize(_damage, _pierceChance, gameObject);
        ResetAttackTimer();
        Debug.Log($"Арбалет стреляет! Урон: {_damage}");
    }

    public override bool Upgrade()
    {
        bool success = base.Upgrade();
        if (success)
        {
            _pierceChance += 0.15f;
            _arrowSpeed *= 1.2f;
            Debug.Log($"Арбалет улучшен! Шанс пробития: {_pierceChance:P0}");
        }
        AudioManager.Instance.PlaySound("tower_upgrade");
        return success;
    }
}