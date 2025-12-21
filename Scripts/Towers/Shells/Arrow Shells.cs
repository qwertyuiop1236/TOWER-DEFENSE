using UnityEngine;

public class ArrowShells : Shells
{
    //[SerializeField] protected float _pierceChanceArrow;
    // private Enemy _target;
    private GameObject _owner;
    // Start is called before the first frame update
    public override  void Initialize(float damage, float pierceChance, GameObject owner)
    {
        base.Initialize(damage,pierceChance,owner);
    }
        void OnTriggerEnter2D(Collider2D other)
    {
        // Игнорируем владельца и другие стрелы
        if (other.gameObject == _owner || other.GetComponent<ArrowShells>()) return;
        
        // Проверяем, враг ли это
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            // Наносим урон врагу
            enemy.TakeDamage((int)_damageShells);
            
            // Эффект попадания
            
            // Проверяем пробитие
            //if (!_hasPierced && Random.value <= _pierceChance)
            //{
            //    _hasPierced = true;
            //    Debug.Log("Стрела пробила врага!");
            //    // Не уничтожаем стрелу, она летит дальше
            //    return;
            //}
            
            // Если нет пробития - уничтожаем стрелу
            Destroy(gameObject);
        }
        else
        {
            // Попадание в стену/препятствие
            Destroy(gameObject);
        }
    }
    
}
