using UnityEngine;
using System.Collections.Generic;

public class SpawnerObjetosCenario : MonoBehaviour
{
    [Header("Configurações")]
    public List<GameObject> prefabsObjetos;
    public int quantidadeMaxima = 5;
    public float intervaloMinimoSpawn = 3f;
    public float intervaloMaximoSpawn = 8f;
    public float margemProfundidadeZ = 10f; // Distância entre objetos

    private List<GameObject> objetosAtivos = new List<GameObject>();
    private float proximoSpawn;
    private float profundidadeAtual = 0f;

    private void Start()
    {
        CalcularProximoSpawn();
    }

    private void Update()
    {
        if (Time.time >= proximoSpawn && objetosAtivos.Count < quantidadeMaxima)
        {
            SpawnarObjeto();
            CalcularProximoSpawn();
        }
    }

    private void SpawnarObjeto()
    {
        if (prefabsObjetos.Count == 0) return;

        GameObject prefab = prefabsObjetos[Random.Range(0, prefabsObjetos.Count)];
        GameObject novoObjeto = Instantiate(prefab);
        
        // Configura profundidade Z para efeito de parallax
        profundidadeAtual += margemProfundidadeZ;
        Vector3 posicao = novoObjeto.transform.position;
        posicao.z = profundidadeAtual;
        novoObjeto.transform.position = posicao;
        
        objetosAtivos.Add(novoObjeto);
    }

    private void CalcularProximoSpawn()
    {
        proximoSpawn = Time.time + Random.Range(intervaloMinimoSpawn, intervaloMaximoSpawn);
    }

    public void RemoverObjeto(GameObject objeto)
    {
        if (objetosAtivos.Contains(objeto))
        {
            objetosAtivos.Remove(objeto);
            profundidadeAtual -= margemProfundidadeZ; // Ajusta profundidade
        }
    }
}