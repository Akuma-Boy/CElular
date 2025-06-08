using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Para Text (ou use TMPro para TextMeshProUGUI)
// using TMPro;
using System.Linq; // Adicionado para suportar OrderByDescending e Take

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private const string ScoreKey = "HighScores";
    private List<ScoreEntry> scoreList = new();
    public int CurrentGameScore { get; private set; }
    [SerializeField] private Text scoreText; // Ou TextMeshProUGUI

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScores();
            ResetCurrentGameScore();
            Debug.Log("ScoreManager: Inicializado com sucesso.");
        }
        else
        {
            Debug.LogWarning("ScoreManager: Instância duplicada destruída.");
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        GameManager.OnGameStart += ResetCurrentGameScore;
    }

    private void OnDisable()
    {
        GameManager.OnGameStart -= ResetCurrentGameScore;
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
        UpdateScoreUI();
        Debug.Log("ScoreManager: Score resetado para 0.");
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = CurrentGameScore.ToString();
            Debug.Log($"ScoreManager: UI atualizada. Score={CurrentGameScore}");
        }
        else
        {
            Debug.LogWarning("ScoreManager: scoreText não atribuído no Inspector.");
        }
    }
}