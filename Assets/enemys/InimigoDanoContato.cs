using UnityEngine;

public class InimigoDanoContato : MonoBehaviour
{
    [Header("Configura��es de Dano")]
    public int dano = 1;                  // Quantidade de dano causado
    public float tempoEntreDanos = 1f;    // Tempo m�nimo entre danos consecutivos
    public bool destruirAoColidir = false; // Se o inimigo deve ser destru�do no impacto

    [Header("Efeitos")]
    public GameObject efeitoImpacto;      // Prefab de part�culas para o impacto
    public AudioClip somImpacto;          // Som ao acertar o jogador

    private float ultimoDanoTempo;        // Controle do tempo entre danos
    private VidaNave vidaJogador;         // Refer�ncia ao script de vida do jogador

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se colidiu com o jogador
        if (collision.gameObject.CompareTag("Player"))
        {
            // Obt�m o componente de vida do jogador uma vez
            if (vidaJogador == null)
            {
                vidaJogador = collision.gameObject.GetComponent<VidaNave>();
            }

            CausarDano();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Vers�o para trigger (caso esteja usando colliders como trigger)
        if (collision.CompareTag("Player"))
        {
            if (vidaJogador == null)
            {
                vidaJogador = collision.GetComponent<VidaNave>();
            }

            CausarDano();
        }
    }

    private void CausarDano()
    {
        // Verifica o intervalo entre danos
        if (Time.time < ultimoDanoTempo + tempoEntreDanos)
            return;

        // Verifica se o jogador tem componente de vida
        if (vidaJogador != null && !vidaJogador.EstaMorto())
        {
            vidaJogador.ReceberDano(dano);
            ultimoDanoTempo = Time.time;

            // Efeitos de impacto
            CriarEfeitoImpacto();

            // Auto-destrui��o se configurado
            if (destruirAoColidir)
            {
                Destroy(gameObject);
            }
        }
    }

    private void CriarEfeitoImpacto()
    {
        // Efeito visual
        if (efeitoImpacto != null)
        {
            Instantiate(efeitoImpacto, transform.position, Quaternion.identity);
        }

        // Som
        if (somImpacto != null)
        {
            AudioSource.PlayClipAtPoint(somImpacto, transform.position);
        }
    }
}