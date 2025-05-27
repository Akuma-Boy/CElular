using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class ObjetoCenario : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [Tooltip("Velocidade mínima do objeto")]
    public float velocidadeMinima = 1f;
    [Tooltip("Velocidade máxima do objeto")]
    public float velocidadeMaxima = 3f;
    [Tooltip("Tempo mínimo entre aparições")]
    public float tempoMinimoEntreSpawns = 5f;
    [Tooltip("Tempo máximo entre aparições")]
    public float tempoMaximoEntreSpawns = 15f;

    [Header("Configurações de Tamanho")]
    [Tooltip("Escala mínima (X e Y aumentam proporcionalmente)")]
    public float escalaMinima = 0.5f;
    [Tooltip("Escala máxima (X e Y aumentam proporcionalmente)")]
    public float escalaMaxima = 1.5f;
    [Tooltip("Margem de variação de escala (0-1)")]
    [Range(0f, 1f)] public float margemVariacaoEscala = 0.1f;

    [Header("Configurações de Margem")]
    [Tooltip("Margem horizontal para spawn/destruição")]
    public float margemHorizontal = 1f;
    [Tooltip("Margem vertical para posicionamento")]
    public float margemVertical = 0.5f;

    [Header("Efeito de Profundidade")]
    [Tooltip("Intensidade do desfoque")]
    [Range(0f, 10f)] public float intensidadeDesfoque = 3f;
    [Tooltip("Transparência mínima")]
    [Range(0f, 1f)] public float transparenciaMinima = 0.6f;

    private float velocidadeAtual;
    private Camera mainCamera;
    private float limiteEsquerdoDestruicao;
    private float limiteDireitoSpawn;
    private float alturaMinima;
    private float alturaMaxima;
    private float larguraObjeto;
    private Material materialObjeto;
    private float escalaBase;

    private void Awake()
    {
        mainCamera = Camera.main;
        materialObjeto = GetComponent<SpriteRenderer>().material = new Material(Shader.Find("UI/Unlit/Detail"));
        larguraObjeto = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void OnEnable()
    {
        IniciarCicloObjeto();
    }

    private void CalcularLimitesTela()
    {
        float alturaCamera = mainCamera.orthographicSize;
        float larguraCamera = alturaCamera * mainCamera.aspect;

        Vector3 posicaoCamera = mainCamera.transform.position;
        limiteEsquerdoDestruicao = posicaoCamera.x - larguraCamera - margemHorizontal;
        limiteDireitoSpawn = posicaoCamera.x + larguraCamera + margemHorizontal;
        alturaMinima = posicaoCamera.y - alturaCamera - margemVertical;
        alturaMaxima = posicaoCamera.y + alturaCamera + margemVertical;
    }

    private void IniciarCicloObjeto()
    {
        CalcularLimitesTela();
        
        // Configura propriedades aleatórias com proporção X/Y igual
        escalaBase = Random.Range(escalaMinima, escalaMaxima);
        float variacao = Random.Range(-margemVariacaoEscala, margemVariacaoEscala);
        float escalaFinal = Mathf.Clamp(escalaBase + variacao, escalaMinima, escalaMaxima);
        
        transform.localScale = new Vector3(escalaFinal, escalaFinal, 1f);

        velocidadeAtual = Random.Range(velocidadeMinima, velocidadeMaxima);

        // Posiciona fora da tela à direita
        float alturaAleatoria = Random.Range(alturaMinima, alturaMaxima);
        transform.position = new Vector3(
            limiteDireitoSpawn + larguraObjeto/2,
            alturaAleatoria,
            transform.position.z
        );

        // Configura efeito de profundidade inicial
        ConfigurarEfeitoProfundidade(0f);

        // Inicia movimento
        StartCoroutine(MoverObjeto());
    }

    private IEnumerator MoverObjeto()
    {
        // Move o objeto até sair da tela à esquerda
        while (transform.position.x > limiteEsquerdoDestruicao)
        {
            transform.Translate(Vector3.left * velocidadeAtual * Time.deltaTime);
            
            // Atualiza efeito de profundidade baseado na posição
            float progresso = Mathf.InverseLerp(limiteDireitoSpawn, limiteEsquerdoDestruicao, transform.position.x);
            ConfigurarEfeitoProfundidade(progresso);
            
            yield return null;
        }

        // Espera um tempo aleatório antes de reaparecer
        yield return new WaitForSeconds(Random.Range(tempoMinimoEntreSpawns, tempoMaximoEntreSpawns));

        // Reposiciona e reinicia o ciclo
        IniciarCicloObjeto();
    }

    private void ConfigurarEfeitoProfundidade(float progresso)
    {
        // Ajusta desfoque e transparência baseado no progresso
        float desfoque = Mathf.Lerp(0f, intensidadeDesfoque, progresso);
        float alpha = Mathf.Lerp(1f, transparenciaMinima, progresso);

        // Aplica efeitos no material
        materialObjeto.SetFloat("_Blur", desfoque);
        
        Color cor = materialObjeto.color;
        cor.a = alpha;
        materialObjeto.color = cor;
    }

    private void Update()
    {
        // Atualiza os limites se a câmera se mover
        if (mainCamera.transform.hasChanged)
        {
            CalcularLimitesTela();
            mainCamera.transform.hasChanged = false;
        }
    }
}