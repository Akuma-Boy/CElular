using UnityEngine;
using System.Collections.Generic;

public class DificuldadeProgressiva : MonoBehaviour
{
    [Header("Configurações de Dificuldade")]
    public float tempoTotal = 180f;
    public float intervaloMinimo = 0.3f;
    public float intervaloMaximo = 2f;
    public int quantidadeMinima = 1;
    public int quantidadeMaxima = 5;
    public bool aumentarVelocidadeInimigos = true;
    public float velocidadeMinima = 2f;
    public float velocidadeMaxima = 6f;

    [Header("Configurações do Parallax")]
    public bool aumentarVelocidadeParallax = true;
    public float parallaxVelocidadeMinima = 1f;
    public float parallaxVelocidadeMaxima = 5f;

    [Header("Referências")]
    public List<Spawner> spawners = new List<Spawner>();
    public List<ParallaxBackground> parallaxLayers = new List<ParallaxBackground>();

    private float tempoDecorrido;
    private float progresso;

    private void Start()
    {
        // Encontra todos os spawners na cena
        var spawnersNaCena = FindObjectsByType<Spawner>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (spawnersNaCena.Length > 0)
        {
            spawners.AddRange(spawnersNaCena);
        }

        // Encontra todas as layers de parallax na cena
        var parallaxNaCena = FindObjectsByType<ParallaxBackground>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (parallaxNaCena.Length > 0)
        {
            parallaxLayers.AddRange(parallaxNaCena);
        }
    }

    private void Update()
    {
        tempoDecorrido += Time.deltaTime;
        progresso = Mathf.Clamp01(tempoDecorrido / tempoTotal);
        
        AtualizarDificuldade();
    }

    private void AtualizarDificuldade()
    {
        // Calcula valores atuais baseados no progresso
        float intervaloAtual = Mathf.Lerp(intervaloMaximo, intervaloMinimo, progresso);
        int quantidadeAtual = Mathf.RoundToInt(Mathf.Lerp(quantidadeMinima, quantidadeMaxima, progresso));
        float velocidadeAtual = Mathf.Lerp(velocidadeMinima, velocidadeMaxima, progresso);
        float parallaxVelocidadeAtual = Mathf.Lerp(parallaxVelocidadeMinima, parallaxVelocidadeMaxima, progresso);

        // Atualiza spawners de inimigos
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

        // Atualiza layers de parallax
        if (aumentarVelocidadeParallax)
        {
            foreach (var layer in parallaxLayers)
            {
                if (layer != null)
                {
                    layer.SetCurrentSpeed(parallaxVelocidadeAtual);
                }
            }
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
            }
            else
            {
                var rb = prefab.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = new Vector2(-novaVelocidade, 0);
                }
            }
        }
    }

    public void AdicionarSpawner(Spawner novoSpawner)
    {
        if (!spawners.Contains(novoSpawner))
        {
            spawners.Add(novoSpawner);
        }
    }

    public void RemoverSpawner(Spawner spawnerParaRemover)
    {
        if (spawners.Contains(spawnerParaRemover))
        {
            spawners.Remove(spawnerParaRemover);
        }
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
        if (parallaxLayers.Contains(layerParaRemover))
        {
            parallaxLayers.Remove(layerParaRemover);
        }
    }

    // Método para reiniciar a progressão da dificuldade
    public void ResetarDificuldade()
    {
        tempoDecorrido = 0f;
        progresso = 0f;
        AtualizarDificuldade();
    }
}