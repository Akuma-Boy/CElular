using UnityEngine;

public class BotaoReiniciar : MonoBehaviour
{
    public void ReiniciarJogo()
    {
        GameManager.Instance.StartNewGame(); // Seu método que já limpa tudo
    }
}
