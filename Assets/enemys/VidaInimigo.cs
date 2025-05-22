using UnityEngine;
using UnityEngine.Events;

public class VidaInimigo : MonoBehaviour
{
    [Header("Configurações de Vida")]
    [SerializeField] private int vidaMaxima = 3;
    [SerializeField] private int vidaAtual;
    [SerializeField] private bool invencivel = false;
    [SerializeField] private float tempoInvencibilidade = 0.5f;
    private float tempoUltimoDano;

    [Header("Eventos")]
    public UnityEvent aoReceberDano; // Disparado quando recebe dano
    public UnityEvent aoMorrer;      // Disparado quando vida chega a zero
    public UnityEvent aoCurar;       // Disparado quando recebe cura

    [Header("Configurações de Feedback")]
    [SerializeField] private Color corDano = Color.red;
    [SerializeField] private float tempoFlash = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color corOriginal;

    private void Awake()
    {
        vidaAtual = vidaMaxima;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            corOriginal = spriteRenderer.color;
        }
    }

    public void ReceberDano(int quantidade)
    {
        // Verifica se pode receber dano
        if (invencivel || Time.time < tempoUltimoDano + tempoInvencibilidade)
            return;

        vidaAtual -= quantidade;
        tempoUltimoDano = Time.time;

        // Dispara evento de dano
        aoReceberDano.Invoke();

        // Feedback visual
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashDano());
        }

        // Verifica morte
        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    private System.Collections.IEnumerator FlashDano()
    {
        spriteRenderer.color = corDano;
        yield return new WaitForSeconds(tempoFlash);
        spriteRenderer.color = corOriginal;
    }

    public void Curar(int quantidade)
    {
        vidaAtual = Mathf.Min(vidaAtual + quantidade, vidaMaxima);
        aoCurar.Invoke();
    }

    public void Morrer()
    {
        // Previne múltiplas chamadas
        if (vidaAtual > 0) vidaAtual = 0;

        // Dispara evento de morte
        aoMorrer.Invoke();

        // Destroi o objeto (substitua por pool de objetos se estiver usando)
        Destroy(gameObject);
    }

    public void TornarInvencivel(bool estado)
    {
        invencivel = estado;
    }

    // Métodos úteis para verificação
    public bool EstaMorto() => vidaAtual <= 0;
    public float PorcentagemVida() => (float)vidaAtual / vidaMaxima;
    public int GetVidaAtual() => vidaAtual;
    public int GetVidaMaxima() => vidaMaxima;

    // Configuração inicial
    public void ConfigurarVida(int vidaMax)
    {
        vidaMaxima = vidaMax;
        vidaAtual = vidaMaxima;
    }
}