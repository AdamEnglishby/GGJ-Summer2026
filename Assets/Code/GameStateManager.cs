using System;
using Unity.Cinemachine;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{

    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private CinemachineCamera staticCamera;
    [SerializeField] private LevelSpawner levelSpawner;
    [SerializeField] private AutoPlayer autoPlayer;
    
    private static GameStateManager _instance;
    
    public enum GameState
    {
        Menu,
        Upgrades,
        Gameplay,
        Death
    }

    private void Awake()
    {
        _instance = this;
        Application.targetFrameRate = 60;
        CurrentGameState = GameState.Menu;
        CoinCount = 0;
        LastRunCoins = 0;
    }

    private static GameState _gameState;
    
    public static GameState CurrentGameState
    {
        get => _gameState;
        set
        {
            if (_gameState == GameState.Gameplay && value != GameState.Gameplay)
            {
                LastRunCoins = CoinCount;
                TotalCoins += CoinCount;
            }

            switch (value)
            {
                case GameState.Menu:
                case GameState.Upgrades:
                    _instance.autoPlayer.ResetPlayer();
                    if (_instance.levelSpawner.NeedsReset)
                    {
                        _instance.levelSpawner.ResetLevel();
                    }
                    _instance.staticCamera.Priority.Value = 1;
                    _instance.playerCamera.Priority.Value = 0;
                    break;
                case GameState.Gameplay:
                    CoinCount = 0;
                    _instance.staticCamera.Priority.Value = 0;
                    _instance.playerCamera.Priority.Value = 1;
                    break;
                case GameState.Death:
                    _instance.staticCamera.Priority.Value = 0;
                    _instance.playerCamera.Priority.Value = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
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

    public static int LastRunCoins { get; private set; }
    public static int TotalCoins { get; private set; }

    public static event Action<GameState> OnStateChange;
    public static event Action OnCoinCollected;
    
}
