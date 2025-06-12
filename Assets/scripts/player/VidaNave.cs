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
    private TiroMultiplo tiroMultiplo; // Declared once

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

        // Initialize UnityEvents if they are null to prevent NullReferenceExceptions
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
        tempoUltimoDano = 0f; // Reset this as well

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = corOriginal;
        }

        gameObject.SetActive(true); // Ensure the player object is active

        if (tiroMultiplo != null)
        {
            tiroMultiplo.ResetTiro();
        }

        aoCurar.Invoke(); // Dispara o evento ao resetar a vida, garantindo que a UI seja atualizada

        Debug.Log("VidaNave: Vida resetada para " + vidaAtual);
    }

    public void ReceberDano(int quantidade)
    {
        if (invencivel || Time.time < tempoUltimoDano + tempoInvencibilidade)
            return;

        vidaAtual -= quantidade;
        tempoUltimoDano = Time.time; // Update last damage time here

        if (tiroMultiplo != null)
            tiroMultiplo.ReduzirTiro();

        if (spriteRenderer != null)
        {
            StartCoroutine(FlashDano());
        }

        aoReceberDano.Invoke(); // Invoke event for UI updates etc.

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
            Color originalColor = spriteRenderer.color; // Store current color in case it's not corOriginal
            spriteRenderer.color = corDano;
            yield return new WaitForSeconds(tempoFlash);
            spriteRenderer.color = originalColor; // Revert to what it was before flash
        }
    }

    private IEnumerator TemporarioInvencivel()
    {
        invencivel = true;
        float tempoFim = Time.time + tempoInvencibilidade;

        // Ensure current sprite state is saved before flashing
        bool originalEnabledState = spriteRenderer != null ? spriteRenderer.enabled : true;

        while (Time.time < tempoFim)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled; // Flashing effect
            }
            yield return new WaitForSeconds(0.1f); // Flash interval
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = originalEnabledState; // Restore original enabled state
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
        // Add a check to prevent multiple 'Morrer' calls if health goes below zero rapidly
        // Also ensure the game is currently active before triggering Game Over
        if (vidaAtual <= 0 && GameManager.Instance != null && !GameManager.Instance.IsGameActive)
        {
            Debug.LogWarning("VidaNave: Morrer() called but game is already inactive. Ignoring.");
            return;
        }

        Debug.Log("VidaNave: Morrer() chamado. Vida atual: " + vidaAtual);
        vidaAtual = 0; // Ensure health is exactly 0
        aoMorrer.Invoke();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver(); // Trigger the game over sequence
            Debug.Log("VidaNave: GameManager.GameOver chamado.");
        }
        else
        {
            Debug.LogError("VidaNave: GameManager.Instance não encontrado! Cannot trigger game over through GameManager. Player will simply disappear.");
            // Optionally: if GameManager is truly critical, you might want to SceneManager.LoadScene("Menu") here
            gameObject.SetActive(false); // Hide the player
        }
    }

    // Public getters for UI or other scripts to read current state
    public bool EstaMorto() => vidaAtual <= 0;
    public float PorcentagemVida() => (float)vidaAtual / vidaMaxima;
    public int GetVidaAtual() => vidaAtual;
    public int GetVidaMaxima() => vidaMaxima;
}