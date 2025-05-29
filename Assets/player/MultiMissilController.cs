using UnityEngine;
using System.Collections.Generic;

public class MultiMissilController : MonoBehaviour
{
    [Header("Configurações dos Mísseis")]
    public GameObject projetilPrefab;
    public float velocidadeProjetil = 5f;
    public float velocidadeRotacao = 200f;
    public float tempoVida = 5f;
    public KeyCode teclaDisparo = KeyCode.E;

    [Header("Configurações de Disparo")]
    [SerializeField] private float cooldown = 2f;
    private float ultimoDisparo = -999f;

    public float Cooldown => cooldown;
    public float UltimoDisparo => ultimoDisparo;

    private void Update()
    {
        if (Input.GetKeyDown(teclaDisparo) && Time.time > ultimoDisparo + cooldown)
        {
            DispararMisseisGuiados();
            ultimoDisparo = Time.time;
        }
    }

    private void DispararMisseisGuiados()
    {
        GameObject[] inimigos = GameObject.FindGameObjectsWithTag("Inimigo");
        if (inimigos.Length == 0) return;

        List<GameObject> alvos = new List<GameObject>();
        for (int i = 0; i < Mathf.Min(4, inimigos.Length); i++)
        {
            GameObject inimigoAleatorio;
            do
            {
                inimigoAleatorio = inimigos[Random.Range(0, inimigos.Length)];
            } while (alvos.Contains(inimigoAleatorio) && inimigos.Length > alvos.Count);

            alvos.Add(inimigoAleatorio);
        }

        foreach (GameObject alvo in alvos)
        {
            CriarMissil(alvo);
        }
    }

    private void CriarMissil(GameObject alvo)
    {
        GameObject projetil = Instantiate(projetilPrefab, transform.position, Quaternion.identity);
        MissilGuiado missil = projetil.AddComponent<MissilGuiado>();
        missil.Configurar(alvo, velocidadeProjetil, velocidadeRotacao, tempoVida);
    }
}
