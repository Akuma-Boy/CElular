using UnityEngine;
using System.Collections.Generic;

public class MultiMissilController : MonoBehaviour
{
    [Header("Configurações dos Mísseis")]
    public GameObject projetilPrefab;      // Prefab do míssil guiado
    public float velocidadeProjetil = 5f;  // Velocidade base do míssil
    public float velocidadeRotacao = 200f; // Velocidade de rotação para perseguir
    public float tempoVida = 5f;           // Tempo até auto-destruição
    public KeyCode teclaDisparo = KeyCode.E; // Tecla para ativar

    [Header("Configurações de Disparo")]
    public float cooldown = 2f;           // Tempo de recarga
    private float ultimoDisparo;           // Controle do cooldown

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
        // Encontra todos os inimigos na cena
        GameObject[] inimigos = GameObject.FindGameObjectsWithTag("Inimigo");
        
        // Se não houver inimigos, não dispara
        if (inimigos.Length == 0) return;

        // Cria lista de alvos únicos
        List<GameObject> alvos = new List<GameObject>();
        
        // Seleciona até 4 inimigos aleatórios
        for (int i = 0; i < Mathf.Min(4, inimigos.Length); i++)
        {
            GameObject inimigoAleatorio;
            do {
                inimigoAleatorio = inimigos[Random.Range(0, inimigos.Length)];
            } while (alvos.Contains(inimigoAleatorio) && inimigos.Length > alvos.Count);
            
            alvos.Add(inimigoAleatorio);
        }

        // Dispara um míssil para cada alvo
        foreach (GameObject alvo in alvos)
        {
            CriarMissil(alvo);
        }
    }

    private void CriarMissil(GameObject alvo)
    {
        // Instancia o projétil
        GameObject projetil = Instantiate(projetilPrefab, transform.position, Quaternion.identity);
        
        // Configura o míssil
        MissilGuiado missil = projetil.AddComponent<MissilGuiado>();
        missil.Configurar(alvo, velocidadeProjetil, velocidadeRotacao, tempoVida);
    }
}

// Script auxiliar para o comportamento do míssil
public class MissilGuiado : MonoBehaviour
{
    private GameObject alvo;
    private float velocidade;
    private float velocidadeRot;
    private float tempoVida;

    public void Configurar(GameObject alvoSelecionado, float vel, float velRot, float vida)
    {
        alvo = alvoSelecionado;
        velocidade = vel;
        velocidadeRot = velRot;
        tempoVida = vida;
        
        // Auto-destruição após tempo de vida
        Destroy(gameObject, vida);
    }

    private void Update()
    {
        if (alvo == null || !alvo.activeInHierarchy)
        {
            // Se o alvo foi destruído, segue em frente
            transform.Translate(Vector2.right * velocidade * Time.deltaTime);
            return;
        }

        // Calcula direção para o alvo
        Vector2 direcao = (Vector2)alvo.transform.position - (Vector2)transform.position;
        direcao.Normalize();

        // Calcula rotação para perseguir
        float rotacaoZ = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        Quaternion rotacaoDesejada = Quaternion.Euler(0f, 0f, rotacaoZ);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotacaoDesejada, velocidadeRot * Time.deltaTime);

        // Movimenta o míssil
        transform.Translate(Vector2.right * velocidade * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == alvo)
        {
            // Adicione aqui lógica de dano ao inimigo se necessário
            Destroy(gameObject);
        }
    }
}