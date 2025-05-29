using UnityEngine;

public class PowerUpTiro : MonoBehaviour
{
    public AudioClip somColeta;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TiroMultiplo tiro = collision.GetComponent<TiroMultiplo>();
        if (tiro != null)
        {
            tiro.AumentarTiro();

            if (somColeta != null)
                AudioSource.PlayClipAtPoint(somColeta, transform.position);

            Destroy(gameObject);
        }
    }
}
