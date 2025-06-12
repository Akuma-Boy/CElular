using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
using System.Linq; // Adicionado para usar Where e FirstOrDefault

public class GameOverUI : MonoBehaviour
{
    [Header("UI Elements (Assign manually if possible, or ensure correct naming for auto-assign)")]
    // Manter SerializeField para depuração e atribuição inicial no Inspector.
    [SerializeField] private GameObject gameOverPanel; 
    [SerializeField] private InputField nickInputField;
    [SerializeField] private Text scoreListText;
    [SerializeField] private Text pressEnterText;
    [SerializeField] private GameObject scoreListPanel;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button menuButton;

    private bool readyToRestart = false;

    // Constante para o nome do painel principal de Game Over
    private const string GAME_OVER_PANEL_NAME = "GameOverPanel"; // <- VERIFIQUE ESTE NOME NA SUA HIERARQUIA EXATAMENTE!
    private const string SCORE_LIST_PANEL_NAME = "ScoreListPanel"; // <- VERIFIQUE ESTE NOME EXATAMENTE!

    private void Awake()
    {
        Debug.Log("<color=cyan>[GameOverUI] Awake called</color>");
        
        // Principal mudança: A função de busca e atribuição.
        // É crucial que esta função seja chamada e que o painel seja encontrado.
        FindAndAssignUIElements(); 
        
        InitializeUI();
    }

    private void OnEnable()
    {
        Debug.Log("<color=green>[GameOverUI] OnEnable - Registrando eventos</color>");
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        Debug.Log("<color=red>[GameOverUI] OnDisable - Removendo eventos</color>");
        UnsubscribeFromEvents();
    }

