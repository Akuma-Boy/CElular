using UnityEngine;

public class SpawnerPowerup : MonoBehaviour
{
    [Header("Configuração")]
    public GameObject[] powerupsPrefabs;
    [Range(0, 100)] public int[] chancesDeSpawn; // Porcentagem por prefab
    public float intervalo = 5f;
    public Vector2 areaSpawnMin = new Vector2(-5f, -3f); // Ajuste no Inspector
    public Vector2 areaSpawnMax = new Vector2(5f, 3f);   // Ajuste no Inspector

    private float tempoProximoSpawn;

    private void Start()
    {
        // Verifica se chancesDeSpawn está configurado corretamente
        if (powerupsPrefabs.Length == 0 || chancesDeSpawn.Length != powerupsPrefabs.Length)
        {
            Debug.LogError($"SpawnerPowerup: Configuração inválida. Powerups={powerupsPrefabs.Length}, Chances={chancesDeSpawn.Length}. Desativando spawner.");
            enabled = false;
            return;
        }

        ResetSpawner();
        Debug.Log($"SpawnerPowerup: Inicializado com intervalo={intervalo}, powerups={powerupsPrefabs.Length}");
    }

    void Update()
    {
        if (!GameManager.Instance.IsGameActive) return; // Só spawna se o jogo estiver ativo

        if (Time.time >= tempoProximoSpawn)
        {
            TentarSpawn();
            tempoProximoSpawn = Time.time + intervalo;
            Debug.Log($"SpawnerPowerup: Tentando spawn em t={Time.time}");
        }
    }

    void TentarSpawn()
    {
        int sorteio = Random.Range(0, 100);
        int acumulado = 0;

        for (int i = 0; i < chancesDeSpawn.Length; i++)
        {
            acumulado += chancesDeSpawn[i];
            if (sorteio < acumulado)
            {
                Vector3 cameraPos = Camera.main.transform.position;
                Vector3 posicaoAleatoria = new Vector3(
                    Random.Range(cameraPos.x + areaSpawnMin.x, cameraPos.x + areaSpawnMax.x),
                    Random.Range(areaSpawnMin.y, areaSpawnMax.y),
                    0f // Z fixado em 0
                );

                Instantiate(powerupsPrefabs[i], posicaoAleatoria, Quaternion.identity);
                Debug.Log($"SpawnerPowerup: Spawnado {powerupsPrefabs[i].name} em {posicaoAleatoria}");
                break;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = (Vector3)((areaSpawnMin + areaSpawnMax) / 2f) + new Vector3(Camera.main.transform.position.x, 0f, 0f);
        Vector3 size = new Vector3(
            Mathf.Abs(areaSpawnMax.x - areaSpawnMin.x),
            Mathf.Abs(areaSpawnMax.y - areaSpawnMin.y),
            0f
        );
        Gizmos.DrawWireCube(center, size);
    }

    public void ResetSpawner()
    {
        tempoProximoSpawn = Time.time + intervalo;
        Debug.Log($"SpawnerPowerup: Resetado. Próximo spawn em t={tempoProximoSpawn}");
    }
}