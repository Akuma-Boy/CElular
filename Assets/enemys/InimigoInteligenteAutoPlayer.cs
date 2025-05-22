using UnityEngine;

public class InimigoInteligenteAutoPlayer : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float velocidadeNormal = 3f;
    public float velocidadeAtaque = 5f;
    public float distanciaMinima = 7f; // Distância mínima do player
    public float distanciaMaxima = 15f; // Distância máxima do player
    public float tempoDeVida = 10f; // Tempo até sair da tela

    [Header("Configurações de Tiro")]
    public GameObject projetilPrefab;
    public Transform[] pontosDeTiro;
    public float tempoEntreTiros = 1.5f;
    public Transform frenteInimigo; // Referência para a "ponta" do inimigo

    private Transform jogador;
    private float ultimoTiro;
    private float tempoNascimento;
    private bool deveSair;

    private void Awake()
    {
        jogador = GameObject.FindWithTag("Player")?.transform;
        if (jogador == null)
        {
            Debug.LogWarning("Player não encontrado! Inimigo será desativado.");
            enabled = false;
        }
        tempoNascimento = Time.time;
    }

    private void Update()
    {
        if (jogador == null) return;

        // Rotaciona para sempre ficar virado para o jogador
        RotacionarParaJogador();

        float distancia = Vector2.Distance(transform.position, jogador.position);

        // Verifica se é hora de sair
        if (Time.time > tempoNascimento + tempoDeVida)
        {
            deveSair = true;
        }

        if (deveSair)
        {
            SairDaTela();
        }
        else if (distancia < distanciaMinima)
        {
            // Afasta-se se estiver muito perto
            Mover((transform.position - jogador.position).normalized, velocidadeAtaque);
        }
        else if (distancia > distanciaMaxima)
        {
            // Aproxima-se se estiver muito longe
            Mover((jogador.position - transform.position).normalized, velocidadeNormal);
        }
        else
        {
            // Mantém distância e atira
            if (Time.time > ultimoTiro + tempoEntreTiros)
            {
                Atirar();
                ultimoTiro = Time.time;
            }
        }
    }

    private void RotacionarParaJogador()
    {
        if (frenteInimigo != null)
        {
            // Calcula a direção para o jogador
            Vector3 direcao = jogador.position - frenteInimigo.position;
            float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
            
            // Rotaciona o objeto para que a "frente" aponte para o jogador
            frenteInimigo.rotation = Quaternion.Euler(0, 0, angulo);
        }
    }

    private void Mover(Vector2 direcao, float velocidade)
    {
        // Movimento suave sem fisicas
        transform.Translate(direcao * velocidade * Time.deltaTime, Space.World);
    }

    private void Atirar()
{
    if (projetilPrefab == null || pontosDeTiro.Length == 0 || jogador == null) return;

    foreach (Transform ponto in pontosDeTiro)
    {
        // Calcula direção para o jogador
        Vector2 direcao = (jogador.position - ponto.position).normalized;
        
        // Instancia o projétil
        GameObject projetil = Instantiate(projetilPrefab, ponto.position, Quaternion.identity);
        
        // Configura a direção
        ProjetilInimigo scriptProjetil = projetil.GetComponent<ProjetilInimigo>();
        if (scriptProjetil != null)
        {
            scriptProjetil.Configurar(direcao);
        }
    }
    
    ultimoTiro = Time.time;
}

    private void SairDaTela()
    {
        // Move-se para a direita (fora da tela)
        transform.Translate(Vector2.right * velocidadeAtaque * Time.deltaTime, Space.World);
        
        // Destrói quando estiver suficientemente longe
        if (transform.position.x > Camera.main.ViewportToWorldPoint(new Vector3(1.5f, 0, 0)).x)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualização das distâncias
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaMinima);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaMaxima);
    }
}