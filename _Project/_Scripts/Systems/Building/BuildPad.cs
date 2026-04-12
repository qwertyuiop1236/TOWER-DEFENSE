using UnityEngine;
using UnityEngine.Events;

public class BuildPad : MonoBehaviour
{
    public bool IsOccupied { get; private set; }
    public Tower CurrentTower { get; private set; }

    [Header("Визуальные элементы")]
    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _upgradeUI;

    public UnityEvent<Tower> OnTowerBuilt;
    public UnityEvent<Tower> OnTowerSelected;

    private void Start()
    {
        Collider2D collider2D = GetComponent<Collider2D>();
        if (collider2D == null)
        {
            Debug.LogError("BuildPad нужен Collider2D для 2D игры!");
        }

        // if (_highlight != null) _highlight.SetActive(false);
        if (_upgradeUI != null) _upgradeUI.SetActive(false);
    }


    public void OnPadClicked()
    {
        if (IsOccupied && CurrentTower != null)
        {
            OnTowerSelected?.Invoke(CurrentTower);
            // Здесь можно вызвать открытие UI улучшений
            // Например, TowerUpgradeUI.Instance.ShowForTower(CurrentTower, this);
        }
        else if (!IsOccupied && BuildManager.IsBuildMode)
        {
            // Строительство обрабатывается в BuildInputHandler
            // Можно просто ничего не делать
        }
    }

    public void SetActive(bool active)
    {
        // Включаем/выключаем коллайдер
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = active;
        
        // Скрываем подсветку (если она есть)
        // if (_highlight != null) _highlight.SetActive(active);
        
        // По желанию: скрываем визуал самой площадки
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = active;
    }

    public void SetTower(Tower tower)
    {
        CurrentTower = tower;
        IsOccupied = true;
        SetActive(false); // выключаем площадку
        OnTowerBuilt?.Invoke(tower);
    }

    public void ClearTower()
    {
        CurrentTower = null;
        IsOccupied = false;
        SetActive(true); // включаем обратно
    }

    public void SetHighlight(bool active)
    {
        //if (_highlight != null) _highlight.SetActive(active);
    }
}