using UnityEngine;
using System.Collections.Generic;

public class TiroMultiplo : MonoBehaviour
{
    [Header("Configuração de Tiro")]
    [Range(1, 5)] public int quantidadeTiros = 1;
    public int maxTiros = 5;
    public int minTiros = 1;
    public float anguloDistribuicao = 45f;
    public float espacamentoVertical = 0.4f;
    public GameObject projetilPrefab;
    public float projectileSpeed = 10f;
    public Transform firePoint; // Já existe, mas vamos garantir sua atribuição

    [Header("Sistema de Stamina")]
    public float staminaMax = 100f;
    public float staminaAtual = 100f;
    public float custoPorTiro = 10f;
    public float recargaPorSegundo = 20f;
    public bool sobrecarregado = false;
    public float limiteMinimoParaDisparo = 20f;

    // Getter para a stamina normalizada
    public float GetStaminaNormalized()
    {
        return Mathf.Clamp01(staminaAtual / staminaMax);
    }

    private void Start()
    {
        // Garante que o firePoint esteja atribuído.
        // Se firePoint não foi arrastado no Inspector, tenta encontrar um filho chamado "FirePoint"
        // Ou assume que ele está no próprio GameObject se não encontrar.
        if (firePoint == null)
        {
            Transform foundFirePoint = transform.Find("FirePoint");
            if (foundFirePoint != null)
            {
                firePoint = foundFirePoint;
                Debug.Log("<color=green>[TiroMultiplo] FirePoint encontrado como filho 'FirePoint'.</color>");
            }
            else
            {
                // Se não encontrar um filho específico, assume que o ponto de disparo é o próprio GameObject da nave.
                // Isso pode ser ajustado se o seu firePoint tiver uma hierarquia diferente.
                firePoint = transform; 
                Debug.LogWarning("<color=orange>[TiroMultiplo] FirePoint não atribuído no Inspector e não encontrado como filho 'FirePoint'. Usando a própria transformação da nave como FirePoint.</color>");
            }
        }
    }

    private void Update()
    {
        // Regeneração da stamina ao longo do tempo
        if (staminaAtual < staminaMax)
        {
            staminaAtual += recargaPorSegundo * Time.deltaTime;
            staminaAtual = Mathf.Min(staminaAtual, staminaMax);
        }

        // Reseta o estado de sobrecarregado se a stamina for suficiente
        if (sobrecarregado && staminaAtual >= limiteMinimoParaDisparo)
        {
            sobrecarregado = false;
        }
    }

    public void Atirar()
    {
        if (firePoint == null)
        {
            Debug.LogError("<color=red>[TiroMultiplo] FirePoint é NULL! Não é possível atirar. Verifique a configuração.</color>");
            return;
        }

        if (sobrecarregado || staminaAtual < custoPorTiro)
        {
            Debug.Log("<color=yellow>[TiroMultiplo] Não é possível atirar: Sobrecarregado ou stamina insuficiente.</color>");
            return;
        }

        // Consome stamina
        staminaAtual -= custoPorTiro;
        if (staminaAtual < limiteMinimoParaDisparo)
        {
            sobrecarregado = true;
            Debug.Log("<color=red>[TiroMultiplo] Nave sobrecarregada! Stamina abaixo do limite.</color>");
        }

        // SISTEMA DE TIRO
        switch (quantidadeTiros)
        {
            case 1:
                CriarTiro(firePoint.position, Vector2.right);
                break;

            case 2:
                CriarTiro(firePoint.position + Vector3.up * espacamentoVertical / 2f, Vector2.right);
                CriarTiro(firePoint.position + Vector3.down * espacamentoVertical / 2f, Vector2.right);
                break;

            case 3:
                CriarTiro(firePoint.position + Vector3.up * espacamentoVertical, Vector2.right);
                CriarTiro(firePoint.position, Vector2.right);
                CriarTiro(firePoint.position + Vector3.down * espacamentoVertical, Vector2.right);
                break;

            case 4:
            case 5:
                float anguloInicial = -anguloDistribuicao / 2f;
                float incremento = anguloDistribuicao / (quantidadeTiros - 1);
                for (int i = 0; i < quantidadeTiros; i++)
                {
                    float angulo = anguloInicial + i * incremento;
                    Vector2 direcao = Quaternion.Euler(0, 0, angulo) * Vector2.right;
                    CriarTiro(firePoint.position, direcao);
                }
                break;
        }
        Debug.Log($"<color=blue>[TiroMultiplo] Disparado {quantidadeTiros} tiro(s). Stamina restante: {staminaAtual:F1}</color>");
    }

    private void CriarTiro(Vector3 posicao, Vector2 direcao)
    {
        GameObject proj = Instantiate(projetilPrefab, posicao, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direcao.normalized * projectileSpeed;
        }
        else
        {
            Debug.LogWarning("Projetil prefab não possui Rigidbody2D. Certifique-se de que o prefab de projétil tenha um Rigidbody2D.");
        }
        Destroy(proj, 3f); // Destrói o projétil após 3 segundos
    }

    public void AumentarTiro()
    {
        quantidadeTiros = Mathf.Min(quantidadeTiros + 1, maxTiros);
        Debug.Log("<color=green>[TiroMultiplo] Nível de tiro aumentado para: " + quantidadeTiros + "</color>");
    }

    public void ReduzirTiro()
    {
        // Garante que o nível de tiro não caia abaixo do mínimo
        quantidadeTiros = Mathf.Max(quantidadeTiros - 1, minTiros);
        Debug.Log("<color=orange>[TiroMultiplo] Nível de tiro reduzido para: " + quantidadeTiros + "</color>");
    }

    // Método para ser chamado para resetar o sistema de tiro (por exemplo, após Game Over ou início de fase)
    public void ResetTiro()
    {
        quantidadeTiros = minTiros; // Reseta para o nível de tiro inicial
        staminaAtual = staminaMax; // Reseta a stamina para o máximo
        sobrecarregado = false; // Garante que não esteja sobrecarregado
        Debug.Log("<color=cyan>[TiroMultiplo] Sistema de tiro resetado para o nível " + minTiros + " e stamina cheia.</color>");
    }
}