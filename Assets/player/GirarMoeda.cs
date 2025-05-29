using UnityEngine;

public class GirarMoeda : MonoBehaviour
{
    public float velocidadeRotacao = 360f; // graus por segundo

    void Update()
    {
        // Gira no eixo Z
        transform.Rotate(0f, 0f, velocidadeRotacao * Time.deltaTime);
    }
}
