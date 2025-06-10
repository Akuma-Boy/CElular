using UnityEngine;

public class ScoreOverTime : MonoBehaviour
{
    public static ScoreOverTime Instance { get; private set; }

    [Header("Configurações de Pontuação por Tempo")]
    [SerializeField] private float intervaloAdicaoPontos = 1f;
    [SerializeField] private int pontosPorIntervaloBase = 10;
    [SerializeField] private int pontosMaximosBonusDificuldade = 50;
    [SerializeField] private float tempoProximaAdicao;

    [Header("Referências")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameObject scoreListPanel; // Adicionado para referenciar o painel

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("ScoreOverTime: Instância criada.");
        }
        else
        {
            Debug.LogWarning("ScoreOverTime: Instância duplicada destruída.");
            Destroy(gameObject);
            return;
        }

        if (scoreManager == null)
        {
            scoreManager = ScoreManager.Instance;
            if (scoreManager == null)
            {
                Debug.LogError("ScoreOverTime: ScoreManager não encontrado! Desativando script.");
                enabled = false;
            }
            else
            {
                Debug.Log("ScoreOverTime: ScoreManager encontrado com sucesso.");
            }
        }
    }

    private void OnEnable()
    {
        tempoProximaAdicao = Time.time + intervaloAdicaoPontos;
        Debug.Log("ScoreOverTime: Ativado. Próxima adição de pontos em: " + tempoProximaAdicao);
    }

    void Update()
    {
        if (!GameManager.Instance.IsGameActive || scoreManager == null || (scoreListPanel != null && scoreListPanel.activeSelf))
        {
            Debug.LogWarning($"ScoreOverTime: Bloqueado. IsGameActive={GameManager.Instance?.IsGameActive}, ScoreManager={(scoreManager != null)}, ScoreListPanel ativo={(scoreListPanel != null ? scoreListPanel.activeSelf : false)}");
            return;
        }

        if (Time.time >= tempoProximaAdicao)
        {
            AdicionarPontos();
            tempoProximaAdicao = Time.time + intervaloAdicaoPontos;
        }
    }
    private void AdicionarPontos()
    {
        int pontosAtuais = pontosPorIntervaloBase;

        if (DificuldadeProgressiva.Instance != null)
        {
            float progressoDificuldade = DificuldadeProgressiva.Instance.ProgressoNormalizado;
            int pontosBonus = Mathf.RoundToInt(pontosMaximosBonusDificuldade * progressoDificuldade);
            pontosAtuais += pontosBonus;
            Debug.Log($"ScoreOverTime: Progresso Dificuldade: {progressoDificuldade:F2}, Bônus de Pontos: {pontosBonus}");
        }
        else
        {
            Debug.LogWarning("ScoreOverTime: DificuldadeProgressiva.Instance não encontrado. Pontos de bônus não aplicados.");
        }

        scoreManager.AddPoints(pontosAtuais);
        Debug.Log($"ScoreOverTime: Adicionando {pontosAtuais} pontos por tempo. Score total: {scoreManager.CurrentGameScore}");
    }




}
