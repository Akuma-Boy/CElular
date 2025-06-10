using UnityEngine;
using UnityEngine.Events;
using System.Collections;

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
        
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            corOriginal = spriteRenderer.color;
        }

        if (aoReceberDano == null) aoReceberDano = new UnityEvent();
        if (aoMorrer == null) aoMorrer = new UnityEvent();
        if (aoCurar == null) aoCurar = new UnityEvent();
    }

    private void OnEnable()
    {
        ResetVida();
        Debug.Log("VidaNave: OnEnable chamou ResetVida(). Vida atual: " + vidaAtual);
    }

    public void ResetVida()
    {
        vidaAtual = vidaMaxima;
        invencivel = false;
        tempoUltimoDano = 0f;

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = corOriginal;
        }

        gameObject.SetActive(true);

        if (tiroMultiplo != null)
        {
            tiroMultiplo.ResetTiro();
        }

        aoCurar.Invoke(); // **Dispara o evento ao resetar a vida, garantindo que a UI seja atualizada**

        Debug.Log("VidaNave: Vida resetada para " + vidaAtual);
    }


    public void ReceberDano(int quantidade)
    {
        if (invencivel || Time.time < tempoUltimoDano + tempoInvencibilidade)
            return;

        vidaAtual -= quantidade;
        tempoUltimoDano = Time.time;

        if (tiroMultiplo != null)
            tiroMultiplo.ReduzirTiro();

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

    private IEnumerator FlashDano()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = corDano;
            yield return new WaitForSeconds(tempoFlash);
            spriteRenderer.color = corOriginal;
        }
    }

    private IEnumerator TemporarioInvencivel()
    {
        invencivel = true;
        float tempoFim = Time.time + tempoInvencibilidade;
        
        while (Time.time < tempoFim)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }
            yield return new WaitForSeconds(0.1f);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
        invencivel = false;
    }

    public void Curar(int quantidade)
    {
        vidaAtual = Mathf.Min(vidaAtual + quantidade, vidaMaxima);
        aoCurar.Invoke();
        Debug.Log("VidaNave: Vida curada para " + vidaAtual);
    }

    public void Morrer()
    {
        if (vidaAtual > 0) return; // Evita chamadas redundantes
        Debug.Log("VidaNave: Morrer() chamado. Vida atual: " + vidaAtual);
        vidaAtual = 0;
        aoMorrer.Invoke();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
            Debug.Log("VidaNave: GameManager.GameOver chamado.");
        }
        else
        {
            Debug.LogError("VidaNave: GameManager.Instance não encontrado!");
            gameObject.SetActive(false);
        }
    }

    public bool EstaMorto() => vidaAtual <= 0;
    public float PorcentagemVida() => (float)vidaAtual / vidaMaxima;
    public int GetVidaAtual() => vidaAtual;
    public int GetVidaMaxima() => vidaMaxima;
}