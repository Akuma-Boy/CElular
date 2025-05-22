using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Configura��es de Spawn")]
    public GameObject[] prefabsParaSpawnar; // Array de prefabs que podem ser spawnados
    public float intervaloDeSpawn = 2f; // Tempo entre cada spawn
    public int spawnSimultaneo = 1; // Quantos objetos spawnar de cada vez
    public float areaSpawn = 5f; // �rea em torno do objeto onde pode spawnar

    [Header("Configura��es Avan�adas")]
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
            Debug.LogError("Nenhum prefab atribu�do para spawn!");
            return;
        }

        for (int i = 0; i < spawnSimultaneo; i++)
        {
            // Seleciona prefab aleat�rio ou o primeiro
            GameObject prefab = spawnAleatorio ?
                prefabsParaSpawnar[Random.Range(0, prefabsParaSpawnar.Length)] :
                prefabsParaSpawnar[0];

            // Calcula posi��o aleat�ria
            Vector3 posicaoSpawn = transform.position + new Vector3(
                Random.Range(-areaSpawn, areaSpawn),
                Random.Range(-areaSpawn, areaSpawn),
                0);

            // Instancia o objeto
            Instantiate(prefab, posicaoSpawn, Quaternion.identity);
        }
    }

    // M�todo para spawnar um prefab espec�fico pelo �ndice
    public void SpawnarPrefabEspecifico(int indice)
    {
        if (indice >= 0 && indice < prefabsParaSpawnar.Length)
        {
            Instantiate(prefabsParaSpawnar[indice], transform.position, Quaternion.identity);
        }
    }

    // Visualiza��o da �rea de spawn no Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSpawn * 2, areaSpawn * 2, 0));
    }
}