using UnityEngine;

public class SpawnerPowerup : MonoBehaviour
{
    [Header("Configuração")]
    public GameObject[] powerupsPrefabs;
    [Range(0, 100)] public int[] chancesDeSpawn; // Porcentagem por prefab
    public float intervalo = 5f;
    public Vector2 areaSpawnMin; // canto inferior esquerdo
    public Vector2 areaSpawnMax; // canto superior direito

    private float tempoProximoSpawn;

    void Update()
    {
        if (Time.time >= tempoProximoSpawn)
        {
            TentarSpawn();
            tempoProximoSpawn = Time.time + intervalo;
        }
    }

void TentarSpawn()
{
    if (powerupsPrefabs.Length == 0 || chancesDeSpawn.Length != powerupsPrefabs.Length) return;

    int sorteio = Random.Range(0, 100);
    int acumulado = 0;

    for (int i = 0; i < chancesDeSpawn.Length; i++)
    {
        acumulado += chancesDeSpawn[i];
        if (sorteio < acumulado)
        {
            // Pega a posição da câmera
            Vector3 cameraPos = Camera.main.transform.position;

            // Gera uma posição à frente da câmera
            Vector2 posicaoAleatoria = new Vector2(
                Random.Range(cameraPos.x + areaSpawnMin.x, cameraPos.x + areaSpawnMax.x),
                Random.Range(areaSpawnMin.y, areaSpawnMax.y)
            );

            Instantiate(powerupsPrefabs[i], posicaoAleatoria, Quaternion.identity);
            break;
        }
    }
}


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = (areaSpawnMin + areaSpawnMax) / 2f;
        Vector3 size = new Vector3(
            Mathf.Abs(areaSpawnMax.x - areaSpawnMin.x),
            Mathf.Abs(areaSpawnMax.y - areaSpawnMin.y),
            0f
        );
        Gizmos.DrawWireCube(center, size);
    }
}
