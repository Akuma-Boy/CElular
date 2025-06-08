using UnityEngine;
using UnityEngine.UI;

public class GameStartManager : MonoBehaviour
{
    [Header("Configurações do Menu")]
    public GameObject startMenu;
    public GameObject player;

    [Header("Configurações do Jogo")]
    public bool startGameOnClick = true;

    // Controle para evitar pausar o tempo após restart
    private static bool jaIniciouAntes = false;

    void Start()
    {
        // Só pausa o tempo se for o primeiro load
        if (!jaIniciouAntes)
        {
            Time.timeScale = 0f;
            if (startMenu != null)
                startMenu.SetActive(true);
        }
        else
        {
            if (startMenu != null)
                startMenu.SetActive(false);
        }

        // Reconecta botão
        Button botao = startMenu != null ? startMenu.GetComponentInChildren<Button>() : null;
        if (botao != null)
        {
            botao.onClick.RemoveAllListeners();
            botao.onClick.AddListener(StartGame);
        }
        else
        {
            Debug.LogWarning("GameStartManager: Nenhum botão encontrado dentro do startMenu.");
        }
    }

    public void StartGame()
    {
        if (startMenu != null)
            startMenu.SetActive(false);

        Time.timeScale = 1f;

        if (player != null)
            player.SetActive(true);

        jaIniciouAntes = true;

        Debug.Log("Jogo Iniciado!");
    }
}
