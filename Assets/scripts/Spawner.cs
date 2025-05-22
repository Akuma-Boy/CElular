using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Configurações de Spawn")]
    public GameObject[] prefabsParaSpawnar; // Array de prefabs que podem ser spawnados
    public float intervaloDeSpawn = 2f; // Tempo entre cada spawn
    public int spawnSimultaneo = 1; // Quantos objetos spawnar de cada vez
    public float areaSpawn = 5f; // Área em torno do objeto onde pode spawnar

    [Header("Configurações Avançadas")]
    public bool spawnAoIniciar = true;
    public bool spawnContinuo = true;
    public bool spawnAleatorio = true;

    private float tempoProximoSpawn;

    private void Start()
    {
        tempoProximoSpawn = Time.time + intervaloDeSpawn;

        if (spawnAoIniciar)
        {
            SpawnarObjetos();
        }
    }

    private void Update()
    {
        if (spawnContinuo && Time.time >= tempoProximoSpawn)
        {
            SpawnarObjetos();
            tempoProximoSpawn = Time.time + intervaloDeSpawn;
        }
    }

    public void SpawnarObjetos()
    {
        if (prefabsParaSpawnar == null || prefabsParaSpawnar.Length == 0)
        {
            Debug.LogError("Nenhum prefab atribuído para spawn!");
            return;
        }

        for (int i = 0; i < spawnSimultaneo; i++)
        {
            // Seleciona prefab aleatório ou o primeiro
            GameObject prefab = spawnAleatorio ?
                prefabsParaSpawnar[Random.Range(0, prefabsParaSpawnar.Length)] :
                prefabsParaSpawnar[0];

            // Calcula posição aleatória
            Vector3 posicaoSpawn = transform.position + new Vector3(
                Random.Range(-areaSpawn, areaSpawn),
                Random.Range(-areaSpawn, areaSpawn),
                0);

            // Instancia o objeto
            Instantiate(prefab, posicaoSpawn, Quaternion.identity);
        }
    }

    // Método para spawnar um prefab específico pelo índice
    public void SpawnarPrefabEspecifico(int indice)
    {
        if (indice >= 0 && indice < prefabsParaSpawnar.Length)
        {
            Instantiate(prefabsParaSpawnar[indice], transform.position, Quaternion.identity);
        }
    }

    // Visualização da área de spawn no Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSpawn * 2, areaSpawn * 2, 0));
    }
}