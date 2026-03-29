using UnityEngine;

public class TowerBuildManager : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private LayerMask _buildPadLayerMask;
    
    [Header("Текущее состояние")]
    private TowerData _selectedTower;
    private GameObject _currentGhost;
    private BuildPad _hoveredPad;
    private bool _isBuildMode = false;
    
    void Update()
    {
        if (_isBuildMode)
        {
            Debug.Log($"BuildMode: TRUE, SelectedTower: {_selectedTower != null}");
            
            // 2D Raycast
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 100f, _buildPadLayerMask);
            
            Debug.Log($"2D Raycast: {hit.collider != null}, MousePos: {mousePos}");
            
            if (hit.collider != null)
            {
                Debug.Log($"Попал в: {hit.collider.name}, Layer: {hit.collider.gameObject.layer}");
            }
        }
        
        HandleBuildMode();
    }
    
    void HandleBuildMode()
    {
        if (!_isBuildMode && Camera.main == null) return;
        // 2D Raycast
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 100f, _buildPadLayerMask);
        
        // Визуальная отладка (видно в сцене)
        Debug.DrawRay(mousePos, Vector2.up * 0.1f, Color.red, 0.1f);
        
        // Ищем BuildPad под курсором
        if (hit.collider != null)
        {
            BuildPad pad = hit.collider.GetComponent<BuildPad>();
            if (pad != null)
            {
                _hoveredPad = pad;
                
                // Позиционируем призрак
                if (_currentGhost != null && _selectedTower != null)
                {
                    _currentGhost.transform.position = pad.transform.position;
                    _currentGhost.SetActive(true);

                    // Проверяем возможность строительства
                    bool canBuild = !pad.IsOccupied && 
                                   StatsSystem.Instance.Money >= _selectedTower.baseCost;
                    
                    // Меняем цвет в зависимости от доступности
                    Color ghostColor = canBuild ? Color.green : Color.red;
                    SetGhostColor(ghostColor);
                    
                    // Строим по клику
                    if (Input.GetMouseButtonDown(0) && canBuild)
                    {
                        BuildTower(pad);
                    }
                }
            }
        }
        else
        {
            // Скрываем призрак если не над BuildPad
            if (_currentGhost != null)
                _currentGhost.SetActive(false);
        }
        
        // Отмена режима по ПКМ или Escape
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelBuildMode();
        }
    }
    
    public void EnterBuildMode(TowerData towerData)
    {
        _selectedTower = towerData;
        _isBuildMode = true;
        
        // Создаем призрак
        if (_currentGhost != null) Destroy(_currentGhost);
        
        if (towerData.ghostPrefab != null)
        {
            _currentGhost = Instantiate(towerData.ghostPrefab);
            _currentGhost.SetActive(false);
        }
        
        Debug.Log($"Режим строительства 2D: {towerData.towerName}");
    }
    
    void BuildTower(BuildPad pad)
    {
        // Пробуем потратить деньги
        if (!StatsSystem.Instance.TrySpendMoney(_selectedTower.baseCost))
        {
            Debug.Log("Недостаточно денег!");
            return;
        }
        
        // Строим башню
        bool success = pad.BuildTower(_selectedTower);
        if (success)
        {
            CancelBuildMode();
            Debug.Log($"Построена {_selectedTower.towerName}");
        }
    }
    
    void CancelBuildMode()
    {
        _isBuildMode = false;
        _selectedTower = null;
        
        if (_currentGhost != null)
            Destroy(_currentGhost);
            
        Debug.Log("Режим строительства отменен");
    }
    
    void SetGhostColor(Color color)
    {
        if (_currentGhost == null) return;
        
        SpriteRenderer renderer = _currentGhost.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = new Color(color.r, color.g, color.b, 0.5f);
        }
    }


        void OnDrawGizmos()
    {
        if (_isBuildMode)
        {
            // Показывает где ищет Raycast
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(mousePos, 0.1f);
        }
    }
}