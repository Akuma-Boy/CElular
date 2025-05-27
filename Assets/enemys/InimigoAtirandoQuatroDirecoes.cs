using UnityEngine;

public class InimigoAtiradorQuatroDirecoes : MonoBehaviour
{
    [Header("Configurações de Tiro")]
    [SerializeField] private GameObject projetilPrefab;
    [SerializeField] private float intervaloEntreTiros = 2f;
    [SerializeField] private float forcaImpulso = 10f;
    [SerializeField] private float distanciaSpawnProjetil = 0.5f;

    private float tempoUltimoTiro;

    void Start()
    {
        tempoUltimoTiro = Time.time;
    }

    void Update()
    {
        if (Time.time - tempoUltimoTiro >= intervaloEntreTiros)
        {
            AtirarQuatroDirecoes();
            tempoUltimoTiro = Time.time;
        }
    }

    void AtirarQuatroDirecoes()
    {
        // Direções dos projéteis (normalizadas)
        Vector2[] direcoes = {
            Vector2.up,        // Cima
            Vector2.down,      // Baixo
            Vector2.left,      // Esquerda
            Vector2.right      // Direita
        };

        foreach (Vector2 direcao in direcoes)
        {
            // Calcula posição de spawn com pequeno offset
            Vector2 posicaoSpawn = (Vector2)transform.position + direcao * distanciaSpawnProjetil;
            
            // Instancia o projétil
            GameObject projetil = Instantiate(projetilPrefab, posicaoSpawn, Quaternion.identity);
            
            // Configura o projétil usando o script existente
            ProjetilInimigo scriptProjetil = projetil.GetComponent<ProjetilInimigo>();
            if (scriptProjetil != null)
            {
                scriptProjetil.Configurar(direcao);
                
                // Aplica impulso físico
                Rigidbody2D rb = projetil.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.AddForce(direcao * forcaImpulso, ForceMode2D.Impulse);
                }
            }
        }
    }
}