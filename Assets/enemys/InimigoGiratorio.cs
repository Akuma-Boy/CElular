using UnityEngine;

public class InimigoGiratorio : MonoBehaviour
{
    [Header("Configurações de Rotação")]
    [SerializeField] private float velocidadeRotacao = 180f; // Graus por segundo
    [SerializeField] private bool girarNoSentidoHorario = true;

    void Update()
    {
        // Calcula a direção da rotação baseada na escolha do sentido
        float direcao = girarNoSentidoHorario ? -1f : 1f;
        
        // Rotaciona o objeto
        transform.Rotate(0f, 0f, velocidadeRotacao * direcao * Time.deltaTime);
    }
}