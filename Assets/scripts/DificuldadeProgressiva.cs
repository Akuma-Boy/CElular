using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class DificuldadeProgressiva : MonoBehaviour
{
    public static DificuldadeProgressiva Instance { get; private set; }

    [Header("Configurações de Dificuldade")]
    [SerializeField] private float tempoTotal = 300f; // Total time to reach max difficulty (5 minutes)
    [SerializeField] private float intervaloMinimoSpawn = 0.3f; // Faster spawn
    [SerializeField] private float intervaloMaximoSpawn = 2f;    // Slower spawn
    [SerializeField] private int quantidadeMinimaSpawn = 1;
    [SerializeField] private int quantidadeMaximaSpawn = 3;

    [SerializeField] private bool aumentarVelocidadeInimigos = true;
    [SerializeField] private float multiplicadorCrescimentoVelocidade = 2f; // How much progress affects speed increase
    [SerializeField] private float velocidadeMinimaInimigo = 2f;
    [SerializeField] private float velocidadeMaximaInimigo = 6f;

    // Novo: Configurações para o aumento da quantidade de inimigos
    [Header("Configurações de Quantidade de Inimigos")]
    [SerializeField] private bool aumentarQuantidadeInimigos = true;
    [SerializeField] private int quantidadeInimigosMinimaGlobal = 5; // Quantidade inicial de inimigos no total na tela
    [SerializeField] private int quantidadeInimigosMaximaGlobal = 20; // Quantidade máxima de inimigos no total na tela
    [SerializeField] private float multiplicadorCrescimentoQuantidade = 1.5f; // Quão rápido a quantidade total aumenta

    [Header("Configurações do Parallax")]
    [SerializeField] private bool aumentarVelocidadeParallax = true;
    [SerializeField] private float parallaxVelocidadeMinima = 1f;
    [SerializeField] private float parallaxVelocidadeMaxima = 5f;
    [SerializeField] private float multiplicadorCrescimentoParallax = 2f;

    private List<Spawner> spawners = new List<Spawner>();
    private List<ParallaxBackground> parallaxLayers = new List<ParallaxBackground>();
    private float tempoAtualJogo;
    public bool EstaAtivo { get; set; } // Esta será controlada pelo GameManager e pelo próprio script

    public float ProgressoNormalizado
    {
        get { return Mathf.Clamp01(tempoAtualJogo / (tempoTotal > 0 ? tempoTotal : 1f)); }
    }

    public float GetCurrentEnemySpeed()
    {
        if (!EstaAtivo) // Se não está ativo, retorna a velocidade mínima
        {
            Debug.LogWarning("<color=red>DificuldadeProgressiva: GetCurrentEnemySpeed called but EstaAtivo is FALSE. Returning min speed.</color>");
            return velocidadeMinimaInimigo;
        }
        float progresso = ProgressoNormalizado;
        float calculatedSpeed = Mathf.Lerp(velocidadeMinimaInimigo, velocidadeMaximaInimigo, progresso * multiplicadorCrescimentoVelocidade);
        return Mathf.Clamp(calculatedSpeed, velocidadeMinimaInimigo, velocidadeMaximaInimigo);
    }

    // Novo método para obter a quantidade atual de inimigos na tela
    public int GetCurrentEnemyQuantity()
    {
        if (!EstaAtivo)
        {
            return quantidadeInimigosMinimaGlobal;
        }
        float progresso = ProgressoNormalizado;
        float calculatedQuantity = Mathf.Lerp(quantidadeInimigosMinimaGlobal, quantidadeInimigosMaximaGlobal, progresso * multiplicadorCrescimentoQuantidade);
        return Mathf.RoundToInt(Mathf.Clamp(calculatedQuantity, quantidadeInimigosMinimaGlobal, quantidadeInimigosMaximaGlobal));
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("<color=green>DificuldadeProgressiva: Instância criada e marcada como DontDestroyOnLoad. HashCode: " + GetHashCode() + "</color>");
            EstaAtivo = false; // Começa inativo por padrão
        }
        else
        {
            Debug.LogWarning("<color=orange>DificuldadeProgressiva: Instância duplicada detectada (" + GetHashCode() + "), destruindo-a. Existing HashCode: " + Instance.GetHashCode() + "</color>");
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        Debug.Log("<color=cyan>DificuldadeProgressiva: OnEnable - Subscribing to SceneManager and GameManager events.</color>");
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Tenta se inscrever imediatamente, ou usa a coroutine se o GameManager ainda não estiver pronto
        if (GameManager.Instance != null)
        {
            GameManager.OnGameStart += OnGameStartHandler;
            GameManager.OnGameOver += OnGameOverHandler;
            Debug.Log("<color=green>DificuldadeProgressiva: Subscribed to GameManager.OnGameStart and OnGameOver (early).</color>");
        }
        else
        {
            StartCoroutine(SubscribeToGameManagerEventsDelayed());
        }
    }

    private IEnumerator SubscribeToGameManagerEventsDelayed()
    {
        while (GameManager.Instance == null)
        {
            yield return null; // Espera um frame até que o GameManager esteja disponível
        }

        GameManager.OnGameStart += OnGameStartHandler;
        GameManager.OnGameOver += OnGameOverHandler;
        Debug.Log("<color=green>DificuldadeProgressiva: Subscribed to GameManager.OnGameStart and OnGameOver (delayed).</color>");
    }

    private void OnDisable()
    {
        Debug.Log("<color=cyan>DificuldadeProgressiva: OnDisable - Unsubscribing from SceneManager and GameManager events.</color>");
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (GameManager.Instance != null)
        {
            GameManager.OnGameStart -= OnGameStartHandler;
            GameManager.OnGameOver -= OnGameOverHandler;
            Debug.Log("<color=red>DificuldadeProgressiva: Unsubscribed from GameManager.OnGameStart and OnGameOver.</color>");
        }
        else
        {
            Debug.LogWarning("<color=orange>DificuldadeProgressiva: GameManager.Instance não encontrado no OnDisable (pode já ter sido destruído).</color>");
        }
    }

    // Handler para o evento OnGameStart do GameManager
    private void OnGameStartHandler()
    {
        Debug.Log("<color=green>DificuldadeProgressiva: OnGameStartHandler chamado! Definindo EstaAtivo = TRUE e resetando tempo.</color>");
        EstaAtivo = true; // ATIVA a progressão
        tempoAtualJogo = 0f; // Reseta o tempo para o início do jogo

        // Garante que a reinicialização dos objetos da cena ocorra após o Start de todos eles.
        StartCoroutine(DelayedReinitializeSceneObjects());
    }

    private IEnumerator DelayedReinitializeSceneObjects()
    {
        // Espera um frame para garantir que todos os objetos da nova cena completaram seus Awakes e Starts.
        yield return null;

        ReinitializeSceneObjects(); // Agora é seguro encontrar e aplicar as configurações
    }

    // Handler para o evento OnGameOver do GameManager
    private void OnGameOverHandler()
    {
        Debug.Log("<color=green>DificuldadeProgressiva: OnGameOverHandler chamado! Definindo EstaAtivo = FALSE.</color>");
        EstaAtivo = false; // DESATIVA a progressão
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameManager.Instance != null && scene.name == GameManager.Instance.gameSceneName)
        {
            Debug.Log($"<color=green>DificuldadeProgressiva: Cenário de jogo '{scene.name}' carregado. Verificando estado do GameManager e ativando progressão se o jogo estiver ativo.</color>");

            // É CRÍTICO que o GameManager.IsGameActive seja TRUE neste ponto para a primeira run
            if (GameManager.Instance.IsGameActive)
            {
                // Se o jogo já está ativo (primeira run, ou reinício), reinicia e ativa
                OnGameStartHandler(); // Reusa a lógica de ativação
            }
            else
            {
                // Se a cena do jogo carregou mas o GameManager diz que o jogo NÃO está ativo
                // (ex: talvez carregou a cena do jogo diretamente no editor sem passar pelo menu)
                // então não ativa a progressão.
                EstaAtivo = false;
                Debug.LogWarning("<color=orange>DificuldadeProgressiva: Cena de jogo carregada, mas GameManager.IsGameActive é FALSE. Dificuldade permanecerá desativada por enquanto.</color>");
            }
        }
        else
        {
            // Para outras cenas (ex: Menu), garante que está desativado e limpa listas
            spawners.Clear();
            parallaxLayers.Clear();
            EstaAtivo = false;
            Debug.Log($"<color=blue>DificuldadeProgressiva: Cena '{scene.name}' carregada (Não é a cena do jogo). Dificuldade desativada. EstaAtivo: {EstaAtivo}</color>");
        }
    }

    private void ReinitializeSceneObjects()
    {
        spawners.Clear();
        parallaxLayers.Clear();

        // Usando FindObjectsByType para garantir que encontramos todos os objetos na cena atual
        spawners = FindObjectsByType<Spawner>(FindObjectsSortMode.None).ToList();
        parallaxLayers = FindObjectsByType<ParallaxBackground>(FindObjectsSortMode.None).ToList();

        Debug.Log($"<color=yellow>DificuldadeProgressiva: Re-inicializando objetos de cena. Encontrados {spawners.Count} spawners e {parallaxLayers.Count} camadas de parallax.</color>");

        if (spawners.Count == 0)
        {
            Debug.LogError("<color=red>DificuldadeProgressiva: NENHUM spawner encontrado na cena de jogo! A progressão do spawn NÃO funcionará. Verifique se estão ativos e na cena.</color>");
        }
        if (parallaxLayers.Count == 0)
        {
            Debug.LogError("<color=red>DificuldadeProgressiva: NENHUMA camada de parallax encontrada na cena de jogo! A velocidade do parallax NÃO funcionará. Verifique se estão ativos e na cena.</color>");
        }

        ApplyInitialDifficultySettings();
    }

    private void ApplyInitialDifficultySettings()
    {
        Debug.Log("<color=yellow>DificuldadeProgressiva: Aplicando configurações iniciais (Spawner, Inimigos e Parallax) *AGORA*.</color>");

        foreach (var spawner in spawners)
        {
            if (spawner != null)
            {
                spawner.intervaloDeSpawn = intervaloMaximoSpawn;
                spawner.spawnSimultaneo = quantidadeMinimaSpawn;
                // Debug.Log($"<color=yellow>  Spawner '{spawner.name}' resetado para Intervalo: {intervaloMaximoSpawn}, Quantidade: {quantidadeMinimaSpawn}</color>");
            }
            else Debug.LogWarning("<color=orange>  DificuldadeProgressiva: Spawner NULL na lista durante ApplyInitialDifficultySettings.</color>");
        }

        if (aumentarVelocidadeInimigos)
        {
            MovimentoInimigo[] inimigosExistentes = FindObjectsByType<MovimentoInimigo>(FindObjectsSortMode.None);
            foreach (var inimigo in inimigosExistentes)
            {
                if (inimigo != null)
                {
                    inimigo.velocidade = velocidadeMinimaInimigo;
                    // Debug.Log($"<color=yellow>  Inimigo '{inimigo.name}' velocidade inicial definida para: {velocidadeMinimaInimigo}</color>");
                }
                else Debug.LogWarning("<color=orange>  DificuldadeProgressiva: MovimentoInimigo NULL na lista de inimigos existentes.</color>");
            }
        }

        if (aumentarVelocidadeParallax)
        {
            foreach (var parallax in parallaxLayers)
            {
                if (parallax != null)
                {
                    parallax.ResetParallax(parallaxVelocidadeMinima);
                    // Debug.Log($"<color=yellow>  Parallax '{parallax.name}' resetado para velocidade: {parallaxVelocidadeMinima}</color>");
                }
                else Debug.LogWarning("<color=orange>  DificuldadeProgressiva: Parallax NULL na lista durante ApplyInitialDifficultySettings.</color>");
            }
        }
    }

    private void Update()
    {
        if (!EstaAtivo)
        {
            return;
        }

        tempoAtualJogo += Time.deltaTime;
        float progresso = ProgressoNormalizado;

        // --- Update Spawner settings ---
        float novoIntervalo = Mathf.Lerp(intervaloMaximoSpawn, intervaloMinimoSpawn, progresso);
        int novaQuantidadeSpawnSimultaneo = Mathf.RoundToInt(Mathf.Lerp(quantidadeMinimaSpawn, quantidadeMaximaSpawn, progresso));

        if (spawners.Count > 0)
        {
            foreach (var spawner in spawners)
            {
                if (spawner != null)
                {
                    spawner.intervaloDeSpawn = novoIntervalo;
                    spawner.spawnSimultaneo = novaQuantidadeSpawnSimultaneo;
                    // Novo: Ajusta a quantidade total de inimigos que o spawner deve tentar manter na cena.
                    if (aumentarQuantidadeInimigos)
                    {
                        spawner.quantidadeMaximaInimigosNaCena = GetCurrentEnemyQuantity();
                    }
                }
            }
        }
        else Debug.LogWarning("<color=red>DificuldadeProgressiva: NENHUM spawner na lista. Spawn não está progredindo.</color>");


        // --- Update Enemy Speed ---
        if (aumentarVelocidadeInimigos)
        {
            float novaVelocidadeInimigo = GetCurrentEnemySpeed();
            MovimentoInimigo[] inimigosAtivos = FindObjectsByType<MovimentoInimigo>(FindObjectsSortMode.None);

            if (inimigosAtivos.Length > 0)
            {
                foreach (var inimigo in inimigosAtivos)
                {
                    if (inimigo != null)
                    {
                        inimigo.velocidade = novaVelocidadeInimigo;
                    }
                }
            }
        }

        // --- Update Parallax Speed ---
        if (aumentarVelocidadeParallax)
        {
            float novaVelocidadeParallax = Mathf.Lerp(parallaxVelocidadeMinima, parallaxVelocidadeMaxima, progresso * multiplicadorCrescimentoParallax);
            novaVelocidadeParallax = Mathf.Clamp(novaVelocidadeParallax, parallaxVelocidadeMinima, parallaxVelocidadeMaxima);

            if (parallaxLayers.Count > 0)
            {
                foreach (var parallax in parallaxLayers)
                {
                    if (parallax != null)
                    {
                        parallax.SetCurrentSpeed(novaVelocidadeParallax);
                    }
                }
            }
            else Debug.LogWarning("<color=red>DificuldadeProgressiva: NENHUMA camada de parallax na lista. Velocidade do parallax não está progredindo.</color>");
        }
    }

    public void ResetarDificuldade()
    {
        tempoAtualJogo = 0f;
        EstaAtivo = false; // Garantir que está inativo ao resetar
        Debug.Log($"<color=green>DificuldadeProgressiva: Dificuldade resetada para o estado inicial. EstaAtivo: {EstaAtivo}</color>");
    }
}