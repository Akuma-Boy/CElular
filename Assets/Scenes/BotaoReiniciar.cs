using UnityEngine;
using UnityEngine.SceneManagement;

public class BotaoReiniciar : MonoBehaviour
{
    public GameObject scoreListPanel;

    public void ReiniciarJogo()
    {
        Debug.Log("BotaoReiniciar: Botão Reiniciar pressionado. Solicitando ao GameManager para recarregar a cena de jogo.");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadGameScene(); // This reloads the game scene
        }
        else
        {
            Debug.LogError("BotaoReiniciar: GameManager.Instance não encontrado! Não foi possível reiniciar o jogo através do GameManager.");
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (scoreListPanel != null)
        {
            scoreListPanel.SetActive(false); // Hide the score panel
        }
    }
}