using UnityEngine;
using UnityEngine.UI;

public class ScoreOverTime : MonoBehaviour
{
    public static ScoreOverTime Instance;

    [Header("Basic Settings")]
    public float baseScorePerSecond = 10f;
    public float difficultyMultiplier = 1.5f;

    [Header("UI References")]
    public Text scoreText;
    public Text multiplierText;

    private float currentScore;
    private float currentMultiplier = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        UpdateScore();
        UpdateMultiplier();
        UpdateUI();
    }

    private void UpdateScore()
    {
        currentScore += (baseScorePerSecond * currentMultiplier) * Time.deltaTime;
    }

    private void UpdateMultiplier()
    {
        if (DificuldadeProgressiva.Instance != null)
        {
            currentMultiplier = 1 + (DificuldadeProgressiva.Instance.Progresso * (difficultyMultiplier - 1));
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {Mathf.FloorToInt(currentScore)}";
        }

        if (multiplierText != null)
        {
            multiplierText.text = $"Multiplier: x{currentMultiplier:F1}";
        }
    }

    public void AddScore(float points)
    {
        currentScore += points * currentMultiplier;
        UpdateUI();
    }

    public float GetCurrentScore() => currentScore;
    public float GetCurrentMultiplier() => currentMultiplier;
}