    // FUNÇÃO MELHORADA: Encontrar e atribuir todos os elementos de UI.
    private void FindAndAssignUIElements()
    {
        // 1. Encontrar o painel principal de Game Over
        // A busca é feita em todos os GameObjects raiz da cena, ativos ou inativos.
        if (gameOverPanel == null)
        {
            // Pega todos os GameObjects raiz na cena atual.
            GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            
            // Procura o GameObject pelo nome, incluindo inativos.
            GameObject foundPanel = rootObjects.Where(obj => obj.name == GAME_OVER_PANEL_NAME)
                                              .FirstOrDefault();
            
            if (foundPanel != null)
            {
                gameOverPanel = foundPanel;
                Debug.Log($"<color=lime>[GameOverUI] gameOverPanel '{GAME_OVER_PANEL_NAME}' encontrado automaticamente na cena (ativo ou inativo).</color>");
            }
            else
            {
                // Este é o log que você está vendo e que não pode acontecer.
                Debug.LogError($"<color=red>[GameOverUI] FATAL: gameOverPanel é NULL e não foi encontrado pelo nome '{GAME_OVER_PANEL_NAME}' nos objetos raiz da cena!</color> " +
                               "Verifique se o GameObject do painel de Game Over existe na cena e tem o nome exato: '" + GAME_OVER_PANEL_NAME + "'.");
                return; // Se não encontrar o painel principal, não podemos continuar.
            }
        }
        
        // 2. Encontrar os outros elementos de UI como filhos do gameOverPanel
        // A busca aqui usa GetComponentInChildren(true) que encontra componentes em filhos ativos e inativos.
        if (gameOverPanel != null)
        {
            if (nickInputField == null) {
                nickInputField = gameOverPanel.GetComponentInChildren<InputField>(true); 
                if(nickInputField == null) Debug.LogWarning("[GameOverUI] InputField (nickInputField) não encontrado no painel. Certifique-se de que ele é um filho do painel.");
            }

            if (scoreListPanel == null) {
                // Busca pelo nome exato dentro dos filhos do gameOverPanel
                Transform scoreListTransform = gameOverPanel.transform.Find(SCORE_LIST_PANEL_NAME); 
                if (scoreListTransform != null) {
                    scoreListPanel = scoreListTransform.gameObject;
                } else {
                    Debug.LogWarning($"[GameOverUI] scoreListPanel não encontrado pelo nome '{SCORE_LIST_PANEL_NAME}' no painel. Atribua-o manualmente ou verifique o nome.");
                }
            }

            if (scoreListText == null && scoreListPanel != null) {
                scoreListText = scoreListPanel.GetComponentInChildren<Text>(true); 
                if(scoreListText == null) Debug.LogWarning("[GameOverUI] ScoreListText (scoreListText) não encontrado no scoreListPanel.");
            }

            if (pressEnterText == null) {
                // Tentativa mais específica para pressEnterText, já que pode haver múltiplos Texts
                // Você pode querer dar um nome único ao seu "Press Enter" Text para encontrá-lo com transform.Find()
                Text[] allTextsInPanel = (scoreListPanel != null ? scoreListPanel : gameOverPanel).GetComponentsInChildren<Text>(true);
                foreach (Text txt in allTextsInPanel) {
                    // Verifique se o nome do GameObject ou o texto do componente é o que você espera para "Press Enter"
                    if (txt.name.Contains("PressEnter") || txt.text.Contains("Pressione Enter para Reiniciar")) { // Ajuste esta condição
                        pressEnterText = txt;
                        Debug.Log("[GameOverUI] PressEnterText encontrado pelo nome/conteúdo.");
                        break;
                    }
                }
                if(pressEnterText == null) Debug.LogWarning("[GameOverUI] PressEnterText não encontrado. Atribua-o manualmente se a busca falhar.");
            }

            if (submitButton == null) {
                 // É melhor buscar pelo nome exato do botão ou atribuí-lo manualmente
                // Exemplo: submitButton = gameOverPanel.transform.Find("SeuNomeDoBotaoSubmit")?.GetComponent<Button>();
                submitButton = gameOverPanel.GetComponentInChildren<Button>(true); // Se tiver mais de um botão, pode pegar o errado.
                if(submitButton != null && submitButton.name != "MenuButton") { // Evita pegar o MenuButton se for o primeiro
                     Debug.Log("[GameOverUI] SubmitButton encontrado automaticamente.");
                } else {
                    Debug.LogWarning("[GameOverUI] SubmitButton não encontrado automaticamente. Atribua-o manualmente.");
                }
            }
            if (menuButton == null) {
                 // É melhor buscar pelo nome exato do botão ou atribuí-lo manualmente
                // Exemplo: menuButton = gameOverPanel.transform.Find("SeuNomeDoBotaoMenu")?.GetComponent<Button>();
                Button[] allButtons = gameOverPanel.GetComponentsInChildren<Button>(true);
                // Pode tentar encontrar por nome se houver muitos botões ou usar uma ordem conhecida
                if (allButtons.Length > 1) { // Assumindo que o segundo botão é o menu
                    menuButton = allButtons[1]; 
                    Debug.Log("[GameOverUI] MenuButton encontrado automaticamente (segundo botão).");
                } else {
                    Debug.LogWarning("[GameOverUI] MenuButton não encontrado automaticamente. Atribua-o manualmente.");
                }
            }
        } else {
             Debug.LogWarning("[GameOverUI] gameOverPanel é NULL, não foi possível tentar encontrar outros elementos UI automaticamente.");
        }
    }


