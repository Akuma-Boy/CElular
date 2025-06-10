using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxBackground : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float velocidadeBase = 1f;
    public float multiplicadorVelocidade = 1f;

    [Header("Repetição Infinita")]
    public bool repeatHorizontal = true;
    public bool repeatVertical = false;

    private float textureUnitSizeX;
    private float textureUnitSizeY;
    private SpriteRenderer spriteRend;
    private Vector3 startPosition;
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

        Sprite sprite = spriteRend.sprite;
        Texture2D tex = sprite.texture;
        textureUnitSizeX = (tex.width / sprite.pixelsPerUnit) * transform.localScale.x;
        textureUnitSizeY = (tex.height / sprite.pixelsPerUnit) * transform.localScale.y;

        startPosition = transform.position;
        currentSpeed = velocidadeBase * multiplicadorVelocidade;
    }

    void Update()
    {
        // Movimento automático para a esquerda
        float newPosition = Mathf.Repeat(Time.time * currentSpeed, textureUnitSizeX);
        transform.position = startPosition + Vector3.left * newPosition;

        // Repetição infinita
        if(repeatHorizontal && Mathf.Abs(transform.position.x - startPosition.x) >= textureUnitSizeX)
        {
            transform.position = startPosition;
        }
    }

    public void SetCurrentSpeed(float newSpeed)
    {
        currentSpeed = newSpeed * multiplicadorVelocidade;
        Debug.Log($"ParallaxBackground: Velocidade ajustada para {currentSpeed:F2}");
    }


    public void SetMultiplicadorVelocidade(float multiplicador)
    {
        multiplicadorVelocidade = multiplicador;
        currentSpeed = velocidadeBase * multiplicadorVelocidade;
    }
}