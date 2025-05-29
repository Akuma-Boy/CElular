using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Image cooldownImage;
    [SerializeField] private MultiMissilController missilController;

    private void Start()
    {
        if (cooldownImage != null)
            cooldownImage.fillAmount = 0f;
    }

    private void Update()
    {
        if (cooldownImage == null || missilController == null) return;

        float tempoDesdeDisparo = Time.time - missilController.UltimoDisparo;
        float progresso = Mathf.Clamp01(tempoDesdeDisparo / missilController.Cooldown);

        cooldownImage.fillAmount = 1f - progresso;
    }
}
