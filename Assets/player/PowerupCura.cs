using UnityEngine;

public class PowerupCura : MonoBehaviour
{
    public int quantidadeCura = 1;
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
        VidaNave vida = other.GetComponent<VidaNave>();
        if (vida != null && !vida.EstaMorto())
        {
            vida.Curar(quantidadeCura);
            Destroy(gameObject);
        }
    }
}
