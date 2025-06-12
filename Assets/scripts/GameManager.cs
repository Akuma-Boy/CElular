using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq; // Needed for .Contains() in CleanUpActiveGameObjects

public class GameManager : MonoBehaviour
{
    // Singleton Instance
    public static GameManager Instance { get; private set; }

    // Events
    public static event System.Action OnGameStart;
    public static event System.Action OnGameOver;

    // Dependencies (Assigned in Inspector or found via Instance)
    [Header("Dependencies (Optional: Assign or rely on Instance)")]
    [SerializeField] private DificuldadeProgressiva dificuldadeProgressivaInstance;
    [SerializeField] private ScoreManager scoreManagerInstance;

    // Scene-bound dependencies (found dynamically)
    private SpawnerPowerup spawnerPowerup;
    private VidaNave playerVida;
    private PlayerReset playerReset;

    [Header("Scene Settings")]
    public string menuSceneName = "Menu";
    public string gameSceneName = "GameScene";
    public string launcherSceneName = "LauncherScene"; // Typically the very first scene loaded

    public bool IsGameActive { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("<color=lime>GameManager: Awake. Instance created and marked as DontDestroyOnLoad.</color>");
        }
        else
        {
            // If an instance already exists, destroy this duplicate
            Debug.LogWarning("<color=orange>GameManager: Awake. Duplicate instance found, destroying this one.</color>");
            Destroy(gameObject);
            return; // Prevent duplicate execution of further Awake code
        }

