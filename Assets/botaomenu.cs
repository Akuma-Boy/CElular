using UnityEngine;
using UnityEngine.SceneManagement;

public class botamenu : MonoBehaviour
{
    public GameObject scoreListPanel;
    public GameObject menui;

    public void ReiniciarJogo()
    {
        Debug.Log("Iniciando processo de rein�cio...");

        // 1� - Pausa o jogo imediatamente
        Time.timeScale = 0f;
        Debug.Log("Jogo pausado");

        // 2� - Fecha painel de scores se estiver aberto
        if (scoreListPanel != null)
        {
            scoreListPanel.SetActive(false);
        }

        // 3� - Recarrega a cena (com o jogo pausado)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadGameScene();
        }
        else
        {
            Debug.LogWarning("Recarregando cena diretamente...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // 4� - Abre o menu (ap�s o carregamento)
        // Isso precisa ser feito ap�s a cena carregar
        // Usaremos um m�todo atrasado para garantir
        Invoke("AbrirMenuAposCarregamento", 0.1f);
    }

    private void AbrirMenuAposCarregamento()
    {
        if (menui != null)
        {
            menui.SetActive(true);
            Debug.Log("Menu aberto ap�s recarregamento");
        }
        else
        {
            Debug.LogError("Objeto do menu n�o atribu�do!");
        }
    }
}