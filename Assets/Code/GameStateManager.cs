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
        CoinCount = 0;
    }

    private static GameState _gameState;
    
    public static GameState CurrentGameState
    {
        get => _gameState;
        set
        {
            _gameState = value;
            OnStateChange?.Invoke(value);
        }
    }
    
    private static int _coinCount;
    public static int CoinCount
    {
        get => _coinCount;
        set
        {
            _coinCount = value;
            OnCoinCollected?.Invoke();
        }
    }

    public static event Action<GameState> OnStateChange;
    public static event Action OnCoinCollected;
    
}
