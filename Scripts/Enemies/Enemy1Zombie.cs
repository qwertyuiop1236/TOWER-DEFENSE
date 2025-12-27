using UnityEngine;
using UnityEngine.UI;

public class EnemyZombie : Enemy
{
    [Header("Настройка уникальных параметров Zombie")]
    [SerializeField] protected float maxXpZombie;
    [SerializeField] protected float xpZombie = 200f;

    [SerializeField] protected float maxAromorZomdie;
    [SerializeField] protected float armorZombie = 0;

    [SerializeField] protected bool hasShield = false;
    [SerializeField] protected float shieldDamageMultiplier = 1f;

    [Header("UI элементы для отображения параметров")]
    [SerializeField] protected Image BarXP;
    [SerializeField] protected Image BarArmor;
    
    // Для управления скоростью
    private float originalSpeed;
    private bool isBoosted = false;

    private float BarRefreshTimeMax = 0.2f;
    private float BarRefreshTime = 0;
    private bool DamageRegistration = false;

    // доступные переменыне

    public float _Armor => _armor;
    
    protected virtual void Awake()
    {
        BarRefreshTime += BarRefreshTimeMax;
        originalSpeed = _speedMuve;
        _xp = xpZombie;
        _armor = armorZombie;
    }


    protected override void Start()
    {
        base.Start();
        maxAromorZomdie += _armor;
        maxXpZombie += _xp;
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
            BarXP.fillAmount = _xp / xpZombie;
            BarArmor.fillAmount = _armor / maxAromorZomdie;
            
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
                Debug.Log("Урона по бране " + damage);
            }
            else
            {
                base.TakeDamage(damage - _armor);
                _armor=0;
                Debug.Log("нанесенный урон " + damage);
                
            }
        }
    }
    
    
    // Методы для управления скоростью
    public void ApplySpeedBoost(float multiplier)
    {
        if (!isBoosted)
        {
            isBoosted = true;
            _speedMuve = originalSpeed * multiplier;
        }
    }

    public void ResetSpeed()
    {
        if (isBoosted)
        {
            isBoosted = false;
            _speedMuve = originalSpeed;
        }
    }

    
    // Методы для управления щитом
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


    public void AppArmor(float AppArmor)
    {
        _armor+=AppArmor;
    }


    public void ArmorZero()
    {
        _armor=0;
    }

}