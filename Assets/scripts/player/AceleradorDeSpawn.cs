using UnityEngine;

public class AceleradorDeSpawn : MonoBehaviour
{
    [Header("Referência ao Spawner")]
    public Spawner scriptDeSpawner;

    [Header("Configuração do Intervalo de Spawn")]
    public float intervaloInicial = 5f; // Começa bem lento
    public float reducaoPorSegundo = 0.01f;
    public float intervaloMinimo = 0.7f;

    [Header("Configuração da Quantidade Simultânea")]
    public int spawnSimultaneoInicial = 1;
    public float aumentoSimultaneoPorSegundo = 0.05f;
    public int maximoSimultaneo = 5;

    [Header("Configuração do Parallax")]
    public float velocidadeBaseParallax = 1f;
    public float aumentoVelocidadeParallaxPorSegundo = 0.25f;
    public float velocidadeMaximaParallax = 5f;

    [Header("Configuração da Velocidade dos Inimigos")]
    public float velocidadeBaseInimigo = 3f;
    public float aumentoVelocidadeInimigoPorSegundo = 0.1f;
    public float velocidadeMaximaInimigo = 10f;

    private float tempoAcumulado = 0f;

    void Start()
    {
        if (scriptDeSpawner == null)
        {
            Debug.LogError("Spawner não atribuído ao AceleradorDeSpawn.");
            enabled = false;
            return;
        }

        scriptDeSpawner.AtualizarIntervalo(intervaloInicial);
        scriptDeSpawner.spawnSimultaneo = spawnSimultaneoInicial;
    }

    void Update()
    {
        if (!GameManager.Instance || !GameManager.Instance.IsGameActive) return;

        tempoAcumulado += Time.deltaTime;

        // Atualiza o intervalo de spawn gradualmente
        float novoIntervalo = intervaloInicial - (reducaoPorSegundo * tempoAcumulado);
        novoIntervalo = Mathf.Max(intervaloMinimo, novoIntervalo);
        scriptDeSpawner.AtualizarIntervalo(novoIntervalo);

        // Atualiza o número de spawns simultâneos gradualmente
        int novoSpawnSimultaneo = Mathf.Min(
            maximoSimultaneo,
            Mathf.FloorToInt(spawnSimultaneoInicial + aumentoSimultaneoPorSegundo * tempoAcumulado)
        );
        scriptDeSpawner.spawnSimultaneo = novoSpawnSimultaneo;

        // Atualiza a velocidade do parallax
        float novaVelocidadeParallax = Mathf.Min(
            velocidadeBaseParallax + aumentoVelocidadeParallaxPorSegundo * tempoAcumulado,
            velocidadeMaximaParallax
        );

        var parallaxList = FindObjectsByType<ParallaxBackground>(FindObjectsSortMode.None);
        foreach (ParallaxBackground pb in parallaxList)
        {
            if (!pb.habilitarProgressaoAutonoma)
                pb.SetCurrentSpeed(novaVelocidadeParallax);
        }

        // Atualiza a velocidade dos inimigos
        float novaVelocidadeInimigo = Mathf.Min(
            velocidadeBaseInimigo + aumentoVelocidadeInimigoPorSegundo * tempoAcumulado,
            velocidadeMaximaInimigo
        );

        var inimigos = FindObjectsByType<MovimentoInimigo>(FindObjectsSortMode.None);
        foreach (MovimentoInimigo inimigo in inimigos)
        {
            inimigo.SetVelocidade(novaVelocidadeInimigo);
        }

        Debug.Log($"Tempo: {tempoAcumulado:F1}s | Intervalo: {novoIntervalo:F2}s | Simultâneo: {novoSpawnSimultaneo} | Parallax: {novaVelocidadeParallax:F2} | Vel Inimigo: {novaVelocidadeInimigo:F2}");
    }

    public void ResetarDificuldade()
    {
        tempoAcumulado = 0f;
        scriptDeSpawner.AtualizarIntervalo(intervaloInicial);
        scriptDeSpawner.spawnSimultaneo = spawnSimultaneoInicial;
    }
}
