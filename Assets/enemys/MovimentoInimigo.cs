using UnityEngine;

public class MovimentoInimigo : MonoBehaviour
{
    public float velocidade = 3f;
    
    private void Update()
    {
        // Movimento básico para esquerda
        transform.Translate(Vector2.left * velocidade * Time.deltaTime);
    }
}