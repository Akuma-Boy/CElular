using UnityEngine;

public class InimigoNavinha : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float velocidade = 3f; // Velocidade de movimento
    public bool destruirQuandoSairDaTela = true; // Se deve ser destruído ao sair da visão da câmera

    [Header("Configurações de Tempo de Vida")]
    public float tempoDeVida = 5f; // Tempo até ser destruído automaticamente
    private float tempoNascimento;

    private void Start()
    {
        tempoNascimento = Time.time;

        // Garante que o objeto tem um Rigidbody2D para física
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

        // Destruição por tempo de vida
        if (Time.time > tempoNascimento + tempoDeVida)
        {
            DestruirInimigo();
        }
    }

    private void OnBecameInvisible()
    {
        // Destrói quando sair da visão da câmera
        if (destruirQuandoSairDaTela)
        {
            DestruirInimigo();
        }
    }

    private void DestruirInimigo()
    {
        // Adicione aqui efeitos de destruição se quiser (partículas, som, etc)
        Destroy(gameObject);
    }

    // Método para configurar a velocidade (pode ser chamado por outros scripts)
    public void ConfigurarVelocidade(float novaVelocidade)
    {
        velocidade = novaVelocidade;
    }
}