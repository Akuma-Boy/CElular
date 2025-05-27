using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxBackground : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [Tooltip("Velocidade inicial do movimento")]
    public float initialScrollSpeed = 1f;
    
    [Header("Configurações de Progressão")]
    [Tooltip("Velocidade máxima que pode alcançar")]
    public float maxScrollSpeed = 5f;
    [Tooltip("Tempo total para alcançar a velocidade máxima (em segundos)")]
    public float timeToMaxSpeed = 180f;
    
    [Header("Repetição Infinita")]
    public bool repeatHorizontal = true;
    public bool repeatVertical = false;

    private float textureUnitSizeX;
    private float textureUnitSizeY;
    private SpriteRenderer spriteRend;
    private Vector3 startPosition;
    private float elapsedTime;
    private float currentSpeed;

    void Awake()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        
        if(spriteRend.sprite == null)
        {
            Debug.LogError("Sprite não atribuído no SpriteRenderer!", gameObject);
            enabled = false;
            return;
        }

        // Calcula o tamanho da textura em unidades do mundo
        Sprite sprite = spriteRend.sprite;
        Texture2D tex = sprite.texture;
        textureUnitSizeX = (tex.width / sprite.pixelsPerUnit) * transform.localScale.x;
        textureUnitSizeY = (tex.height / sprite.pixelsPerUnit) * transform.localScale.y;

        startPosition = transform.position;
        currentSpeed = initialScrollSpeed;
    }

    void Update()
    {
        // Atualiza a velocidade progressivamente
        elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsedTime / timeToMaxSpeed);
        currentSpeed = Mathf.Lerp(initialScrollSpeed, maxScrollSpeed, progress);

        // Movimento automático para a esquerda
        float newPosition = Mathf.Repeat(Time.time * currentSpeed, textureUnitSizeX);
        transform.position = startPosition + Vector3.left * newPosition;

        // Repetição infinita
        if(repeatHorizontal)
        {
            if(Mathf.Abs(transform.position.x - startPosition.x) >= textureUnitSizeX)
            {
                transform.position = startPosition;
            }
        }
    }

    // Método para reiniciar a progressão (opcional)
    public void ResetSpeedProgression()
    {
        elapsedTime = 0f;
        currentSpeed = initialScrollSpeed;
    }

    // Método para ajustar manualmente a velocidade (opcional)
    public void SetCurrentSpeed(float newSpeed)
    {
        currentSpeed = Mathf.Clamp(newSpeed, initialScrollSpeed, maxScrollSpeed);
    }
}