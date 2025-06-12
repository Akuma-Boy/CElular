using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Para Text (ou use TMPro para TextMeshProUGUI)
// using TMPro; // Descomente se estiver usando TextMeshPro
using System.Linq; // Adicionado para suportar OrderByDescending e Take
using UnityEngine.SceneManagement; // Adicionado para SceneManager

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private const string ScoreKey = "HighScores";
    private List<ScoreEntry> scoreList = new();
    public int CurrentGameScore { get; private set; }

    [SerializeField] private Text scoreText; // Ou TextMeshProUGUI. DEVE SER ATRIBUÍDO NO INSPECTOR APÓS A CENA CARREGAR.

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScores();
            // ResetCurrentGameScore() não é mais chamado aqui diretamente para evitar UI warnings no Awake
            Debug.Log("ScoreManager: Instância criada e marcada como DontDestroyOnLoad.");
        }
        else
        {
            Debug.LogWarning("ScoreManager: Instância duplicada destruída.");
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameManager.OnGameStart += ResetCurrentGameScore;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameManager.OnGameStart -= ResetCurrentGameScore;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == GameManager.Instance.gameSceneName) // Check if it's the game scene
        {
            // Find the scoreText UI element in the newly loaded scene
            // This is crucial because the old one was destroyed.
            // Using FindAnyObjectByType as recommended by Unity's deprecation message.
            scoreText = FindAnyObjectByType<Text>(); // <--- CORREÇÃO AQUI!
                                                  
            // If using TextMeshPro, use: scoreText = FindAnyObjectByType<TextMeshProUGUI>();

            if (scoreText == null)
            {
                Debug.LogWarning("ScoreManager: UI Text component for score display not found in the GameScene. Please ensure it exists and is active.");
            }
            else
            {
                Debug.Log("ScoreManager: UI Text component for score display found and assigned.");
                // Immediately update the UI to show the current score (which should be 0 if ResetCurrentGameScore was just called)
                UpdateScoreUI();
            }
            // Reset score when the game scene loads for a clean start
            ResetCurrentGameScore();
        }
        else // For other scenes (like Menu), clear the reference if the UI isn't there
        {
            scoreText = null;
        }
    }


    [System.Serializable]
    public class ScoreEntry
    {
        public string nick;
        public int score;

        public ScoreEntry(string nick, int score)
        {
            this.nick = nick;
            this.score = score;
        }
    }

    public void AddScore(string nick, int score)
    {
        scoreList.Add(new ScoreEntry(nick, score));
        scoreList = scoreList.OrderByDescending(s => s.score).Take(10).ToList();
        SaveScores();
    }

    public List<ScoreEntry> GetScores() => scoreList;

    private void SaveScores()
    {
        string json = JsonUtility.ToJson(new ScoreListWrapper(scoreList));
        PlayerPrefs.SetString(ScoreKey, json);
        PlayerPrefs.Save();
    }

    private void LoadScores()
    {
        string json = PlayerPrefs.GetString(ScoreKey, "");
        if (!string.IsNullOrEmpty(json))
        {
            scoreList = JsonUtility.FromJson<ScoreListWrapper>(json).scores;
        }
    }

    [System.Serializable]
    private class ScoreListWrapper
    {
        public List<ScoreEntry> scores;
        public ScoreListWrapper(List<ScoreEntry> scores)
        {
            this.scores = scores;
        }
    }

    public void AddPoints(int points)
    {
        CurrentGameScore += points;
        UpdateScoreUI();
        Debug.Log($"ScoreManager: Adicionados {points} pontos. Score atual: {CurrentGameScore}");
    }

    public void ResetCurrentGameScore()
    {
        CurrentGameScore = 0;
        UpdateScoreUI(); // Update UI to show 0
        Debug.Log("ScoreManager: Score resetado para 0.");
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = CurrentGameScore.ToString();
            // Debug.Log($"ScoreManager: UI atualizada. Score={CurrentGameScore}"); // Keep for debugging, comment out for cleaner console
        }
        else
        {
            Debug.LogWarning("ScoreManager: scoreText não atribuído ou encontrado na cena atual.");
        }
    }
}