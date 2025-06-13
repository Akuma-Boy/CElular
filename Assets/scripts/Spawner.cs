using UnityEngine;
using System.Linq;

public class Spawner : MonoBehaviour
{
    [Header("Configurações de Spawn")]
    public GameObject[] prefabsParaSpawnar;
    public float[] porcentagensSpawn;
    public float intervaloDeSpawn = 2f;
    public int spawnSimultaneo = 1;
    public float margemDireita = 1f;
    public float variacaoVertical = 3f;

    [Header("Configurações Avançadas")]
    public bool spawnAoIniciar = true;
    public bool spawnContinuo = true;
    public int quantidadeMaximaInimigosNaCena = 10;

    private float tempoDesdeUltimoSpawn = 0f;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        if (porcentagensSpawn == null || porcentagensSpawn.Length != Mathf.Max(0, prefabsParaSpawnar.Length - 1))
        {
            ResetarPorcentagens();
        }

        if (spawnAoIniciar)
        {
            TentarSpawnarObjetos();
            tempoDesdeUltimoSpawn = 0f;
        }
    }

    private void ResetarPorcentagens()
    {
        if (prefabsParaSpawnar == null || prefabsParaSpawnar.Length <= 1)
        {
            porcentagensSpawn = new float[0];
            return;
        }

        porcentagensSpawn = new float[prefabsParaSpawnar.Length - 1];
        float porcentagemPadrao = 1f / (prefabsParaSpawnar.Length - 1);
        for (int i = 0; i < porcentagensSpawn.Length; i++)
        {
            porcentagensSpawn[i] = porcentagemPadrao;
        }
    }

    private void Update()
    {
        if (!spawnContinuo) return;
        if (GameManager.Instance == null || !GameManager.Instance.IsGameActive) return;

        tempoDesdeUltimoSpawn += Time.deltaTime;

        if (tempoDesdeUltimoSpawn >= intervaloDeSpawn)
        {
            TentarSpawnarObjetos();
            tempoDesdeUltimoSpawn = 0f; // reseta o contador

            Debug.Log($"Spawn realizado. Intervalo atual: {intervaloDeSpawn:F2}");
        }
    }

    public void TentarSpawnarObjetos()
    {
        if (prefabsParaSpawnar == null || prefabsParaSpawnar.Length == 0)
        {
            Debug.LogWarning("Nenhum prefab atribuído para spawn!");
            return;
        }

        var inimigosNaCena = FindObjectsByType<MovimentoInimigo>(FindObjectsSortMode.None);
        int inimigosAtuaisNaCena = inimigosNaCena.Count(e => prefabsParaSpawnar.Any(p => e.name.StartsWith(p.name)));

        if (inimigosAtuaisNaCena < quantidadeMaximaInimigosNaCena)
        {
            int quantidadeParaSpawnar = Mathf.Min(spawnSimultaneo, quantidadeMaximaInimigosNaCena - inimigosAtuaisNaCena);

            for (int i = 0; i < quantidadeParaSpawnar; i++)
            {
                SpawnarInimigo();
            }
            Debug.Log($"Spawnando {quantidadeParaSpawnar} inimigos. Total na cena: {inimigosAtuaisNaCena + quantidadeParaSpawnar}/{quantidadeMaximaInimigosNaCena}");
        }
        else
        {
            Debug.Log($"Limite de inimigos atingido: {inimigosAtuaisNaCena}/{quantidadeMaximaInimigosNaCena}");
        }
    }

    private void SpawnarInimigo()
    {
        GameObject prefab = SelecionarPrefab();
        Vector3 posicaoSpawn = CalcularPosicaoDireita();
        Instantiate(prefab, posicaoSpawn, Quaternion.identity);
    }

    private GameObject SelecionarPrefab()
    {
        if (prefabsParaSpawnar.Length == 1)
            return prefabsParaSpawnar[0];

        float chanceTotal = porcentagensSpawn.Sum();
        float randomValue = Random.Range(0f, chanceTotal);
        float acumulado = 0f;

        for (int i = 0; i < porcentagensSpawn.Length; i++)
        {
            acumulado += porcentagensSpawn[i];
            if (randomValue <= acumulado)
            {
                return prefabsParaSpawnar[i + 1];
            }
        }

        return prefabsParaSpawnar[0];
    }

    private Vector3 CalcularPosicaoDireita()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return transform.position;
        }

        float posicaoY = Random.Range(0f, 1f);
        Vector3 viewportPos = new Vector3(1 + margemDireita, posicaoY, mainCamera.nearClipPlane);
        Vector3 worldPos = mainCamera.ViewportToWorldPoint(viewportPos);
        worldPos.y += Random.Range(-variacaoVertical, variacaoVertical);
        worldPos.z = 0;
        return worldPos;
    }

    public void ResetarEstado()
    {
        tempoDesdeUltimoSpawn = 0f;
    }

    public void AtualizarIntervalo(float novoIntervalo)
    {
        intervaloDeSpawn = novoIntervalo;
    }
}