    private void InitializeUI()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false); 
        if (scoreListPanel != null) scoreListPanel.SetActive(false);
        if (pressEnterText != null) pressEnterText.gameObject.SetActive(false);

        // Remover e adicionar listeners para evitar que o script se inscreva múltiplas vezes
        if (submitButton != null)
        {
            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(OnSubmitNick); 
        }
        if (menuButton != null)
        {
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(ReturnToMenuButton_Click);
        }
    }

    private void SubscribeToEvents()
    {
        if (GameManager.Instance != null)
        {
            GameManager.OnGameOver += ShowGameOverUI;
            GameManager.OnGameStart += HideGameOverUI;
            Debug.Log("<color=green>[GameOverUI] Inscrito nos eventos do GameManager</color>");
        }
        else
        {
            Debug.LogWarning("<color=orange>[GameOverUI] GameManager.Instance não encontrado ao tentar se inscrever</color>");
            Invoke(nameof(TrySubscribeAgain), 0.5f);
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (GameManager.Instance != null)
        {
            GameManager.OnGameOver -= ShowGameOverUI;
            GameManager.OnGameStart -= HideGameOverUI;
        }
        
        if (submitButton != null) submitButton.onClick.RemoveListener(OnSubmitNick);
        if (menuButton != null) menuButton.onClick.RemoveListener(ReturnToMenuButton_Click);
    }

    private void TrySubscribeAgain()
    {
        if (GameManager.Instance != null)
        {
            SubscribeToEvents();
        }
        else
        {
            Debug.LogError("<color=red>[GameOverUI] Ainda não conseguiu encontrar GameManager.Instance!</color>");
        }
    }

    void Update()
    {
        if (readyToRestart && Input.GetKeyDown(KeyCode.Return))
        {
            RestartGameButton_Click();
        }
    }

    public void ShowGameOverUI()
    {
        Debug.Log("<color=red>[GameOverUI] EVENTO GAME OVER RECEBIDO!</color>");
        
        // Este é o check final para garantir que o painel existe antes de tentar ativá-lo.
        if (gameOverPanel == null) 
        {
            // Este log é o que você está vendo e que não pode acontecer.
            Debug.LogError("<color=red>[GameOverUI] FATAL: gameOverPanel é NULL no momento de mostrar a UI, mesmo após todas as tentativas de busca!</color> O painel de Game Over não será exibido.");
            return;
        }

        gameOverPanel.SetActive(true);
        Debug.Log($"<color=green>[GameOverUI] Painel ativado. Estado atual: {gameOverPanel.activeSelf}</color>");

        if (nickInputField != null)
        {
            nickInputField.Select();
            nickInputField.ActivateInputField();
        }

        readyToRestart = false; 
    }

    private void HideGameOverUI()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (scoreListPanel != null) scoreListPanel.SetActive(false);
        if (pressEnterText != null) pressEnterText.gameObject.SetActive(false);
        readyToRestart = false;
    }

    public void OnSubmitNick()
    {
        Debug.Log("<color=magenta>GameOverUI: OnSubmitNick called.</color>");
        if (string.IsNullOrWhiteSpace(nickInputField?.text))
        {
            Debug.LogWarning("<color=orange>GameOverUI: Nickname empty or null. Score will not be saved.</color>");
            return;
        }

        int finalScore = ScoreManager.Instance?.CurrentGameScore ?? 0;
        Debug.Log($"<color=magenta>GameOverUI: Submitting score. Nick='{nickInputField.text}', Score={finalScore}</color>");
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(nickInputField.text, finalScore);
            ShowScores();
        }
        else
        {
            Debug.LogError("<color=red>GameOverUI: ScoreManager.Instance not found! Score cannot be saved.</color>");
            ShowScores(); 
        }
    }

    void ShowScores()
    {
        Debug.Log("<color=magenta>GameOverUI: ShowScores called. Transitioning to score list.</color>");
        if (gameOverPanel != null) gameOverPanel.SetActive(false); 
        if (scoreListPanel != null) scoreListPanel.SetActive(true);
        if (pressEnterText != null) pressEnterText.gameObject.SetActive(true);

        if (ScoreManager.Instance != null && scoreListText != null)
        {
            StringBuilder builder = new StringBuilder(); 
            var scores = ScoreManager.Instance.GetScores();
            int maxScores = Mathf.Min(scores.Count, 10); 
            builder.AppendLine("Top Scores:\n");
            for (int i = 0; i < maxScores; i++)
            {
                builder.AppendLine($"{i + 1}. {scores[i].nick} - {scores[i].score}");
            }
            if (scores.Count == 0)
            {
                builder.AppendLine("No scores yet. Be the first!");
            }
            scoreListText.text = builder.ToString();
            Debug.Log("<color=magenta>GameOverUI: Score list updated.</color>");
        }
        else
        {
            Debug.LogWarning("<color=orange>GameOverUI: ScoreManager.Instance or scoreListText is null when trying to show scores. Cannot display scores.</color>");
            if (scoreListText != null) scoreListText.text = "Error: Could not load scores.";
        }
        readyToRestart = true; 
    }

    public void RestartGameButton_Click()
    {
        Debug.Log("<color=blue>GameOverUI: RestartGameButton_Click called. Requesting GameManager to reload game scene.</color>");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadGameScene();
        }
        else
        {
            Debug.LogError("<color=red>GameOverUI: GameManager.Instance not found. Cannot restart game via GameManager. Reloading directly (may not reset game state fully).</color>");
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void ReturnToMenuButton_Click()
    {
        Debug.Log("<color=blue>GameOverUI: ReturnToMenuButton_Click called. Requesting GameManager to return to menu.</color>");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMenu();
        }
        else
        {
            Debug.LogError("<color=red>GameOverUI: GameManager.Instance not found. Cannot return to menu via GameManager. Loading menu directly.</color>");
            Time.timeScale = 1f;
            SceneManager.LoadScene("Menu"); 
        }
    }
}