using UnityEngine;
using UnityEngine.SceneManagement;

public class botamenu : MonoBehaviour
{
    public GameObject scoreListPanel;
    public GameObject menui;

    public void ReiniciarJogo()
    {
        Debug.Log("Iniciando processo de reinício...");

        // 1º - Pausa o jogo imediatamente
        Time.timeScale = 0f;
        Debug.Log("Jogo pausado");

        // 2º - Fecha painel de scores se estiver aberto
        if (scoreListPanel != null)
        {
            scoreListPanel.SetActive(false);
        }

        // 3º - Recarrega a cena (com o jogo pausado)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadGameScene();
        }
        else
        {
            Debug.LogWarning("Recarregando cena diretamente...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // 4º - Abre o menu (após o carregamento)
        // Isso precisa ser feito após a cena carregar
        // Usaremos um método atrasado para garantir
        Invoke("AbrirMenuAposCarregamento", 0.1f);
    }

    private void AbrirMenuAposCarregamento()
    {
        if (menui != null)
        {
            menui.SetActive(true);
            Debug.Log("Menu aberto após recarregamento");
        }
        else
        {
            Debug.LogError("Objeto do menu não atribuído!");
        }
    }
}