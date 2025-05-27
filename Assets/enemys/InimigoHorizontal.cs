using UnityEngine;

public class InimigoHorizontal : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float velocidade = 10f; // Velocidade de movimento
    [SerializeField] private float distancia = 5f;   // Distância entre os pontos de movimento
    [SerializeField] private bool comecarNaDireita = false; // Direção inicial

    private Vector3 pontoEsquerdo;
    private Vector3 pontoDireito;
    private bool movendoParaDireita;

    void Start()
    {
        // Define os pontos de movimento baseados na posição inicial
        pontoEsquerdo = transform.position - Vector3.right * distancia;
        pontoDireito = transform.position + Vector3.right * distancia;

        // Define a direção inicial
        movendoParaDireita = comecarNaDireita;
    }

    void Update()
    {
        // Move o objeto
        if (movendoParaDireita)
        {
            transform.position = Vector3.MoveTowards(transform.position, pontoDireito, velocidade * Time.deltaTime);

            // Verifica se chegou ao ponto direito
            if (Vector3.Distance(transform.position, pontoDireito) < 0.1f)
            {
                movendoParaDireita = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, pontoEsquerdo, velocidade * Time.deltaTime);

            // Verifica se chegou ao ponto esquerdo
            if (Vector3.Distance(transform.position, pontoEsquerdo) < 0.1f)
            {
                movendoParaDireita = true;
            }
        }
    }

    // Desenha os pontos de movimento no editor (apenas para visualização)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (Application.isPlaying)
        {
            Gizmos.DrawSphere(pontoEsquerdo, 0.2f);
            Gizmos.DrawSphere(pontoDireito, 0.2f);
        }
        else
        {
            Gizmos.DrawSphere(transform.position - Vector3.right * distancia, 0.2f);
            Gizmos.DrawSphere(transform.position + Vector3.right * distancia, 0.2f);
        }
    }
    
    void OnBecameInvisible() {
    Destroy(gameObject);
}
}