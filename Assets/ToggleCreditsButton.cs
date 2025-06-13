using UnityEngine;
using UnityEngine.UI; // Necessário se o script estiver em um botão UI para associar a OnClick

public class ToggleCreditsButton : MonoBehaviour
{
    [Tooltip("Arraste o GameObject 'creditos' (a imagem/objeto que você quer ligar/desligar) aqui.")]
    public GameObject creditsGameObject;

    private void Start()
    {
        // Tenta encontrar o GameObject "creditos" se não foi atribuído no Inspector
        // Esta busca pode ser lenta, é sempre melhor atribuir no Inspector se possível.
        if (creditsGameObject == null)
        {
            // Tenta encontrar pelo nome exato. Ajuste "creditos" se o nome for diferente.
            creditsGameObject = GameObject.Find("creditos"); 
            if (creditsGameObject == null)
            {
                Debug.LogWarning("<color=orange>ToggleCreditsButton em " + gameObject.name + ": O GameObject 'creditos' não foi encontrado na cena. Por favor, atribua-o manualmente no Inspector.</color>");
                // Opcional: Desativar o script se a referência for nula para evitar erros.
                // this.enabled = false;
            }
        }

        // Garante que o painel de créditos esteja inicialmente desativado (escondido)
        // Isso é uma boa prática para UIs que aparecem sob demanda.
        if (creditsGameObject != null)
        {
            creditsGameObject.SetActive(false);
            Debug.Log("<color=green>ToggleCreditsButton: 'creditos' inicialmente desativado.</color>");
        }
    }

    // Este método será chamado pelo evento OnClick do botão
    public void ToggleCredits()
    {
        if (creditsGameObject != null)
        {
            // Inverte o estado de ativação do GameObject "creditos"
            bool newState = !creditsGameObject.activeSelf;
            creditsGameObject.SetActive(newState);
            Debug.Log($"<color=blue>ToggleCreditsButton: GameObject 'creditos' alterado para {(newState ? "ATIVO" : "INATIVO")}.</color>");
        }
        else
        {
            Debug.LogWarning("<color=orange>ToggleCreditsButton: Não foi possível alternar 'creditos' porque a referência está nula.</color>");
        }
    }
}