using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Adicionado para usar Where()

public class MultiMissilController : MonoBehaviour
{
    [Header("Configurações dos Mísseis")]
    public GameObject projetilPrefab;
    public float velocidadeProjetil = 5f;
    public float velocidadeRotacao = 200f;
    public float tempoVida = 5f;
    // Removendo KeyCode teclaDisparo, pois o disparo agora será via método público ou Input.GetKeyDown(KeyCode.E)
    // public KeyCode teclaDisparo = KeyCode.E; // Não é mais necessário para o botão UI

    [Header("Configurações de Disparo")]
    [SerializeField] private float cooldown = 2f;
    private float ultimoDisparo = -999f;

    public float Cooldown => cooldown;
    public float UltimoDisparo => ultimoDisparo;

    private void Update()
    {
        // Agora, o teclado ainda funciona, mas o botão UI chamará o método publico diretamente.
        if (Input.GetKeyDown(KeyCode.E) && Time.time > ultimoDisparo + cooldown) // Usando KeyCode.E diretamente
        {
            DispararMisseisGuiadosComCooldown(); // Chama o novo método que contém o cooldown
        }
    }

    // NOVO MÉTODO PÚBLICO para ser chamado pelo botão da UI
    public void AtivarHabilidadeMissilGuiado()
    {
        if (Time.time > ultimoDisparo + cooldown)
        {
            DispararMisseisGuiadosComCooldown();
        }
        else
        {
            Debug.Log($"<color=orange>MultiMissilController:</color> Mísseis guiados em cooldown. Tempo restante: {Mathf.Max(0, ultimoDisparo + cooldown - Time.time):F1}s");
        }
    }

    // Novo método privado para encapsular a lógica de disparo com cooldown
    private void DispararMisseisGuiadosComCooldown()
    {
        // Se houver lógica de custo (stamina, etc.) na sua nave, adicione aqui
        // Ex: if (GetComponent<NaveController>().StaminaAtual >= custoHabilidade) { ... }
        // Ou você pode adicionar um custo aqui no MultiMissilController

        DispararMisseisGuiados(); // Chama a função real de disparo
        ultimoDisparo = Time.time;
    }

    private void DispararMisseisGuiados()
    {
        // Usando LINQ para uma busca mais eficiente (certifique-se de ter 'using System.Linq;' no topo)
        GameObject[] inimigos = GameObject.FindGameObjectsWithTag("Inimigo");
        if (inimigos.Length == 0)
        {
            Debug.Log("<color=yellow>MultiMissilController:</color> Nenhum inimigo encontrado para atirar mísseis guiados.");
            return;
        }

        List<GameObject> inimigosADireita = inimigos
            .Where(inimigo => inimigo != null && inimigo.transform.position.x > transform.position.x)
            .ToList();

        if (inimigosADireita.Count == 0)
        {
            Debug.Log("<color=yellow>MultiMissilController:</color> Nenhum inimigo à direita para atirar mísseis guiados.");
            return;
        }

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
        Debug.Log($"<color=blue>MultiMissilController:</color> Disparado {alvosSelecionados.Count} mísseis guiados.");
    }

    private void CriarMissil(GameObject alvo)
    {
        GameObject projetil = Instantiate(projetilPrefab, transform.position, Quaternion.identity);
        MissilGuiado missil = projetil.AddComponent<MissilGuiado>();
        missil.Configurar(alvo, velocidadeProjetil, velocidadeRotacao, tempoVida);
    }
}