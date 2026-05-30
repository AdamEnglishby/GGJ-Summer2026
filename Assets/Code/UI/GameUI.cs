using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class GameUI : MonoBehaviour
{
    
    private UIDocument _document;

    private void OnEnable()
    {
        _document = GetComponent<UIDocument>();
        GameStateManager.OnStateChange += GameStateChange;

        _document.rootVisualElement.Q<Button>("btn-debug-menu").clicked += () => GameStateManager.CurrentGameState = GameStateManager.GameState.Menu;
        _document.rootVisualElement.Q<Button>("btn-debug-upgrades").clicked += () => GameStateManager.CurrentGameState = GameStateManager.GameState.Upgrades;
        _document.rootVisualElement.Q<Button>("btn-debug-gameplay").clicked += () => GameStateManager.CurrentGameState = GameStateManager.GameState.Gameplay;
        _document.rootVisualElement.Q<Button>("btn-debug-death").clicked += () => GameStateManager.CurrentGameState = GameStateManager.GameState.Death;
        
        _document.rootVisualElement.Q<Button>("button-play").clicked += OnPlayButtonClicked;
        _document.rootVisualElement.Q<Button>("button-upgrades").clicked += OnUpgradesButtonClicked;
        _document.rootVisualElement.Q<Button>("button-exit").clicked += OnExitButtonClicked;
        
        _document.rootVisualElement.Q<Button>("button-play-hover").clicked += OnPlayButtonClicked;
        _document.rootVisualElement.Q<Button>("button-upgrades-hover").clicked += OnUpgradesButtonClicked;
        _document.rootVisualElement.Q<Button>("button-exit-hover").clicked += OnExitButtonClicked;
    }

    private void OnPlayButtonClicked()
    {
        GameStateManager.CurrentGameState = GameStateManager.GameState.Gameplay;
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
    }

    private void GameStateChange(GameStateManager.GameState obj)
    {
        _document.rootVisualElement.RemoveFromClassList("game-state-menu");
        _document.rootVisualElement.RemoveFromClassList("game-state-upgrades");
        _document.rootVisualElement.RemoveFromClassList("game-state-gameplay");
        _document.rootVisualElement.RemoveFromClassList("game-state-death");

        _document.rootVisualElement.AddToClassList($"game-state-{obj.ToString().ToLower()}");
    }
    
}
