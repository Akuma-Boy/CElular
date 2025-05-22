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

    [Header("Referências")]
    public List<Spawner> spawners = new List<Spawner>();

    private float tempoDecorrido;
    private float progresso;

    private void Start()
    {
        var spawnersNaCena = FindObjectsByType<Spawner>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (spawnersNaCena.Length > 0)
        {
            spawners.AddRange(spawnersNaCena);
        }
    }

    private void Update()
    {
        if (spawners.Count == 0) return;

        tempoDecorrido += Time.deltaTime;
        progresso = Mathf.Clamp01(tempoDecorrido / tempoTotal);
        AtualizarDificuldade();
    }

    private void AtualizarDificuldade()
    {
        float intervaloAtual = Mathf.Lerp(intervaloMaximo, intervaloMinimo, progresso);
        int quantidadeAtual = Mathf.RoundToInt(Mathf.Lerp(quantidadeMinima, quantidadeMaxima, progresso));
        float velocidadeAtual = Mathf.Lerp(velocidadeMinima, velocidadeMaxima, progresso);

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
                    // CORREÇÃO: Substituído velocity por linearVelocity
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
}