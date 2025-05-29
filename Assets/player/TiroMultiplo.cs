using UnityEngine;

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
    public Transform firePoint;

    [Header("Sistema de Stamina")]
    public float staminaMax = 100f;
    public float staminaAtual = 100f;
    public float custoPorTiro = 10f;
    public float recargaPorSegundo = 20f;
    public bool sobrecarregado = false;
    public float limiteMinimoParaDisparo = 20f;
    public float GetStaminaNormalized()
    {
        return Mathf.Clamp01(staminaAtual / staminaMax);
    }




    private void Update()
    {
        


        // Regeneração da stamina ao longo do tempo
        if (staminaAtual < staminaMax)
        {
            staminaAtual += recargaPorSegundo * Time.deltaTime;
            staminaAtual = Mathf.Min(staminaAtual, staminaMax);
        }

        if (sobrecarregado && staminaAtual >= limiteMinimoParaDisparo)
        {
            sobrecarregado = false;
        }
    }



    public void Atirar()
    {

        if (sobrecarregado || staminaAtual < custoPorTiro)
            return;

        // Consome stamina
        staminaAtual -= custoPorTiro;
        if (staminaAtual < limiteMinimoParaDisparo)
            sobrecarregado = true;

        // SISTEMA DE TIRO
        switch (quantidadeTiros)
        {
            case 1:
                CriarTiro(firePoint.position, Vector2.right);
                break;

            case 2:
                CriarTiro(firePoint.position + Vector3.up * espacamentoVertical, Vector2.right);
                CriarTiro(firePoint.position + Vector3.down * espacamentoVertical, Vector2.right);
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
    }


    private void CriarTiro(Vector3 posicao, Vector2 direcao)
    {
        GameObject proj = Instantiate(projetilPrefab, posicao, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direcao.normalized * projectileSpeed;
        Destroy(proj, 3f);
    }


    public void AumentarTiro()
    {
        quantidadeTiros = Mathf.Min(quantidadeTiros + 1, maxTiros);
    }

    public void ReduzirTiro()
    {
        quantidadeTiros = Mathf.Max(quantidadeTiros - 1, minTiros);
    }
}
