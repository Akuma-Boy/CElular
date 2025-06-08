using UnityEngine;

public class ProjetilDano : MonoBehaviour
{
    [Header("Configurações de Dano")]
    public int dano = 1; // Quantidade de dano que o projétil causa
    public bool destruirAoColidir = true; // Se o projétil deve ser destruído ao acertar algo
    public string[] tagsAlvo = { "Inimigo" }; // Tags que podem receber dano

    [Header("Efeitos")]
    public GameObject efeitoImpacto; // Prefab de partículas para o impacto
    public AudioClip somImpacto; // Som ao acertar o inimigo

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se colidiu com um alvo válido
        foreach (string tag in tagsAlvo)
        {
            if (collision.CompareTag(tag))
            {
                CausarDano(collision.gameObject);

                // Destrói o projétil se configurado
                if (destruirAoColidir)
                {
                    CriarEfeitoImpacto();
                    Destroy(gameObject);
                }
                return;
            }
        }

        // Se colidiu com algo que não é alvo (como parede)
        if (destruirAoColidir)
        {
            Destroy(gameObject);
        }
    }

    private void CausarDano(GameObject alvo)
    {
        VidaInimigo vidaInimigo = alvo.GetComponent<VidaInimigo>();
        if (vidaInimigo != null && !vidaInimigo.EstaMorto())
        {
            vidaInimigo.ReceberDano(dano);
        }
    }

    private void CriarEfeitoImpacto()
    {
        // Cria efeito visual de impacto
        if (efeitoImpacto != null)
        {
            Instantiate(efeitoImpacto, transform.position, Quaternion.identity);
        }

        // Toca som de impacto
        if (somImpacto != null)
        {
            AudioSource.PlayClipAtPoint(somImpacto, transform.position);
        }
    }
}