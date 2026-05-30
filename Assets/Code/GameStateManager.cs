using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{

    public enum GameState
    {
        Menu,
        Upgrades,
        Gameplay,
        Death
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        CurrentGameState = GameState.Menu;
    }

    private static GameState _gameState;
    
    public static GameState CurrentGameState
    {
        get => _gameState;
        set
        {
            OnStateChange?.Invoke(value);
            _gameState = value;
        }
    }

    public static event Action<GameState> OnStateChange;
    
}
