using UnityEngine;
using System.Collections.Generic;

public class DificuldadeProgressiva : MonoBehaviour
{
    [Header("Configurações de Dificuldade")]
    [SerializeField] private float tempoTotal = 180f;
    [SerializeField] private float intervaloMinimo = 0.3f;
    [SerializeField] private float intervaloMaximo = 2f;
    [SerializeField] private int quantidadeMinima = 1;
    [SerializeField] private int quantidadeMaxima = 5;
    [SerializeField] private bool aumentarVelocidadeInimigos = true;
    [SerializeField] private float velocidadeMinima = 2f;
    [SerializeField] private float velocidadeMaxima = 6f;

    [Header("Configurações do Parallax")]
    [SerializeField] private bool aumentarVelocidadeParallax = true;
    [SerializeField] private float parallaxVelocidadeMinima = 1f;
    [SerializeField] private float parallaxVelocidadeMaxima = 5f;

    [Header("Referências")]
    [SerializeField] private List<Spawner> spawners = new List<Spawner>();
    [SerializeField] private List<ParallaxBackground> parallaxLayers = new List<ParallaxBackground>();

    private float tempoDecorrido;
    private float progresso;
    private bool estaAtivo = true;

    public static DificuldadeProgressiva Instance { get; private set; }
    public float Progresso => progresso;
    public bool EstaAtivo { get => estaAtivo; set => estaAtivo = value; }

    #region Lifecycle Methods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start() => InicializarReferencias();

    private void Update()
    {
        if (!estaAtivo) return;

        AtualizarProgresso();
        AtualizarDificuldade();
    }
    #endregion

    #region Initialization
    private void InicializarReferencias()
    {
        BuscarSpawnersNaCena();
        BuscarParallaxNaCena();
    }

    private void BuscarSpawnersNaCena()
    {
        var spawnersEncontrados = FindObjectsByType<Spawner>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (spawnersEncontrados.Length > 0)
        {
            spawners.AddRange(spawnersEncontrados);
        }
    }

    private void BuscarParallaxNaCena()
    {
        var parallaxEncontrado = FindObjectsByType<ParallaxBackground>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (parallaxEncontrado.Length > 0)
        {
            parallaxLayers.AddRange(parallaxEncontrado);
        }
    }
    #endregion

    #region Difficulty Management
    private void AtualizarProgresso()
    {
        tempoDecorrido += Time.deltaTime;
        progresso = Mathf.Clamp01(tempoDecorrido / tempoTotal);
    }

    private void AtualizarDificuldade()
    {
        var (intervaloAtual, quantidadeAtual, velocidadeAtual, parallaxVelocidadeAtual) = CalcularValoresAtuais();
        AtualizarSpawners(intervaloAtual, quantidadeAtual, velocidadeAtual);
        AtualizarParallax(parallaxVelocidadeAtual);
    }

    private (float intervalo, int quantidade, float velocidade, float parallaxVelocidade) CalcularValoresAtuais()
    {
        return (
            Mathf.Lerp(intervaloMaximo, intervaloMinimo, progresso),
            Mathf.RoundToInt(Mathf.Lerp(quantidadeMinima, quantidadeMaxima, progresso)),
            Mathf.Lerp(velocidadeMinima, velocidadeMaxima, progresso),
            Mathf.Lerp(parallaxVelocidadeMinima, parallaxVelocidadeMaxima, progresso)
        );
    }

    private void AtualizarSpawners(float intervaloAtual, int quantidadeAtual, float velocidadeAtual)
    {
        foreach (var spawner in spawners)
        {
            if (spawner == null) continue;

            spawner.intervaloDeSpawn = intervaloAtual;
            spawner.spawnSimultaneo = quantidadeAtual;

            if (aumentarVelocidadeInimigos)
            {
                AtualizarVelocidadeInimigos(spawner, velocidadeAtual);
            }
        }
    }

    private void AtualizarParallax(float parallaxVelocidadeAtual)
    {
        if (!aumentarVelocidadeParallax) return;

        foreach (var layer in parallaxLayers)
        {
            layer?.SetCurrentSpeed(parallaxVelocidadeAtual);
        }
    }

    private void AtualizarVelocidadeInimigos(Spawner spawner, float novaVelocidade)
    {
        foreach (var prefab in spawner.prefabsParaSpawnar)
        {
            var movimento = prefab.GetComponent<MovimentoInimigo>();
            if (movimento != null)
            {
                movimento.velocidade = novaVelocidade;
                continue;
            }

            var rb = prefab.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(-novaVelocidade, 0);
            }
        }
    }
    #endregion

    #region Public Methods
    public void AdicionarSpawner(Spawner novoSpawner)
    {
        if (!spawners.Contains(novoSpawner))
        {
            spawners.Add(novoSpawner);
        }
    }

    public void RemoverSpawner(Spawner spawnerParaRemover)
    {
        spawners.Remove(spawnerParaRemover);
    }

    public void AdicionarParallaxLayer(ParallaxBackground novaLayer)
    {
        if (!parallaxLayers.Contains(novaLayer))
        {
            parallaxLayers.Add(novaLayer);
        }
    }

    public void RemoverParallaxLayer(ParallaxBackground layerParaRemover)
    {
        parallaxLayers.Remove(layerParaRemover);
    }

    public void ResetarDificuldade()
    {
        tempoDecorrido = 0f;
        progresso = 0f;
        AtualizarDificuldade();
    }

    public float GetIntervaloSpawnAtual() => Mathf.Lerp(intervaloMaximo, intervaloMinimo, progresso);
    public float GetVelocidadeInimigosAtual() => Mathf.Lerp(velocidadeMinima, velocidadeMaxima, progresso);
    #endregion
}
