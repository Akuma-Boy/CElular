using UnityEngine;
using System.Collections.Generic;

public class ParallaxManager : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layerTransform;
        public float parallaxMultiplier = 0.5f;
        public bool repeatHorizontal = true;
        public bool repeatVertical = false;
        public bool isForeground = false;
    }

    [Header("Configurações")]
    [SerializeField] private Transform mainCamera;
    [SerializeField] private List<ParallaxLayer> layers = new List<ParallaxLayer>();

    private Vector3 lastCameraPosition;
    private Dictionary<Transform, Vector3> initialPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, float> textureUnitSizesX = new Dictionary<Transform, float>();
    private Dictionary<Transform, float> textureUnitSizesY = new Dictionary<Transform, float>();

    private void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main.transform;
        lastCameraPosition = mainCamera.position;

        foreach (var layer in layers)
        {
            // Salva posições iniciais
            initialPositions[layer.layerTransform] = layer.layerTransform.position;

            // Calcula tamanhos das texturas para repetição
            var spriteRenderer = layer.layerTransform.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                Sprite sprite = spriteRenderer.sprite;
                Texture2D texture = sprite.texture;
                textureUnitSizesX[layer.layerTransform] = texture.width / sprite.pixelsPerUnit * layer.layerTransform.localScale.x;
                textureUnitSizesY[layer.layerTransform] = texture.height / sprite.pixelsPerUnit * layer.layerTransform.localScale.y;
            }
        }
    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = mainCamera.position - lastCameraPosition;

        foreach (var layer in layers)
        {
            if (layer.layerTransform == null) continue;

            // Aplica parallax
            float parallaxX = deltaMovement.x * layer.parallaxMultiplier;
            float parallaxY = deltaMovement.y * layer.parallaxMultiplier;

            // Inverte o movimento para foreground
            if (layer.isForeground)
            {
                parallaxX *= -1f;
                parallaxY *= -1f;
            }

            Vector3 newPosition = layer.layerTransform.position + new Vector3(parallaxX, parallaxY, 0);
            layer.layerTransform.position = newPosition;

            // Verifica repetição
            if (layer.repeatHorizontal && textureUnitSizesX.ContainsKey(layer.layerTransform))
            {
                float textureUnitSizeX = textureUnitSizesX[layer.layerTransform];
                float diffX = mainCamera.position.x - layer.layerTransform.position.x;
                if (Mathf.Abs(diffX) >= textureUnitSizeX)
                {
                    float offsetX = diffX % textureUnitSizeX;
                    layer.layerTransform.position = new Vector3(
                        mainCamera.position.x + offsetX,
                        layer.layerTransform.position.y,
                        layer.layerTransform.position.z);
                }
            }

            if (layer.repeatVertical && textureUnitSizesY.ContainsKey(layer.layerTransform))
            {
                float textureUnitSizeY = textureUnitSizesY[layer.layerTransform];
                float diffY = mainCamera.position.y - layer.layerTransform.position.y;
                if (Mathf.Abs(diffY) >= textureUnitSizeY)
                {
                    float offsetY = diffY % textureUnitSizeY;
                    layer.layerTransform.position = new Vector3(
                        layer.layerTransform.position.x,
                        mainCamera.position.y + offsetY,
                        layer.layerTransform.position.z);
                }
            }
        }

        lastCameraPosition = mainCamera.position;
    }

    // Método para resetar todas as posições (útil para reiniciar cena)
    public void ResetAllPositions()
    {
        foreach (var layer in layers)
        {
            if (layer.layerTransform != null && initialPositions.ContainsKey(layer.layerTransform))
            {
                layer.layerTransform.position = initialPositions[layer.layerTransform];
            }
        }
        lastCameraPosition = mainCamera.position;
    }
}
