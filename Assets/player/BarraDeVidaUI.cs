using UnityEngine;
using UnityEngine.UI;

public class BarraDeVidaUI : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private VidaNave vidaNave;
    [SerializeField] private Image barraDeVida;

    private void Start()
    {
        if (vidaNave == null)
        {
            vidaNave = FindFirstObjectByType<VidaNave>();
        }

        AtualizarBarra();

        vidaNave.aoReceberDano.AddListener(AtualizarBarra);
        vidaNave.aoCurar.AddListener(AtualizarBarra);
        vidaNave.aoMorrer.AddListener(AtualizarBarra);
    }

    private void AtualizarBarra()
    {
        if (barraDeVida != null && vidaNave != null)
        {
            barraDeVida.fillAmount = vidaNave.PorcentagemVida();
        }
    }
}
