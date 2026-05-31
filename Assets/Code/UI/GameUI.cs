using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class GameUI : MonoBehaviour
{

    [SerializeField] private AutoPlayer autoPlayer;
    
    private UIDocument _document;

    private void OnEnable()
    {
        _document = GetComponent<UIDocument>();
        GameStateManager.OnStateChange += GameStateChange;

        _document.rootVisualElement.Q<Button>("btn-debug-menu").clicked += () => GameStateManager.CurrentGameState = GameStateManager.GameState.Menu;
        _document.rootVisualElement.Q<Button>("btn-debug-upgrades").clicked += () => GameStateManager.CurrentGameState = GameStateManager.GameState.Upgrades;
        _document.rootVisualElement.Q<Button>("btn-debug-gameplay").clicked += () => GameStateManager.CurrentGameState = GameStateManager.GameState.Gameplay;
        _document.rootVisualElement.Q<Button>("btn-debug-death").clicked += () => GameStateManager.CurrentGameState = GameStateManager.GameState.Death;
        
        _document.rootVisualElement.Query<Button>("button-play").ForEach(b => b.clicked += OnPlayButtonClicked);
        _document.rootVisualElement.Query<Button>("button-back-to-menu").ForEach(b => b.clicked += OnMenuButtonClicked);
        _document.rootVisualElement.Query<Button>("button-upgrades").ForEach(b => b.clicked += OnUpgradesButtonClicked);
        _document.rootVisualElement.Query<Button>("button-exit").ForEach(b => b.clicked += OnExitButtonClicked);
        
        _document.rootVisualElement.Query<Button>("button-play-hover").ForEach(b => b.clicked += OnPlayButtonClicked);
        _document.rootVisualElement.Query<Button>("button-back-to-menu-hover").ForEach(b => b.clicked += OnMenuButtonClicked);
        _document.rootVisualElement.Query<Button>("button-upgrades-hover").ForEach(b => b.clicked += OnUpgradesButtonClicked);
        _document.rootVisualElement.Query<Button>("button-exit-hover").ForEach(b => b.clicked += OnExitButtonClicked);
        
        GameStateManager.OnCoinCollected += OnCoinCollected;
    }

    private void OnCoinCollected()
    {
        _document.rootVisualElement.Query<Label>("label-coin-count").ForEach(e =>
        {
            e.text = GameStateManager.CoinCount.ToString(CultureInfo.InvariantCulture);
        });
    }

    private void OnPlayButtonClicked()
    {
        autoPlayer.StartRun();
        GameStateManager.CurrentGameState = GameStateManager.GameState.Gameplay;
    }

    private void OnMenuButtonClicked()
    {
        GameStateManager.CurrentGameState = GameStateManager.GameState.Menu;
    }
    
    private void OnUpgradesButtonClicked()
    {
        GameStateManager.CurrentGameState = GameStateManager.GameState.Upgrades;
    }
    
    private void OnExitButtonClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
        #else
        Application.Quit();
        #endif
    }

    private void OnDisable()
    {
        GameStateManager.OnStateChange -= GameStateChange;
        GameStateManager.OnCoinCollected -= OnCoinCollected;
    }

    private void GameStateChange(GameStateManager.GameState obj)
    {
        _document.rootVisualElement.RemoveFromClassList("game-state-menu");
        _document.rootVisualElement.RemoveFromClassList("game-state-upgrades");
        _document.rootVisualElement.RemoveFromClassList("game-state-gameplay");
        _document.rootVisualElement.RemoveFromClassList("game-state-death");

        _document.rootVisualElement.AddToClassList($"game-state-{obj.ToString().ToLower()}");

        if (obj == GameStateManager.GameState.Death)
        {
            _document.rootVisualElement.Q<Label>("label-coins-this-run").text = GameStateManager.LastRunCoins.ToString(CultureInfo.InvariantCulture);
            var distance = autoPlayer.transform.position.z;
            _document.rootVisualElement.Q<Label>("label-distance").text = $"{distance / 1000:F2} KM";
        }
    }
    
}
