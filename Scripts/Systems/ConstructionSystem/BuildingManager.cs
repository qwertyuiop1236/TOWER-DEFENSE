using UnityEngine;

public class TowerBuildManager : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private LayerMask _buildPadLayer;
    
    [Header("Текущее состояние")]
    private TowerData _selectedTower;
    private GameObject _currentGhost;
    private BuildPad _hoveredPad;
    private bool _isBuildMode = false;
    
    void Update()
    {
        if (!_isBuildMode) return;
        
        HandleBuildMode();
    }
    
    void HandleBuildMode()
    {
        Debug.Log($"BuildMode активен. Tower: {_selectedTower}");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        // Ищем BuildPad под курсором
        if (Physics.Raycast(ray, out hit, 100f, _buildPadLayer))
        {
            BuildPad pad = hit.collider.GetComponent<BuildPad>();
            if (pad != null)
            {
                Debug.Log($"Наведен на BuildPad: {hit.collider.name}");

                _hoveredPad = pad;
                
                // Позиционируем призрак
                if (_currentGhost != null)
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
             Debug.Log("Не наведен на BuildPad");
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
        _currentGhost = Instantiate(towerData.ghostPrefab);
        _currentGhost.SetActive(false);
        
        Debug.Log($"Режим строительства: {towerData.towerName}");
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
        
        MeshRenderer renderer = _currentGhost.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Material mat = renderer.material;
            mat.color = new Color(color.r, color.g, color.b, 0.5f);
        }
    }
}