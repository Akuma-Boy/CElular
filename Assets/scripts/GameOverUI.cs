using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverPanel;
    public InputField nickInputField;
    public Text scoreListText;
    public Text pressEnterText;
    public GameObject scoreListPanel;

    private bool readyToRestart = false;

    private void Start()
    {
        gameOverPanel.SetActive(false);
        pressEnterText.gameObject.SetActive(false);
        scoreListPanel.SetActive(false);

        VidaNave vida = FindFirstObjectByType<VidaNave>();
        if (vida != null)
        {
            vida.aoMorrer.AddListener(ShowGameOverUI);
        }
    }

    void Update()
    {
        if (readyToRestart && Input.GetKeyDown(KeyCode.Return))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void ShowGameOverUI()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnSubmitNick()
    {
        string nick = nickInputField.text;
        if (string.IsNullOrWhiteSpace(nick)) return;

        int score = Mathf.FloorToInt(ScoreOverTime.Instance.GetCurrentScore());
        ScoreManager.Instance.AddScore(nick, score);

        ShowScores();
    }

    void ShowScores()
    {
        gameOverPanel.SetActive(false);
        scoreListPanel.SetActive(true);
        pressEnterText.gameObject.SetActive(true);

        StringBuilder builder = new();
        var scores = ScoreManager.Instance.GetScores();
        for (int i = 0; i < scores.Count; i++)
        {
            builder.AppendLine($"{i + 1}. {scores[i].nick} - {scores[i].score}");
        }

        scoreListText.text = builder.ToString();
        readyToRestart = true;
    }
}
