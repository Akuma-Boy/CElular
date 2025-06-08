using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static event System.Action OnGameStart;
    public static event System.Action OnGameOver;

    public DificuldadeProgressiva dificuldadeProgressiva;
    public ScoreManager scoreManager;
    private SpawnerPowerup spawnerPowerup;

    [Header("Configurações de Cenas")]
    public string menuSceneName = "Menu";
    public string gameSceneName = "GameScene";
    public string launcherSceneName = "LauncherScene";

    public bool IsGameActive { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager: Instância criada e marcada como DontDestroyOnLoad.");
        }
        else
        {
            Debug.LogWarning("GameManager: Instância duplicada destruída.");
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        Debug.Log("GameManager: Inicializado na cena " + SceneManager.GetActiveScene().name);
        if (SceneManager.GetActiveScene().name == launcherSceneName)
        {
            Debug.Log("GameManager: LauncherScene carregada. Carregando Cena de Menu...");
            SceneManager.LoadScene(menuSceneName);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"GameManager: Cena carregada: {scene.name}");
        if (scene.name == gameSceneName)
        {
            StartNewGame();
        }
        else if (scene.name == menuSceneName)
        {
            Time.timeScale = 1f;
            IsGameActive = false;
            Debug.Log("GameManager: Retornado para a Cena de Menu. Time scale normalizado.");
            CleanUpActiveGameObjects();
        }
    }

    public void StartNewGame()
    {
        Debug.Log("GameManager: Iniciando um novo jogo...");
        Time.timeScale = 1f;
        CleanUpActiveGameObjects();

        if (dificuldadeProgressiva == null) dificuldadeProgressiva = DificuldadeProgressiva.Instance;
        if (scoreManager == null) scoreManager = ScoreManager.Instance;
        spawnerPowerup = FindAnyObjectByType<SpawnerPowerup>();

        if (dificuldadeProgressiva != null)
        {
            dificuldadeProgressiva.ResetarDificuldade();
            dificuldadeProgressiva.EstaAtivo = true;
            Debug.Log("GameManager: DificuldadeProgressiva resetada e ativada.");
        }
        else Debug.LogWarning("GameManager: DificuldadeProgressiva ausente.");

        if (scoreManager != null)
        {
            scoreManager.ResetCurrentGameScore();
            Debug.Log("GameManager: Pontuação atual do jogo resetada.");
        }
        else Debug.LogWarning("GameManager: ScoreManager ausente.");

        if (spawnerPowerup != null)
        {
            spawnerPowerup.ResetSpawner();
            Debug.Log("GameManager: SpawnerPowerup resetado.");
        }
        else Debug.LogWarning("GameManager: SpawnerPowerup não encontrado.");

        VidaNave playerVida = FindAnyObjectByType<VidaNave>();
        if (playerVida != null)
        {
            playerVida.ResetVida();
            Debug.Log("GameManager: Player Vida resetada.");
        }
        else
        {
            Debug.LogWarning("GameManager: Player VidaNave não encontrada.");
        }

        PlayerReset playerReset = FindAnyObjectByType<PlayerReset>();
        if (playerReset != null)
        {
            playerReset.ResetPlayer();
            Debug.Log("GameManager: PlayerReset chamado.");
        }
        else
        {
            Debug.LogWarning("GameManager: PlayerReset não encontrado.");
        }

        IsGameActive = true;
        OnGameStart?.Invoke();
        Debug.Log("GameManager: Novo jogo iniciado com sucesso. Evento OnGameStart invocado.");
    }

    public void GameOver()
    {
        if (!IsGameActive) return;
        Debug.Log("GameManager: Game Over!");
        IsGameActive = false;
        Time.timeScale = 0f;
        OnGameOver?.Invoke();
    }

    public void ReturnToMenu()
    {
        Debug.Log("GameManager: Solicitação de retorno ao Menu.");
        if (dificuldadeProgressiva != null)
        {
            dificuldadeProgressiva.EstaAtivo = false;
        }
        CleanUpActiveGameObjects();
        SceneManager.LoadScene(menuSceneName);
    }

    public void LoadGameScene()
    {
        Debug.Log($"GameManager: Carregando cena de jogo: {gameSceneName}");
        if (dificuldadeProgressiva != null)
        {
            dificuldadeProgressiva.ResetarDificuldade();
            Debug.Log("GameManager: Dificuldade resetada antes de carregar GameScene.");
        }
        SceneManager.LoadScene(gameSceneName);
    }

    private void CleanUpActiveGameObjects()
    {
        Debug.Log("GameManager: Limpando objetos de jogo ativos...");
        string[] tagsToClean = { "Inimigo", "Projetil", "Powerup" };
        foreach (string tag in tagsToClean)
        {
            try
            {
                GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject obj in objectsToDestroy)
                {
                    if (obj != null)
                    {
                        Debug.Log($"GameManager: Destruindo {obj.name} com a tag {tag}");
                        Destroy(obj);
                    }
                }
            }
            catch (UnityException ex)
            {
                Debug.LogError($"GameManager: Erro ao limpar objetos com a tag '{tag}': {ex.Message}.");
            }
        }
    }
}
