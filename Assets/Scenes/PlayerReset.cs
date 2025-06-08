using UnityEngine;

public class PlayerReset : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody2D rb;

    private void Awake()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        rb = GetComponent<Rigidbody2D>();
    }

    public void ResetPlayer()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // Reseta animações, estados de ataque, etc, se necessário
    }
}
