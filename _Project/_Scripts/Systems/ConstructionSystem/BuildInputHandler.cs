using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildInputHandler : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private LayerMask _buildPadLayerMask;
    [SerializeField] private KeyCode _undoKey = KeyCode.Z;

    private GameObject _currentGhost;
    private BuildPad _hoveredPad;

    private void Start()
    {
        // Подписка на смену режима строительства
        BuildManager.OnBuildModeChanged += OnBuildModeChanged;
    }

    private void Update()
    {
        HandleBuildMode();
        HandleUndo();
    }

    private void HandleBuildMode()
    {
        if (!BuildManager.IsBuildMode)
        {
            if (_currentGhost != null) _currentGhost.SetActive(false);
            return;
        }

        // 2D Raycast
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 100f, _buildPadLayerMask);
        BuildPad newPad = hit.collider?.GetComponent<BuildPad>();

        // Обновляем подсветку
        if (newPad != _hoveredPad)
        {
            if (_hoveredPad != null) _hoveredPad.SetHighlight(false);
            _hoveredPad = newPad;
            if (_hoveredPad != null) _hoveredPad.SetHighlight(true);
        }

        // Управление призраком
        TowerData selected = BuildManager.SelectedTower;
        if (selected != null && selected.ghostPrefab != null)
        {
            if (_currentGhost == null)
            {
                _currentGhost = Instantiate(selected.ghostPrefab);
                _currentGhost.SetActive(false);
            }

            if (_hoveredPad != null && !_hoveredPad.IsOccupied)
            {
                _currentGhost.transform.position = _hoveredPad.transform.position;
                _currentGhost.SetActive(true);
                bool canAfford = StatsSystem.Instance.Money >= selected.baseCost;
                SetGhostColor(canAfford ? Color.green : Color.red);
            }
            else
            {
                _currentGhost.SetActive(false);
            }
        }

        // Строительство по клику
        if (_hoveredPad != null && Input.GetMouseButtonDown(0))
        {
            BuildManager.TryBuildOnPad(_hoveredPad);
        }

        // Отмена режима по ПКМ или Escape
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            BuildManager.ExitBuildMode();
        }
    }

    private void HandleUndo()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(_undoKey))
        {
            BuildManager.UndoLast();
        }
    }

    private void SetGhostColor(Color color)
    {
        if (_currentGhost == null) return;
        SpriteRenderer sr = _currentGhost.GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = new Color(color.r, color.g, color.b, 0.5f);
    }

    private void OnBuildModeChanged(bool active, TowerData data)
    {
        if (!active && _currentGhost != null)
        {
            Destroy(_currentGhost);
            _currentGhost = null;
        }
    }

    private void OnDestroy()
    {
        BuildManager.OnBuildModeChanged -= OnBuildModeChanged;
        if (_currentGhost != null) Destroy(_currentGhost);
    }
}
