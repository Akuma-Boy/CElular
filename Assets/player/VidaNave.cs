using UnityEngine;
using UnityEngine.Events;

public class VidaNave : MonoBehaviour
{
    [Header("Configurações de Vida")]
    [SerializeField] private int vidaMaxima = 3;
    [SerializeField] private int vidaAtual;
    [SerializeField] private float tempoInvencibilidade = 1.5f;
    private float tempoUltimoDano;
    private bool invencivel = false;

    [Header("Eventos")]
    public UnityEvent aoReceberDano;
    public UnityEvent aoMorrer;
    public UnityEvent aoCurar;

    [Header("Feedback Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color corDano = Color.red;
    [SerializeField] private float tempoFlash = 0.1f;
    private Color corOriginal;
    private TiroMultiplo tiroMultiplo;

    private void Awake()
    {

        tiroMultiplo = GetComponent<TiroMultiplo>();
        vidaAtual = vidaMaxima;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            corOriginal = spriteRenderer.color;
        }
    }

    public void ReceberDano(int quantidade)
    {



        if (invencivel || Time.time < tempoUltimoDano + tempoInvencibilidade)
            return;

        vidaAtual -= quantidade;
        tempoUltimoDano = Time.time;

        if (tiroMultiplo != null)
            tiroMultiplo.ReduzirTiro();

        // Feedback visual
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashDano());
        }

        aoReceberDano.Invoke();

        if (vidaAtual <= 0)
        {
            Morrer();
        }
        else
        {
            StartCoroutine(TemporarioInvencivel());
        }
    }

    private System.Collections.IEnumerator FlashDano()
    {
        spriteRenderer.color = corDano;
        yield return new WaitForSeconds(tempoFlash);
        spriteRenderer.color = corOriginal;
    }

    private System.Collections.IEnumerator TemporarioInvencivel()
    {
        invencivel = true;

        // Piscar enquanto invencível
        float tempoFim = Time.time + tempoInvencibilidade;
        while (Time.time < tempoFim)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
        }

        spriteRenderer.enabled = true;
        invencivel = false;
    }

    public void Curar(int quantidade)
    {
        vidaAtual = Mathf.Min(vidaAtual + quantidade, vidaMaxima);
        aoCurar.Invoke();
    }

    public void Morrer()
    {
        vidaAtual = 0;
        aoMorrer.Invoke();

        // Desativa a nave (você pode substituir por lógica de game over)
        gameObject.SetActive(false);
    }

    // Métodos úteis
    public bool EstaMorto() => vidaAtual <= 0;
    public float PorcentagemVida() => (float)vidaAtual / vidaMaxima;
    public int GetVidaAtual() => vidaAtual;
    public int GetVidaMaxima() => vidaMaxima;
}