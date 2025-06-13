using UnityEngine;
using TMPro; // Usar TextMeshPro se você estiver usando-o para seu campo de entrada
using UnityEngine.UI; // Usar para InputField (legacy UI)
using UnityEngine.EventSystems; // Para detectar cliques na UI

public class MobileKeyboardOpener : MonoBehaviour, IPointerClickHandler
{
    private TouchScreenKeyboard keyboard;
    
    // Referência para o campo de entrada onde o texto será digitado
    // Use TextMeshProUGUI para TMPro ou InputField para o UI legado
    public TMP_InputField inputFieldTMP; // Se estiver usando TextMeshPro
    public InputField inputFieldLegacy;  // Se estiver usando o sistema de UI antigo

    private void Awake()
    {
        // Tenta pegar a referência ao componente de input na Awake
        // Isso é mais robusto do que FindObjectOfType e evita erros de Nulo.
        if (inputFieldTMP == null)
        {
            inputFieldTMP = GetComponent<TMP_InputField>();
        }
        if (inputFieldLegacy == null)
        {
            inputFieldLegacy = GetComponent<InputField>();
        }

        if (inputFieldTMP == null && inputFieldLegacy == null)
        {
            Debug.LogError("<color=red>MobileKeyboardOpener: Nenhum componente TMP_InputField ou InputField encontrado neste GameObject. O teclado não funcionará.</color>");
            this.enabled = false; // Desativa o script se não encontrar o componente
        }
    }

    // Este método é chamado quando o GameObject é clicado (graças a IPointerClickHandler)
    public void OnPointerClick(PointerEventData eventData)
    {
        OpenKeyboard();
    }

    public void OpenKeyboard()
    {
        // Certifique-se de que o campo de entrada não é nulo antes de tentar abrir o teclado
        if (inputFieldTMP == null && inputFieldLegacy == null)
        {
            Debug.LogWarning("<color=orange>MobileKeyboardOpener: Não é possível abrir o teclado porque não há um campo de entrada atribuído.</color>");
            return;
        }

        // Abre o teclado virtual
        // Você pode configurar o tipo de teclado (text, number, url, etc.)
        // e se o teclado deve ser multi-linha ou não.
        keyboard = TouchScreenKeyboard.Open(
            "", // Texto inicial (você pode usar inputFieldTMP.text ou inputFieldLegacy.text aqui se quiser preencher)
            TouchScreenKeyboardType.Default, // Tipo de teclado: Default (alfanumérico), NumberPad, EmailAddress, etc.
            false, // Autocorrect
            false, // Multi-line (false para uma única linha de nick/nome)
            false, // IsPassword
            false, // Alert
            "", // Placeholder (texto que aparece no campo se estiver vazio)
            0 // Caracteres máximos (0 significa ilimitado, use o limite do seu InputField)
        );

        Debug.Log("<color=green>MobileKeyboardOpener: Teclado virtual aberto.</color>");
    }

    private void Update()
    {
        // Se o teclado está ativo e foi digitado algo
        if (keyboard != null && keyboard.status == TouchScreenKeyboard.Status.Done)
        {
            // Atribui o texto digitado ao campo de entrada
            if (inputFieldTMP != null)
            {
                inputFieldTMP.text = keyboard.text;
            }
            else if (inputFieldLegacy != null)
            {
                inputFieldLegacy.text = keyboard.text;
            }
            keyboard = null; // Limpa a referência ao teclado
            Debug.Log("<color=green>MobileKeyboardOpener: Teclado fechado. Texto digitado: " + (inputFieldTMP != null ? inputFieldTMP.text : inputFieldLegacy.text) + "</color>");
        }
        else if (keyboard != null && keyboard.status == TouchScreenKeyboard.Status.Canceled)
        {
            // O teclado foi cancelado (pelo usuário, ex: botão "voltar" em Android)
            keyboard = null;
            Debug.Log("<color=orange>MobileKeyboardOpener: Teclado virtual cancelado pelo usuário.</color>");
        }
    }
}