using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager _instance;
    public static GameStateManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public enum GameState
    {
        StartMenus,
        GameInProgress,
        Paused,
        GameOver
    }

    public GameState CurrentGameState { get; private set; }
    
    private void Awake()
    {
        // Wont need this just for clarification.
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            // If the singleton hasn't been initialized yet
            _instance = this;
            Initialise();
        }
    }

    private void Initialise()
    {
        CurrentGameState = GameState.GameInProgress;
    }

    public bool IsGameInProgress
    {
        get { return CurrentGameState == GameState.GameInProgress; }
    }
    
    public bool IsGameOver
    {
        get { return CurrentGameState == GameState.GameOver; }
    }

    public void PauseGameState()
    {
        if (CurrentGameState == GameState.GameInProgress)
        {
            CurrentGameState = GameState.Paused;
            Debug.Log("Game Paused");
        }
        else
        {
            Debug.LogWarning(string.Format("Cannot pause game from the state: {0}", CurrentGameState.ToString()));
        }
    }

    public void SetGameOverState()
    {
        if (CurrentGameState == GameState.GameInProgress)
        {
            CurrentGameState = GameState.GameOver;
        }
        else
        {
            Debug.LogWarning(string.Format("Cannot set game state to game over from the state: {0}", CurrentGameState.ToString()));
        }
    }
}
