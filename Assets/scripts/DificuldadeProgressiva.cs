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
    [SerializeField] private float multiplicadorCrescimentoVelocidade = 2f; // Ajustável no Inspector
    [SerializeField] private float velocidadeMinimaInimigo = 2f;
    [SerializeField] private float velocidadeMaximaInimigo = 6f;


    [Header("Configurações do Parallax")]
    [SerializeField] private bool aumentarVelocidadeParallax = true;
    [SerializeField] private float parallaxVelocidadeMinima = 1f;
    [SerializeField] private float parallaxVelocidadeMaxima = 5f;
    [SerializeField] private float multiplicadorCrescimentoParallax = 2f; // Ajustável no Inspector




    private List<Spawner> spawners = new();
    private List<ParallaxBackground> parallaxLayers = new(); 
    private float tempoAtualJogo;
    public bool EstaAtivo { get; set; }


    public void IniciarDificuldade()
    {
        tempoAtualJogo = 0f;
        EstaAtivo = true;
        Debug.Log("DificuldadeProgressiva: Ativando dificuldade progressiva.");
    }



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
        spawners = FindObjectsByType<Spawner>(FindObjectsSortMode.None).ToList();
        parallaxLayers = FindObjectsByType<ParallaxBackground>(FindObjectsSortMode.None).ToList(); // Adicionado

        Debug.Log($"DificuldadeProgressiva: Encontrados {spawners.Count} spawners e {parallaxLayers.Count} camadas de parallax.");

        if (spawners.Count == 0)
        {
            Debug.LogError("DificuldadeProgressiva: Nenhum spawner encontrado! Verifique se eles estão na cena.");
        }
        if (parallaxLayers.Count == 0)
        {
            Debug.LogError("DificuldadeProgressiva: Nenhuma camada de parallax encontrada! Verifique se elas estão na cena.");
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
            float novaVelocidadeInimigo = Mathf.Lerp(velocidadeMinimaInimigo, velocidadeMaximaInimigo, progresso * multiplicadorCrescimentoVelocidade);
            MovimentoInimigo[] inimigos = FindObjectsByType<MovimentoInimigo>(FindObjectsSortMode.None);

            foreach (var inimigo in inimigos)
            {
                inimigo.velocidade = novaVelocidadeInimigo;
            }

            Debug.Log($"DificuldadeProgressiva: Velocidade dos inimigos ajustada para {novaVelocidadeInimigo:F2}");
        }


        if (aumentarVelocidadeParallax)
        {
            float novaVelocidadeParallax = Mathf.Lerp(parallaxVelocidadeMinima, parallaxVelocidadeMaxima, Mathf.Clamp(progresso * multiplicadorCrescimentoParallax, 0f, 1f));

            foreach (var parallax in parallaxLayers)
            {
                parallax.SetCurrentSpeed(novaVelocidadeParallax);
            }

            Debug.Log($"DificuldadeProgressiva: Velocidade do parallax ajustada para {novaVelocidadeParallax:F2}");
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