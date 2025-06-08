using UnityEngine;

public class PowerUpMover : MonoBehaviour
{
    public float velocidade = 2f;
    public float tempoDeVida = 10f;

    void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }

    void Update()
    {
        transform.Translate(Vector2.left * velocidade * Time.deltaTime);
    }
}
