using UnityEngine;

public class MovimentoInimigo : MonoBehaviour
{
    public float velocidade = 3f; // Initial value, will be overridden by DificuldadeProgressiva

    void Update()
    {
        transform.Translate(Vector2.left * velocidade * Time.deltaTime);
    }
}