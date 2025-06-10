using UnityEngine;

public class BotaoReiniciar : MonoBehaviour
{
    public GameObject scoreListPanel;
    public VidaNave vidaNave; // Referência à vida da nave

    public void ReiniciarJogo()
    {
        GameManager.Instance.StartNewGame(); // Reinicia o jogo e limpa a cena

        // Aguarda um pouco para garantir que tudo foi destruído antes de resetar score e vida
        Invoke(nameof(ResetStatus), 0.5f);

        if (scoreListPanel != null)
        {
            scoreListPanel.SetActive(false); // Esconde o painel de score
        }
    }

    private void ResetStatus()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetCurrentGameScore(); // Zera o score
            Debug.Log("BotaoReiniciar: Score resetado após limpeza da cena.");
        }
        else
        {
            Debug.LogError("BotaoReiniciar: ScoreManager não encontrado!");
        }

        if (vidaNave != null)
        {
            vidaNave.ResetVida(); // Reseta a vida da nave
            Debug.Log("BotaoReiniciar: Vida resetada após reiniciar jogo.");
        }
        else
        {
            Debug.LogError("BotaoReiniciar: VidaNave não encontrado!");
        }
    }
}
