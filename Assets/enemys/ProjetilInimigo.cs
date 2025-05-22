using UnityEngine;

public class ProjetilInimigo : MonoBehaviour
{
    [Header("Configurações")]
    public float velocidade = 10f;
    public float tempoVida = 3f;
    public int dano = 1;

    private Vector2 direcao;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Configurar(Vector2 dir)
    {
        direcao = dir.normalized;
        
        // Rotaciona o projétil para ficar virado na direção do movimento
        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angulo);
    }

    private void Start()
    {
        Destroy(gameObject, tempoVida);
        
        // Aplica velocidade inicial
        if (rb != null)
        {
            rb.linearVelocity = direcao * velocidade;
        }
    }

    private void Update()
    {
        // Movimento alternativo (caso não use Rigidbody)
        // transform.Translate(direcao * velocidade * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            VidaNave player = collision.GetComponent<VidaNave>();
            if (player != null)
            {
                player.ReceberDano(dano);
            }
            Destroy(gameObject);
        }
        else if (!collision.isTrigger) // Ignora outros triggers
        {
            Destroy(gameObject);
        }
    }
}