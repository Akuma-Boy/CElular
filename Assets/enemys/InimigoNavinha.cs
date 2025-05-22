using UnityEngine;

public class InimigoNavinha : MonoBehaviour
{
    [Header("Configura��es de Movimento")]
    public float velocidade = 3f; // Velocidade de movimento
    public bool destruirQuandoSairDaTela = true; // Se deve ser destru�do ao sair da vis�o da c�mera

    [Header("Configura��es de Tempo de Vida")]
    public float tempoDeVida = 5f; // Tempo at� ser destru�do automaticamente
    private float tempoNascimento;

    private void Start()
    {
        tempoNascimento = Time.time;

        // Garante que o objeto tem um Rigidbody2D para f�sica
        if (GetComponent<Rigidbody2D>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    private void Update()
    {
        // Movimento constante para a esquerda
        transform.Translate(Vector2.left * velocidade * Time.deltaTime);

        // Destrui��o por tempo de vida
        if (Time.time > tempoNascimento + tempoDeVida)
        {
            DestruirInimigo();
        }
    }

    private void OnBecameInvisible()
    {
        // Destr�i quando sair da vis�o da c�mera
        if (destruirQuandoSairDaTela)
        {
            DestruirInimigo();
        }
    }

    private void DestruirInimigo()
    {
        // Adicione aqui efeitos de destrui��o se quiser (part�culas, som, etc)
        Destroy(gameObject);
    }

    // M�todo para configurar a velocidade (pode ser chamado por outros scripts)
    public void ConfigurarVelocidade(float novaVelocidade)
    {
        velocidade = novaVelocidade;
    }
}