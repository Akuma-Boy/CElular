using UnityEngine;

public class MovimentoInimigo : MonoBehaviour
{
    public float velocidade = 3f;

    public void SetVelocidade(float novaVelocidade)
    {
        velocidade = novaVelocidade;
    }

    void Update()
    {
        transform.Translate(Vector2.left * velocidade * Time.deltaTime);
    }
}
