using UnityEngine;
using System.Collections;

public class RaioExpansivo : MonoBehaviour
{
    [Header("Modos de Operação")]
    public bool modoOtimizado = true; // Alternar entre os modos

    [Header("Configurações Comuns")]
    public float velocidadeExpansao = 15f;
    public float distanciaMaxima = 20f;
    public float tempoVidaTotal = 0.8f;
    public float fadeOutDuration = 0.2f;
    public int dano = 1; // Quantidade de dano que o raio causa
    public float intervaloDano = 0.5f; // Intervalo entre aplicações de dano

    [Header("Modo Segmentado (Pesado)")]
    public GameObject segmentoRaioPrefab;
    public float larguraSegmento = 0.5f;
    public float tempoVidaSegmento = 0.3f;

    [Header("Modo Otimizado")]
    public SpriteRenderer spriteRendererRaio;
    public BoxCollider2D hitboxRaio;

    [Header("Efeitos")]
    public ParticleSystem efeitoInicio;
    public ParticleSystem efeitoFim;

    private Vector2 pontoInicial;
    private Vector2 direcao;
    private float larguraAtual;
    private float timer;
    private Color corOriginal;
    private bool iniciandoFadeOut;
    private float ultimoDanoTempo;

    void Start()
    {
        pontoInicial = transform.position;
        direcao = transform.right;

        if (efeitoInicio != null)
        {
            Instantiate(efeitoInicio, pontoInicial, Quaternion.identity);
        }

        if (modoOtimizado)
        {
            IniciarModoOtimizado();
        }
        else
        {
            StartCoroutine(ExpandirRaioSegmentado());
        }
    }

    void IniciarModoOtimizado()
    {
        if (spriteRendererRaio == null) spriteRendererRaio = GetComponent<SpriteRenderer>();
        if (hitboxRaio == null) hitboxRaio = GetComponent<BoxCollider2D>();

        // Configurar o collider como trigger
        if (hitboxRaio != null)
        {
            hitboxRaio.isTrigger = true;
        }

        corOriginal = spriteRendererRaio.color;
        larguraAtual = 0.1f;
        AtualizarTamanhoRaio();
    }

    void Update()
    {
        if (!modoOtimizado) return;

        timer += Time.deltaTime;

        // Fase de expansão
        if (timer < tempoVidaTotal - fadeOutDuration)
        {
            larguraAtual = Mathf.Min(larguraAtual + velocidadeExpansao * Time.deltaTime, distanciaMaxima);
            AtualizarTamanhoRaio();
        }
        // Fase de fade out
        else if (!iniciandoFadeOut)
        {
            iniciandoFadeOut = true;
            StartCoroutine(FadeOutRaio());
        }

        // Destruir quando terminar
        if (timer >= tempoVidaTotal)
        {
            FinalizarRaio();
        }
    }

    IEnumerator ExpandirRaioSegmentado()
    {
        float distanciaAtual = 0f;
        bool expandindo = true;

        while (expandindo)
        {
            distanciaAtual += velocidadeExpansao * Time.deltaTime;

            if (distanciaAtual >= distanciaMaxima)
            {
                distanciaAtual = distanciaMaxima;
                expandindo = false;

                if (efeitoFim != null)
                {
                    Instantiate(efeitoFim, pontoInicial + direcao * distanciaAtual, Quaternion.identity);
                }

                break;
            }

            Vector2 posicaoSegmento = pontoInicial + direcao * distanciaAtual;
            GameObject segmento = Instantiate(segmentoRaioPrefab, posicaoSegmento, Quaternion.identity);
            segmento.transform.right = direcao;

            // Adicionar componente de dano aos segmentos
            var danoSegmento = segmento.AddComponent<DanoSegmento>();
            danoSegmento.dano = this.dano;
            danoSegmento.intervaloDano = this.intervaloDano;

            Destroy(segmento, tempoVidaSegmento);

            yield return null;
        }

        Destroy(gameObject, 1f);
    }

    void AtualizarTamanhoRaio()
    {
        spriteRendererRaio.size = new Vector2(larguraAtual, spriteRendererRaio.size.y);

        if (hitboxRaio != null)
        {
            hitboxRaio.size = new Vector2(larguraAtual, hitboxRaio.size.y);
            hitboxRaio.offset = new Vector2(larguraAtual * 0.5f, 0);
        }
    }

    IEnumerator FadeOutRaio()
    {
        float fadeTimer = 0;
        Color corAtual = corOriginal;

        while (fadeTimer < fadeOutDuration)
        {
            fadeTimer += Time.deltaTime;
            float progresso = fadeTimer / fadeOutDuration;

            corAtual.a = Mathf.Lerp(corOriginal.a, 0, progresso);
            spriteRendererRaio.color = corAtual;

            yield return null;
        }
    }

    void FinalizarRaio()
    {
        if (efeitoFim != null)
        {
            Instantiate(efeitoFim, pontoInicial + direcao * larguraAtual, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!modoOtimizado) return;

        // Verifica se pode causar dano novamente
        if (Time.time >= ultimoDanoTempo + intervaloDano)
        {
            VidaNave vida = other.GetComponent<VidaNave>();
            if (vida != null && !vida.EstaMorto())
            {
                vida.ReceberDano(dano);
                ultimoDanoTempo = Time.time;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * distanciaMaxima);
    }
}

// Classe auxiliar para os segmentos no modo não-otimizado
public class DanoSegmento : MonoBehaviour
{
    public int dano;
    public float intervaloDano;
    private float ultimoDanoTempo;

    void OnTriggerStay2D(Collider2D other)
    {
        if (Time.time >= ultimoDanoTempo + intervaloDano)
        {
            VidaNave vida = other.GetComponent<VidaNave>();
            if (vida != null && !vida.EstaMorto())
            {
                vida.ReceberDano(dano);
                ultimoDanoTempo = Time.time;
            }
        }
    }
}