using System;
using UnityEngine;

public static class GameStateManager
{

    public enum GameState
    {
        Menu,
        Upgrades,
        Gameplay,
        Death
    }

    private static GameState _gameState;
    
    public static GameState CurrentGameState
    {
        get => _gameState;
        set
        {
            Debug.Log("Game state changed! " + value);
            OnStateChange?.Invoke(value);
            _gameState = value;
        }
    }

    public static event Action<GameState> OnStateChange;
    
}