        // Try to get singleton references immediately if not assigned in Inspector
        if (dificuldadeProgressivaInstance == null)
        {
            dificuldadeProgressivaInstance = DificuldadeProgressiva.Instance;
            if (dificuldadeProgressivaInstance != null) Debug.Log("<color=lime>GameManager: DificuldadeProgressiva.Instance assigned via Awake.</color>");
        }
        if (scoreManagerInstance == null)
        {
            scoreManagerInstance = ScoreManager.Instance;
            if (scoreManagerInstance != null) Debug.Log("<color=lime>GameManager: ScoreManager.Instance assigned via Awake.</color>");
        }
    }

    private void Start()
    {
        Debug.Log("<color=lime>GameManager: Start called. Current scene: " + SceneManager.GetActiveScene().name + "</color>");
        // This logic handles the very first load
        if (SceneManager.GetActiveScene().name == launcherSceneName)
        {
            Debug.Log("<color=lime>GameManager: LauncherScene loaded. Loading Menu Scene...</color>");
            SceneManager.LoadScene(menuSceneName);
        }
    }

    private void OnEnable()
    {
        Debug.Log("<color=lime>GameManager: OnEnable called. Subscribing to SceneManager.sceneLoaded.</color>");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // IMPORTANT: Unsubscribe to prevent memory leaks or errors when GameManager is destroyed
        Debug.Log("<color=lime>GameManager: OnDisable called. Unsubscribing from SceneManager.sceneLoaded.</color>");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"<color=lime>GameManager: OnSceneLoaded callback. Scene loaded: {scene.name}</color>");

        // Re-find scene-bound objects on every scene load, especially for gameScene
        spawnerPowerup = FindAnyObjectByType<SpawnerPowerup>();
        if (spawnerPowerup == null && scene.name == gameSceneName) Debug.LogWarning("<color=orange>GameManager: SpawnerPowerup not found in current scene (" + scene.name + ").</color>");

        playerVida = FindAnyObjectByType<VidaNave>();
        if (playerVida == null && scene.name == gameSceneName) Debug.LogWarning("<color=orange>GameManager: Player VidaNave not found in current scene (" + scene.name + ").</color>");

        playerReset = FindAnyObjectByType<PlayerReset>();
        if (playerReset == null && scene.name == gameSceneName) Debug.LogWarning("<color=orange>GameManager: PlayerReset not found in current scene (" + scene.name + ").</color>");

        if (scene.name == gameSceneName)
        {
            StartNewGame();
        }
        else if (scene.name == menuSceneName)
        {
            Time.timeScale = 1f; // Ensure time is normal in menus
            IsGameActive = false; // Game is not active in menu
            Debug.Log("<color=lime>GameManager: Returned to Menu Scene. Time scale normalized. Cleaning up game objects...</color>");
            CleanUpActiveGameObjects(); // Clean up any lingering objects from previous game
        }
    }

    public void StartNewGame()
    {
        Debug.Log("<color=lime>GameManager: StartNewGame() initiated.</color>");
        Time.timeScale = 1f;

        // Reset game state and dependencies
        if (dificuldadeProgressivaInstance != null)
        {
            dificuldadeProgressivaInstance.ResetarDificuldade();
            dificuldadeProgressivaInstance.EstaAtivo = true; // Ensure it's active for gameplay
            Debug.Log("<color=lime>GameManager: DificuldadeProgressiva reset and activated.</color>");
        }
        else Debug.LogWarning("<color=orange>GameManager: DificuldadeProgressiva instance is null. Check GameManager Inspector or its Awake.</color>");

        if (scoreManagerInstance != null)
        {
            scoreManagerInstance.ResetCurrentGameScore();
            Debug.Log("<color=lime>GameManager: Current game score reset.</color>");
        }
        else Debug.LogWarning("<color=orange>GameManager: ScoreManager instance is null. Check GameManager Inspector or its Awake.</color>");

        if (spawnerPowerup != null)
        {
            spawnerPowerup.ResetSpawner();
            Debug.Log("<color=lime>GameManager: SpawnerPowerup reset.</color>");
        }
        else Debug.LogWarning("<color=orange>GameManager: SpawnerPowerup (scene object) is null. Make sure it exists in the scene.</color>");

        if (playerVida != null)
        {
            playerVida.ResetVida(); // Ensure player's health is reset
            Debug.Log("<color=lime>GameManager: Player Vida reset.</color>");
        }
        else Debug.LogWarning("<color=orange>GameManager: Player VidaNave (scene object) is null. Player may not be correctly set up or spawned.</color>");

        if (playerReset != null)
        {
            playerReset.ResetPlayer(); // Reset player position/state
            Debug.Log("<color=lime>GameManager: PlayerReset called.</color>");
        }
        else Debug.LogWarning("<color=orange>GameManager: PlayerReset (scene object) is null. Make sure it exists in the scene.</color>");

        IsGameActive = true;
        OnGameStart?.Invoke(); // Null-conditional operator: only invoke if not null
        Debug.Log("<color=lime>GameManager: New game successfully started. OnGameStart event invoked.</color>");
    }

    public void GameOver()
    {
        Debug.Log("<color=red>GameManager: GameOver() called.</color>");
        if (!IsGameActive)
        {
            Debug.LogWarning("<color=orange>GameManager: GameOver() called, but IsGameActive was already FALSE. Ignoring duplicate call.</color>");
            return;
        }

        Debug.Log("<color=red>GameManager: Game Over! Setting IsGameActive=false and Time.timeScale=0.</color>");
        IsGameActive = false;
        Time.timeScale = 0f;

        if (dificuldadeProgressivaInstance != null)
        {
            dificuldadeProgressivaInstance.EstaAtivo = false; // Ensure it stops progressing on game over
            Debug.Log($"<color=red>GameManager: DificuldadeProgressiva.EstaAtivo set to FALSE in GameOver.</color>");
        }

        // CRITICAL: Invoke the OnGameOver event
        OnGameOver?.Invoke();
        Debug.Log("<color=red>GameManager: OnGameOver event INVOKED!</color>");
    }

    public void ReturnToMenu()
    {
        Debug.Log("<color=blue>GameManager: Request to return to Menu.</color>");
        if (dificuldadeProgressivaInstance != null)
        {
            dificuldadeProgressivaInstance.EstaAtivo = false; // Stop difficulty progression
        }
        CleanUpActiveGameObjects(); // Clean up current scene objects
        SceneManager.LoadScene(menuSceneName);
    }

    public void LoadGameScene()
    {
        Debug.Log($"<color=blue>GameManager: Loading game scene: {gameSceneName}</color>");
        Time.timeScale = 1f; // Ensure time scale is normal before loading
        SceneManager.LoadScene(gameSceneName);
    }

    private void CleanUpActiveGameObjects()
    {
        Debug.Log("<color=blue>GameManager: Cleaning up active game objects (enemies, projectiles, powerups)...</color>");
        string[] tagsToClean = { "Inimigo", "Projetil", "Powerup" }; // Add other relevant tags if needed

        foreach (string tag in tagsToClean)
        {
            // The problematic line was here: UnityEditor.UnityEditorInternal.InternalEditorUtility.tags.Contains(tag)
            // This code block now works in builds and outside of the editor as it does not rely on Editor-only namespaces.

            GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag(tag); // This will throw an error if the tag does not exist.
            // A more robust solution for checking tag existence *without* Editor APIs:
            // You can add a try-catch for the FindGameObjectsWithTag, but it's often simpler to ensure all tags are defined.
            // Or, you can use a list of GameObjects instead of tags if possible.

            foreach (GameObject obj in objectsToDestroy)
            {
                if (obj != null)
                {
                    // Check if it's not the player itself if player also has "Projetil" tag (unlikely but good to check)
                    // Assuming player is the only one with VidaNave or a unique tag like "Player"
                    if (obj.GetComponent<VidaNave>() == null || !obj.CompareTag("Player")) // Avoid destroying player
                    {
                        Debug.Log($"<color=blue>GameManager: Destroying {obj.name} with tag {tag}</color>");
                        Destroy(obj);
                    }
                }
            }
        }
    }
}