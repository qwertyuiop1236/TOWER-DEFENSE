using UnityEngine;
using UnityEngine.UI;

public class EnemyZombie : Enemy
{
    [Header("Настройка уникальных параметров Zombie")]
    [SerializeField] protected bool hasShield = false;
    [SerializeField] protected float shieldDamageMultiplier = 1f;

    [Header("UI элементы")]
    [SerializeField] protected Image BarXP;
    [SerializeField] protected Image BarArmor;

    private float originalSpeed;
    private bool isBoosted = false;
    private float BarRefreshTimeMax = 0.2f;
    private float BarRefreshTime = 0;
    private bool DamageRegistration = false;

    public float _Armor => _armor;

    protected override void Awake()
    {
        BarRefreshTime += BarRefreshTimeMax;
        originalSpeed = _moveSpeed;
        AudioManager.Instance.PlaySound("zombie_damage", randomPitch: true, position: transform.position);
    }

    protected override void Start()
    {
        base.Start();
        _maxHealth = _health;
        _maxArmor = _armor;
    }

    protected override void Update()
    {
        base.Update();
        UI_Update();
    }

    protected virtual void UI_Update()
    {
        BarRefreshTime -= Time.deltaTime;
        if (BarRefreshTime <= 0 && DamageRegistration)
        {
            BarXP.fillAmount = _health / _maxHealth;
            BarArmor.fillAmount = _armor / _maxArmor;
            BarRefreshTime += BarRefreshTimeMax;
            DamageRegistration = false;
        }
    }

    public override void TakeDamage(float damage)
    {
        DamageRegistration = true;
        if (hasShield)
        {
            damage *= shieldDamageMultiplier;
        }
        else
        {
            if (_armor - damage > 0)
            {
                _armor -= damage;
                AudioManager.Instance.PlaySound("zombie_armor_hit", volume: 0.3f, randomPitch: true, position: transform.position);
                Debug.Log("Урона по броне " + damage);
            }
            else
            {
                base.TakeDamage(damage - _armor);
                _armor = 0;
                Debug.Log("нанесенный урон " + damage);
            }
        }
    }

    protected override void Death()
    {
        base.Death();
        AudioManager.Instance.PlaySound("zombie_death", randomPitch: true, position: transform.position);
    }

    public void ApplySpeedBoost(float multiplier)
    {
        if (!isBoosted)
        {
            isBoosted = true;
            _moveSpeed = originalSpeed * multiplier;
        }
    }

    public void ResetSpeed()
    {
        if (isBoosted)
        {
            isBoosted = false;
            _moveSpeed = originalSpeed;
        }
    }

    public void ApplyShield(float damageReduction, float duration)
    {
        hasShield = true;
        shieldDamageMultiplier = damageReduction;
    }

    public void RemoveShield()
    {
        hasShield = false;
        shieldDamageMultiplier = 1f;
    }

    public void AddArmor(float AddArmor) => _armor += AddArmor;
    public void ArmorZero() => _armor = 0;
}