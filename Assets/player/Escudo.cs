using UnityEngine;

public class Escudo : MonoBehaviour
{
    [Header("Referência Visual")]
    public GameObject escudoVisual;
    private bool escudoAtivo = false;

    private void Awake()
    {
        if (escudoVisual != null)
            escudoVisual.SetActive(false);
    }

    public void AtivarEscudo()
    {
        escudoAtivo = true;
        if (escudoVisual != null)
            escudoVisual.SetActive(true);
    }

    public bool ConsumirEscudo()
    {
        if (escudoAtivo)
        {
            escudoAtivo = false;
            if (escudoVisual != null)
                escudoVisual.SetActive(false);
            return true;
        }
        return false;
    }

    public bool EscudoAtivo() => escudoAtivo;
}
