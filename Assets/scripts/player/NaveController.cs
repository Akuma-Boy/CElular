using UnityEngine;

public class NaveController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float speed = 5f;
    public float boundaryPadding = 0.5f;

    [Header("Configurações de Tiro")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    [Header("Configurações de Animação")]
    public Animator animator;
    private bool movendoCima = false;
    private bool movendoBaixo = false;

    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;
    public TiroMultiplo tiroMultiplo;

    private void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        objectWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        objectHeight = GetComponent<SpriteRenderer>().bounds.extents.y;

        if (tiroMultiplo == null)
            tiroMultiplo = GetComponent<TiroMultiplo>();

        // Cria firePoint automaticamente se não existir
        if (firePoint == null)
        {
            firePoint = new GameObject("FirePoint").transform;
            firePoint.SetParent(transform);
            firePoint.localPosition = new Vector3(0.5f, 0, 0);
            firePoint.localRotation = Quaternion.identity;
        }

        // Verifica se há Animator
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Nenhum Animator encontrado na nave!");
            }
        }
    }

    private void Update()
    {
        MoveShip();
        UpdateAnimation();

        if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && Time.time > nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void MoveShip()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            transform.position = Vector2.Lerp(transform.position, touchPosition, speed * Time.deltaTime);

            // Detecta movimento vertical no touch
            float deltaY = touch.deltaPosition.y;
            movendoCima = deltaY > 10f;
            movendoBaixo = deltaY < -10f;
        }
        else
        {
            float moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            float moveY = Input.GetAxis("Vertical") * speed * Time.deltaTime;
            transform.Translate(moveX, moveY, 0);

            // Controles específicos para animação
            movendoCima = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            movendoBaixo = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        }

        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -screenBounds.x + objectWidth + boundaryPadding, screenBounds.x - objectWidth - boundaryPadding);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -screenBounds.y + objectHeight + boundaryPadding, screenBounds.y - objectHeight - boundaryPadding);
        transform.position = clampedPosition;
    }

    private void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("Cima", movendoCima);
            animator.SetBool("Baixo", movendoBaixo);
        }
    }

    private void Shoot()
    {
        if (tiroMultiplo != null)
        {
            tiroMultiplo.Atirar();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(firePoint.position, firePoint.position + Vector3.right * 2);
        }
    }
}