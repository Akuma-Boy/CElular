using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;

public class GameOverUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject gameOverPanel;
    public InputField nickInputField;
    public Text scoreListText;
    public Text pressEnterText;
    public GameObject scoreListPanel;
    public Button submitButton;
    public Button menuButton;

    private bool readyToRestart = false;

    private void Awake()
    {
        gameOverPanel?.SetActive(false);
        pressEnterText?.gameObject.SetActive(false);
        scoreListPanel?.SetActive(false);
    }

    private void Start()
    {
        Debug.Log("GameOverUI: Iniciando. Conectando OnGameOver...");
        submitButton?.onClick.AddListener(OnSubmitNick);
        menuButton?.onClick.AddListener(ReturnToMenu);
        GameManager.OnGameOver += ShowGameOverUI;
        Debug.Log("GameOverUI: OnGameOver conectado.");
    }

    private void OnDestroy()
    {
        GameManager.OnGameOver -= ShowGameOverUI;
        submitButton?.onClick.RemoveListener(OnSubmitNick);
        menuButton?.onClick.RemoveListener(ReturnToMenu);
    }

    void Update()
    {
        if (readyToRestart && Input.GetKeyDown(KeyCode.Return))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadGameScene();
            }
            else
            {
                Debug.LogWarning("GameOverUI: GameManager.Instance não encontrado. Carregando cena diretamente.");
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    public void ShowGameOverUI()
    {
        Debug.Log("GameOverUI: ShowGameOverUI chamado!");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Debug.Log("GameOverUI: Painel ativado.");
            if (nickInputField != null)
            {
                nickInputField.Select();
                nickInputField.ActivateInputField();
                Debug.Log("GameOverUI: InputField focado.");
            }
            else
            {
                Debug.LogError("GameOverUI: nickInputField não atribuído!");
            }
        }
        else
        {
            Debug.LogError("GameOverUI: gameOverPanel não atribuído!");
        }
        Time.timeScale = 0f;
    }

    public void OnSubmitNick()
    {
        if (string.IsNullOrWhiteSpace(nickInputField.text))
        {
            Debug.LogWarning("GameOverUI: Nickname vazio, score não salvo.");
            return;
        }

        int finalScore = ScoreManager.Instance?.CurrentGameScore ?? 0;
        Debug.Log($"GameOverUI: Submetendo score. Nick={nickInputField.text}, Score={finalScore}");
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(nickInputField.text, finalScore);
            ShowScores();
        }
        else
        {
            Debug.LogError("GameOverUI: ScoreManager.Instance não encontrado!");
        }
    }

    void ShowScores()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (scoreListPanel != null) scoreListPanel.SetActive(true);
        if (pressEnterText != null) pressEnterText.gameObject.SetActive(true);

        if (ScoreManager.Instance != null && scoreListText != null)
        {
            StringBuilder builder = new StringBuilder();
            var scores = ScoreManager.Instance.GetScores();
            int maxScores = Mathf.Min(scores.Count, 10);
            for (int i = 0; i < maxScores; i++)
            {
                builder.AppendLine($"{i + 1}. {scores[i].nick} - {scores[i].score}");
            }
            scoreListText.text = builder.ToString();
        }
        readyToRestart = true;
    }

    public void RestartGame()
    {
        Debug.Log("GameOverUI: Reiniciando jogo.");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadGameScene();
        }
        else
        {
            Debug.LogError("GameOverUI: GameManager.Instance não encontrado. Carregando cena de jogo diretamente.");
            Time.timeScale = 1f;
            SceneManager.LoadScene("GameScene");
        }
    }

    public void ReturnToMenu()
    {
        Debug.Log("GameOverUI: Retornando ao menu.");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMenu();
        }
        else
        {
            Debug.LogError("GameOverUI: GameManager.Instance não encontrado. Carregando cena do menu diretamente.");
            Time.timeScale = 1f;
            SceneManager.LoadScene("Menu");
        }
    }
}