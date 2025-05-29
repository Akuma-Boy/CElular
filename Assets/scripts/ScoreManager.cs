using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private const string ScoreKey = "HighScores";
    private List<ScoreEntry> scoreList = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScores();
        }
        else
        {
            Destroy(gameObject);
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
        scoreList = scoreList.OrderByDescending(s => s.score).Take(10).ToList(); // Top 10
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
}
