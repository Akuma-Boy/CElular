using UnityEngine;
using UnityEngine.EventSystems; // Necessário para IPoinerDownHandler e IPointerUpHandler

public class UIButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Eventos que outros scripts podem assinar
    public event System.Action OnButtonDown;
    public event System.Action OnButtonUp;

    // Propriedade para verificar o estado atual do botão
    public bool IsPressed { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
        OnButtonDown?.Invoke(); // Dispara o evento OnButtonDown
        Debug.Log($"<color=green>[UIButtonHandler] Botão {gameObject.name} Pressionado.</color>");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
        OnButtonUp?.Invoke(); // Dispara o evento OnButtonUp
        Debug.Log($"<color=red>[UIButtonHandler] Botão {gameObject.name} Solto.</color>");
    }

    // Opcional: Adicione um método para resetar o estado se necessário (embora PointerUp deva lidar com isso)
    private void OnDisable()
    {
        // Garante que o botão seja considerado "solto" se o GameObject for desativado
        if (IsPressed)
        {
            IsPressed = false;
            OnButtonUp?.Invoke();
        }
    }
}