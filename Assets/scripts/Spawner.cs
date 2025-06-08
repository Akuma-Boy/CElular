using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Configurações de Spawn")]
    public GameObject[] prefabsParaSpawnar;
    public float[] porcentagensSpawn; // Porcentagens para cada prefab (exceto o primeiro)
    public float intervaloDeSpawn = 2f;
    public int spawnSimultaneo = 1;
    public float margemDireita = 1f;
    public float variacaoVertical = 3f;

    [Header("Configurações Avançadas")]
    public bool spawnAoIniciar = true;
    public bool spawnContinuo = true;

    private float tempoProximoSpawn;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        tempoProximoSpawn = Time.time + intervaloDeSpawn; 

        // Garante que o array de porcentagens tem o tamanho correto
        if (porcentagensSpawn == null || porcentagensSpawn.Length != Mathf.Max(0, prefabsParaSpawnar.Length - 1))
        {
            ResetarPorcentagens();
        }

        if (spawnAoIniciar)
        {
            SpawnarObjetos();
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
        if (spawnContinuo && Time.time >= tempoProximoSpawn && GameManager.Instance.IsGameActive)
        {
            SpawnarObjetos();
            tempoProximoSpawn = Time.time + intervaloDeSpawn;
            Debug.Log($"Spawner: Spawnando {spawnSimultaneo} inimigos. Próximo spawn em {tempoProximoSpawn}");
        }
    }

    public void SpawnarObjetos()
    {
        if (prefabsParaSpawnar == null || prefabsParaSpawnar.Length == 0)
        {
            Debug.LogWarning("Nenhum prefab atribuído para spawn!");
            return;
        }

        for (int i = 0; i < spawnSimultaneo; i++)
        {
            SpawnarInimigo();
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

        float chanceTotal = 0f;
        for (int i = 0; i < porcentagensSpawn.Length; i++)
        {
            chanceTotal += porcentagensSpawn[i];
        }

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
        float posicaoY = Random.Range(0f, 1f);
        Vector3 viewportPos = new Vector3(1 + margemDireita, posicaoY, 0);
        Vector3 worldPos = mainCamera.ViewportToWorldPoint(viewportPos);
        worldPos.y += Random.Range(-variacaoVertical, variacaoVertical);
        worldPos.z = 0;
        return worldPos;
    }

    private void OnDrawGizmosSelected()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
        {
            Vector3 spawnPoint = CalcularPosicaoDireita();
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spawnPoint, 0.5f);
            Gizmos.DrawLine(spawnPoint, spawnPoint + Vector3.left * 2f);
        }
    }

    public void ResetarEstado(float intervalo, int quantidade)
    {
        intervaloDeSpawn = intervalo;
        spawnSimultaneo = quantidade;
        if (Time.time < 1f) // Apenas inicializa no início do jogo
        {
            tempoProximoSpawn = Time.time + intervalo;
        }
        Debug.Log($"Spawner: Estado resetado. Intervalo={intervalo}, Quantidade={quantidade}, Próximo Spawn={tempoProximoSpawn}");
    }
}