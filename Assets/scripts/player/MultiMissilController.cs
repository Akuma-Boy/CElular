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

        // Filtra os inimigos que estão à direita do ponto de disparo
        List<GameObject> inimigosADireita = new List<GameObject>();
        foreach (GameObject inimigo in inimigos)
        {
            if (inimigo != null && inimigo.transform.position.x > transform.position.x)
            {
                inimigosADireita.Add(inimigo);
            }
        }

        if (inimigosADireita.Count == 0) return;

        // Seleciona até 4 alvos únicos
        List<GameObject> alvosSelecionados = new List<GameObject>();
        int quantidadeDeMisseis = Mathf.Min(4, inimigosADireita.Count);
        List<int> indicesUsados = new List<int>();

        while (alvosSelecionados.Count < quantidadeDeMisseis)
        {
            int index = Random.Range(0, inimigosADireita.Count);
            if (!indicesUsados.Contains(index))
            {
                indicesUsados.Add(index);
                alvosSelecionados.Add(inimigosADireita[index]);
            }
        }

        foreach (GameObject alvo in alvosSelecionados)
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
