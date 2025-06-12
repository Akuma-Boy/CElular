using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxBackground : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float velocidadeBase = 1f; 
    public float multiplicadorVelocidade = 1f; 

    [Header("Progressão de Velocidade (Autônoma)")]
    [Tooltip("Se TRUE, este parallax aumentará a velocidade por conta própria ao longo do tempo.")]
    public bool habilitarProgressaoAutonoma = false; // Novo: Para ativar/desativar a progressão própria
    [Tooltip("Tempo em segundos para atingir a velocidade máxima de progressão.")]
    public float tempoTotalProgressao = 300f; // Novo: Tempo total para a progressão (eixo X)
    [Tooltip("Velocidade máxima que este parallax pode atingir através da progressão autônoma.")]
    public float velocidadeMaximaProgressao = 5f; // Novo: Velocidade máxima da progressão
    [Tooltip("Curva para controlar como a velocidade aumenta ao longo do tempo (linear, rápido no início, etc.).")]
    public AnimationCurve curvaProgressaoVelocidade = AnimationCurve.Linear(0, 0, 1, 1); // Novo: Curva de progressão

    [Header("Repetição Infinita")]
    public bool repeatHorizontal = true;
    public bool repeatVertical = false; 

    private float textureUnitSizeX;
    private SpriteRenderer spriteRend;
    private Vector3 originalStartPosition; // Stores the initial position from the editor
    private float currentSpeed; // This is the actual speed used for movement
    private float tempoDecorridoProgressao = 0f; // Novo: Tempo decorrido para a progressão autônoma

    void Awake()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        
        if(spriteRend.sprite == null)
        {
            Debug.LogError("ParallaxBackground: Sprite não atribuído no SpriteRenderer!", gameObject);
            enabled = false;
            return;
        }

        Sprite sprite = spriteRend.sprite;
        Texture2D tex = sprite.texture;
        textureUnitSizeX = (tex.width / sprite.pixelsPerUnit) * transform.localScale.x;

        originalStartPosition = transform.position; // Store the initial position on Awake

        // Inicializa currentSpeed. Se a progressão autônoma estiver desativada, usa velocidadeBase.
        currentSpeed = velocidadeBase * multiplicadorVelocidade; 
        
        Debug.Log($"ParallaxBackground: {gameObject.name} Awake. Initial currentSpeed: {currentSpeed:F2}");
    }

    // Public method to reset the parallax layer's position and speed
    public void ResetParallax(float initialParallaxSpeed)
    {
        transform.position = originalStartPosition; // Reset to the original position in the editor
        
        // Se a progressão autônoma estiver habilitada, o ResetParallax não deve sobrescrever a velocidade inicial,
        // apenas a posição. A velocidade será controlada pelo Update.
        // Se ela NÃO estiver habilitada, então a velocidade inicial virá de DificuldadeProgressiva.
        if (!habilitarProgressaoAutonoma)
        {
             currentSpeed = initialParallaxSpeed * multiplicadorVelocidade; 
        }
        else
        {
            // Se tiver progressão autônoma, resetamos o tempo decorrido para a progressão.
            tempoDecorridoProgressao = 0f;
            currentSpeed = velocidadeBase * multiplicadorVelocidade; // Começa da velocidade base própria
        }
        Debug.Log($"ParallaxBackground: {gameObject.name} Resetado. Posição: {transform.position}, Velocidade inicial aplicada: {currentSpeed:F2} (via DificuldadeProgressiva ou própria base)");
    }

    void Update()
    {
        // Se a progressão autônoma estiver habilitada, ajuste a velocidade aqui.
        if (habilitarProgressaoAutonoma)
        {
            if (tempoTotalProgressao > 0) // Evita divisão por zero
            {
                tempoDecorridoProgressao += Time.deltaTime;
                float progress = Mathf.Clamp01(tempoDecorridoProgressao / tempoTotalProgressao);
                float evaluatedProgress = curvaProgressaoVelocidade.Evaluate(progress);

                // Interpola entre a velocidade base e a velocidade máxima de progressão
                currentSpeed = Mathf.Lerp(velocidadeBase, velocidadeMaximaProgressao, evaluatedProgress) * multiplicadorVelocidade;
                // O clamp garante que a velocidade não ultrapasse a máxima definida, mesmo com a curva
                currentSpeed = Mathf.Clamp(currentSpeed, velocidadeBase * multiplicadorVelocidade, velocidadeMaximaProgressao * multiplicadorVelocidade);
            }
            else // Se tempoTotalProgressao for 0, usa a velocidade base imediatamente
            {
                currentSpeed = velocidadeBase * multiplicadorVelocidade;
            }
        }
        // Se habilitarProgressaoAutonoma for FALSE, currentSpeed será controlado externamente (e.g., por DificuldadeProgressiva)
        // ou permanecerá como a velocidade inicial definida no Awake se SetCurrentSpeed nunca for chamado.

        transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);

        if (repeatHorizontal)
        {
            if (transform.position.x < originalStartPosition.x - textureUnitSizeX)
            {
                transform.position = new Vector3(originalStartPosition.x, transform.position.y, transform.position.z);
            }
        }
    }

    // Public method to set the actual speed that the background moves at.
    // Este método ainda pode ser usado pelo DificuldadeProgressiva se habilitarProgressaoAutonoma for FALSE.
    public void SetCurrentSpeed(float newBaseSpeed)
    {
        // Se a progressão autônoma estiver habilitada, este método não deveria ser o principal controlador.
        // Mas podemos permitir que ele sobrescreva temporariamente ou atue como um ajuste.
        // Para a sua necessidade de autonomia, vamos garantir que a progressão autônoma tenha precedência se ativada.
        if (!habilitarProgressaoAutonoma)
        {
            currentSpeed = newBaseSpeed * multiplicadorVelocidade;
            // Debug.Log($"ParallaxBackground: {gameObject.name} Velocidade ajustada por DificuldadeProgressiva para {newBaseSpeed:F2}. Velocidade efetiva: {currentSpeed:F2}");
        }
        else
        {
            // Debug.LogWarning($"ParallaxBackground: SetCurrentSpeed chamado em '{gameObject.name}' mas progressão autônoma está habilitada. A velocidade será definida internamente.");
            // Você pode decidir o que fazer aqui: ignorar, ou talvez usar como um multiplicador adicional, etc.
            // Por simplicidade, se a progressão autônoma está ligada, este método é ignorado ou seu impacto é menor.
        }
    }

    public void SetMultiplicadorVelocidade(float multiplier)
    {
        multiplicadorVelocidade = multiplier;
        // Se a progressão autônoma está ligada, recalcule a velocidade imediatamente.
        if (habilitarProgressaoAutonoma)
        {
            // Isso garante que a velocidade atual seja reajustada com o novo multiplicador
            // sem ter que esperar o próximo Update.
            float progress = Mathf.Clamp01(tempoDecorridoProgressao / tempoTotalProgressao);
            float evaluatedProgress = curvaProgressaoVelocidade.Evaluate(progress);
            currentSpeed = Mathf.Lerp(velocidadeBase, velocidadeMaximaProgressao, evaluatedProgress) * multiplicadorVelocidade;
            currentSpeed = Mathf.Clamp(currentSpeed, velocidadeBase * multiplicadorVelocidade, velocidadeMaximaProgressao * multiplicadorVelocidade);
        }
    }
}