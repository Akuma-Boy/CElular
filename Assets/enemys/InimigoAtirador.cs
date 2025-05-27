using UnityEngine;

public class InimigoAtirador : MonoBehaviour
{
    [Header("Configurações do Raio")]
    public GameObject raioPrefab; // Prefab do raio (deve ter Collider2D e SpriteRenderer)
    public float tempoCarregamento = 1f; // Tempo para carregar antes de atirar
    public float duracaoRaio = 2f; // Quanto tempo o raio fica ativo
    public float intervaloEntreAtaques = 3f; // Tempo entre um ataque e outro

    [Header("Efeitos")]
    public GameObject efeitoCarregamentoPrefab; // Efeito visual durante carregamento
    public float offsetPontoTiro = 0.5f; // Ajuste para o ponto de origem do raio

    private float tempoUltimoAtaque;
    private GameObject efeitoCarregamentoAtual;

    void Start()
    {
        tempoUltimoAtaque = -intervaloEntreAtaques; // Permite atacar imediatamente
    }

    void Update()
    {
        if (Time.time >= tempoUltimoAtaque + intervaloEntreAtaques)
        {
            StartCoroutine(Atacar());
            tempoUltimoAtaque = Time.time;
        }
    }

    System.Collections.IEnumerator Atacar()
    {
        // 1. Fase de carregamento
        if (efeitoCarregamentoPrefab != null)
        {
            efeitoCarregamentoAtual = Instantiate(efeitoCarregamentoPrefab, transform.position, Quaternion.identity);
            efeitoCarregamentoAtual.transform.parent = transform;
        }

        yield return new WaitForSeconds(tempoCarregamento);

        // 2. Destruir efeito de carregamento
        if (efeitoCarregamentoAtual != null)
        {
            Destroy(efeitoCarregamentoAtual);
        }

        // 3. Calcular posição e tamanho do raio
        Vector2 pontoTiro = new Vector2(
            transform.position.x - offsetPontoTiro,
            transform.position.y
        );

        // Criar o raio
        GameObject raio = Instantiate(raioPrefab, pontoTiro, Quaternion.identity);

        // Configurar o raio para se estender até o final da tela à esquerda
        ConfigurarRaio(raio, pontoTiro);

        // Destruir o raio após a duração
        Destroy(raio, duracaoRaio);
    }

    void ConfigurarRaio(GameObject raio, Vector2 pontoOrigem)
    {
        // Obter os componentes necessários
        SpriteRenderer spriteRenderer = raio.GetComponent<SpriteRenderer>();
        BoxCollider2D collider = raio.GetComponent<BoxCollider2D>();

        if (spriteRenderer == null || collider == null)
        {
            Debug.LogError("O prefab do raio precisa ter SpriteRenderer e BoxCollider2D");
            return;
        }

        // Calcular a distância até a borda esquerda da tela
        float distanciaParaBorda = Mathf.Abs(pontoOrigem.x - Camera.main.ViewportToWorldPoint(Vector2.zero).x);

        // Ajustar a escala do sprite (assumindo que o sprite está virado para a esquerda)
        Vector2 novaEscala = new Vector2(distanciaParaBorda, spriteRenderer.size.y);
        spriteRenderer.size = novaEscala;

        // Ajustar o collider
        collider.size = novaEscala;
        collider.offset = new Vector2(-novaEscala.x / 2f, 0); // Centralizar o collider
    }
}