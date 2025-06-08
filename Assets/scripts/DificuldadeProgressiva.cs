using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class DificuldadeProgressiva : MonoBehaviour
{
    public static DificuldadeProgressiva Instance { get; private set; }

    [Header("Configurações de Dificuldade")]
    [SerializeField] private float tempoTotal = 300f; // Aumentado para 5 minutos
    [SerializeField] private float intervaloMinimoSpawn = 0.3f;
    [SerializeField] private float intervaloMaximoSpawn = 2f;
    [SerializeField] private int quantidadeMinimaSpawn = 1;
    [SerializeField] private int quantidadeMaximaSpawn = 3; // Reduzido para evitar excesso
    
    [SerializeField] private bool aumentarVelocidadeInimigos = true;
    [SerializeField] private float velocidadeMinimaInimigo = 2f;
    [SerializeField] private float velocidadeMaximaInimigo = 6f;

    [Header("Configurações do Parallax")]
    [SerializeField] private bool aumentarVelocidadeParallax = true;
    [SerializeField] private float parallaxVelocidadeMinima = 1f;
    [SerializeField] private float parallaxVelocidadeMaxima = 5f;

    private List<Spawner> spawners = new();
    private List<ParallaxBackground> parallaxLayers = new(); 
    private float tempoAtualJogo;
    public bool EstaAtivo { get; set; }

    public float ProgressoNormalizado
    {
        get { return Mathf.Clamp01(tempoAtualJogo / tempoTotal); }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("DificuldadeProgressiva: Instância criada e marcada como DontDestroyOnLoad.");
        }
        else
        {
            Debug.LogWarning("DificuldadeProgressiva: Instância duplicada destruída.");
            Destroy(gameObject);
            return;
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
        if (GameManager.Instance != null && scene.name == GameManager.Instance.gameSceneName)
        {
            spawners = FindObjectsByType<Spawner>(FindObjectsSortMode.None).ToList();
            parallaxLayers = FindObjectsByType<ParallaxBackground>(FindObjectsSortMode.None).ToList();
            Debug.Log($"DificuldadeProgressiva: Encontrados {spawners.Count} Spawners e {parallaxLayers.Count} ParallaxBackgrounds na cena '{scene.name}'.");
        }
        else if (GameManager.Instance == null)
        {
            Debug.LogError("DificuldadeProgressiva: GameManager.Instance não encontrado.");
            EstaAtivo = false;
        }
        else
        {
            EstaAtivo = false;
            Debug.Log("DificuldadeProgressiva: Não na cena de jogo. EstaAtivo = false.");
        }
    }

    private void Update()
    {
        if (!EstaAtivo) return;

        tempoAtualJogo += Time.deltaTime;
        float progresso = ProgressoNormalizado;
        float novoIntervalo = Mathf.Lerp(intervaloMaximoSpawn, intervaloMinimoSpawn, progresso);
        int novaQuantidade = Mathf.RoundToInt(Mathf.Lerp(quantidadeMinimaSpawn, quantidadeMaximaSpawn, progresso));

        foreach (var spawner in spawners)
        {
            spawner.intervaloDeSpawn = novoIntervalo;
            spawner.spawnSimultaneo = novaQuantidade;
        }

        Debug.Log($"DificuldadeProgressiva: Progresso={progresso:F2}, Intervalo={novoIntervalo:F2}, Quantidade={novaQuantidade}");

        if (aumentarVelocidadeInimigos)
        {
            float novaVelocidadeInimigo = Mathf.Lerp(velocidadeMinimaInimigo, velocidadeMaximaInimigo, progresso);
            // Lógica para ajustar a velocidade dos inimigos
        }

        if (aumentarVelocidadeParallax)
        {
            float novaVelocidadeParallax = Mathf.Lerp(parallaxVelocidadeMinima, parallaxVelocidadeMaxima, progresso);
            foreach (var parallax in parallaxLayers)
            {
                parallax.SetCurrentSpeed(novaVelocidadeParallax);
            }
        }
    }

    public void ResetarDificuldade()
    {
        tempoAtualJogo = 0f;
        EstaAtivo = false;
        
        foreach (var spawner in spawners)
        {
            spawner.ResetarEstado(intervaloMaximoSpawn, quantidadeMinimaSpawn);
        }
        if (aumentarVelocidadeParallax)
        {
            foreach (var parallax in parallaxLayers)
            {
                parallax.SetCurrentSpeed(parallaxVelocidadeMinima);
            }
        }
        Debug.Log("DificuldadeProgressiva: Dificuldade resetada para o estado inicial.");
    }
}