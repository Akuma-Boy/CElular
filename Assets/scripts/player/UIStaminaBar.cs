using UnityEngine;
using UnityEngine.UI;

public class UIStaminaBar : MonoBehaviour
{
    [SerializeField] private Image staminaFill; // A imagem com Fill Mode (tipo Filled)
    [SerializeField] private TiroMultiplo tiroMultiplo; // Referência ao script que controla a stamina

    private void Update()
    {
        if (tiroMultiplo == null || staminaFill == null)
            return;

        // Atualiza o fill da barra (valor entre 0 e 1)
        staminaFill.fillAmount = tiroMultiplo.GetStaminaNormalized();
    }
}
