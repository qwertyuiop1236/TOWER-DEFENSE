using UnityEngine;
using System;

public enum GameState
{
    PreWave,      // ожидание начала волны (таймер)
    WaveActive,   // волна активна, враги спавнятся и движутся
    WaveComplete, // волна завершена (все враги убиты), переход к таймеру
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action<GameState> OnStateChanged;

    private GameState _currentState;

    public GameState CurrentState => _currentState;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SetState(GameState.PreWave);
    }

    public void SetState(GameState newState)
    {
        _currentState = newState;
        OnStateChanged?.Invoke(_currentState);
        Debug.Log($"Game state changed to: {_currentState}");
    }
}