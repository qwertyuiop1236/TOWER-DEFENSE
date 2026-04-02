using System;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public event Action OnLevelComplete;
    public event Action OnLevelFailed;

    [SerializeField] private WaveController waveController;

    private int levelIndex;

    public void Initialize(int index)
    {
        levelIndex = index;

        if (waveController != null)
        {
            waveController.OnAllWavesComplete += OnAllWavesComplete;
            waveController.OnBaseDestroyed += OnBaseDestroyed;
        }

        // Подписываемся на событие разрушения базы
        if (StatsSystem.Instance != null)
        {
            StatsSystem.Instance.OnHealthDepleted += OnBaseHealthDepleted;
        }
    }

    private void OnAllWavesComplete()
    {
        OnLevelComplete?.Invoke();
    }

    private void OnBaseDestroyed()
    {
        OnLevelFailed?.Invoke();
    }

    private void OnBaseHealthDepleted(int health)
    {
        if (health <= 0)
            OnLevelFailed?.Invoke();
    }

    private void OnDestroy()
    {
        if (waveController != null)
        {
            waveController.OnAllWavesComplete -= OnAllWavesComplete;
            waveController.OnBaseDestroyed -= OnBaseDestroyed;
        }
        if (StatsSystem.Instance != null)
        {
            StatsSystem.Instance.OnHealthDepleted -= OnBaseHealthDepleted;
        }
    }
}