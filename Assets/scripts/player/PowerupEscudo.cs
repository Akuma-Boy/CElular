using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PowerupEscudo : MonoBehaviour
{
    public float velocidade = 2f;
    public float tempoDeVida = 10f;

    private void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }

    private void Update()
    {
        transform.Translate(Vector2.left * velocidade * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que colidiu é o player
        if (other.CompareTag("Player"))
        {
            Escudo escudo = other.GetComponent<Escudo>();
            if (escudo != null)
            {
                escudo.AtivarEscudo();
                Destroy(gameObject);
            }
        }
    }
}
