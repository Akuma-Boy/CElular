using UnityEngine;
using UnityEngine.EventSystems; // Remova se não estiver mais usando EventTrigger diretamente para botões de movimento
using System.Collections.Generic; // Mantenha se precisar de List para MultiMissilController

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

    // Referências aos seus botões da UI (arrastar manualmente no Inspector)
    [Header("Botões de Movimento UI")]
    public UIButtonHandler btnMoveUp;
    public UIButtonHandler btnMoveDown;
    public UIButtonHandler btnMoveLeft;
    public UIButtonHandler btnMoveRight;
    public UIButtonHandler btnShoot; // Para o botão de atirar
    public UIButtonHandler btnSpecialAbility; // Para o botão "E"

    // Variáveis para controle de movimento via UI (mantemos, mas elas serão setadas pelo NaveController)
    private bool moveUpUI;
    private bool moveDownUI;
    private bool moveLeftUI;
    private bool moveRightUI;
    private bool shootUI; 
    private bool specialAbilityUI;

    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;

    public TiroMultiplo tiroMultiplo;
    public MultiMissilController multiMissilController; // Adicione esta referência

    private void Awake()
    {
        // Garante que a lógica de subscribe/unsubscribe seja feita corretamente
        SubscribeToUIButtons();
    }

    private void OnDestroy()
    {
        // Importante: Desinscrever para evitar erros quando a cena é recarregada ou objetos destruídos
        UnsubscribeFromUIButtons();
    }

    private void SubscribeToUIButtons()
    {
        if (btnMoveUp != null) { btnMoveUp.OnButtonDown += () => moveUpUI = true; btnMoveUp.OnButtonUp += () => moveUpUI = false; }
        if (btnMoveDown != null) { btnMoveDown.OnButtonDown += () => moveDownUI = true; btnMoveDown.OnButtonUp += () => moveDownUI = false; }
        if (btnMoveLeft != null) { btnMoveLeft.OnButtonDown += () => moveLeftUI = true; btnMoveLeft.OnButtonUp += () => moveLeftUI = false; }
        if (btnMoveRight != null) { btnMoveRight.OnButtonDown += () => moveRightUI = true; btnMoveRight.OnButtonUp += () => moveRightUI = false; }
        
        if (btnShoot != null) { btnShoot.OnButtonDown += () => shootUI = true; btnShoot.OnButtonUp += () => shootUI = false; }
        
        // Para a habilidade especial, você pode querer que seja um único clique (OnButtonDown)
        // ou que ative enquanto pressionado (OnButtonDown/OnButtonUp).
        // Se for um clique único:
        if (btnSpecialAbility != null) { btnSpecialAbility.OnButtonDown += AtivarHabilidadeEspecial; }
        // Se for para ativar/desativar ao pressionar/soltar:
        // if (btnSpecialAbility != null) { btnSpecialAbility.OnButtonDown += () => specialAbilityUI = true; btnSpecialAbility.OnButtonUp += () => specialAbilityUI = false; }
    }

    private void UnsubscribeFromUIButtons()
    {
        if (btnMoveUp != null) { btnMoveUp.OnButtonDown -= () => moveUpUI = true; btnMoveUp.OnButtonUp -= () => moveUpUI = false; }
        if (btnMoveDown != null) { btnMoveDown.OnButtonDown -= () => moveDownUI = true; btnMoveDown.OnButtonUp -= () => moveDownUI = false; }
        if (btnMoveLeft != null) { btnMoveLeft.OnButtonDown -= () => moveLeftUI = true; btnMoveLeft.OnButtonUp -= () => moveLeftUI = false; }
        if (btnMoveRight != null) { btnMoveRight.OnButtonDown -= () => moveRightUI = true; btnMoveRight.OnButtonUp -= () => moveRightUI = false; }

        if (btnShoot != null) { btnShoot.OnButtonDown -= () => shootUI = true; btnShoot.OnButtonUp -= () => shootUI = false; }

        if (btnSpecialAbility != null) { btnSpecialAbility.OnButtonDown -= AtivarHabilidadeEspecial; }
    }


    private void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        objectWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        objectHeight = GetComponent<SpriteRenderer>().bounds.extents.y;

        if (tiroMultiplo == null)
            tiroMultiplo = GetComponent<TiroMultiplo>();
        if (multiMissilController == null) // Tenta encontrar o MultiMissilController
            multiMissilController = GetComponent<MultiMissilController>();

        // Cria firePoint automaticamente se não existir
        if (firePoint == null)
        {
            firePoint = new GameObject("FirePoint").transform;
            firePoint.SetParent(transform);
            firePoint.localPosition = new Vector3(0.5f, 0, 0); // Ajuste conforme necessário
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
        // Controles de Teclado (ainda funcionam)
        if ((Input.GetKey(KeyCode.Space) || shootUI) && Time.time > nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        // Habilidade Especial (Tecla E ou botão UI)
        if (Input.GetKeyDown(KeyCode.E)) // Agora o Input.GetKeyDown(KeyCode.E) está separado da UI
        {
            if (multiMissilController != null)
            {
                multiMissilController.AtivarHabilidadeMissilGuiado();
            }
        }
        // A lógica do botão UI para habilidade especial é tratada pelo AtivarHabilidadeEspecial()
        
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        MoveShip();
    }


    private void MoveShip()
    {
        Vector3 moveDirection = Vector3.zero;

        // Prioriza inputs de teclado/setas OU inputs da UI
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || moveUpUI)
        {
            moveDirection += Vector3.up;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || moveDownUI)
        {
            moveDirection += Vector3.down;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || moveLeftUI)
        {
            moveDirection += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || moveRightUI)
        {
            moveDirection += Vector3.right;
        }
        
        // Normaliza o vetor para que o movimento diagonal não seja mais rápido
        moveDirection.Normalize();

        transform.Translate(moveDirection * speed * Time.fixedDeltaTime);

        // Atualiza as flags de animação baseadas no movimento final (teclado ou UI)
        movendoCima = (moveDirection.y > 0.1f);
        movendoBaixo = (moveDirection.y < -0.1f);


        // Limita a posição dentro da tela
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
        else if (projectilePrefab != null && firePoint != null) 
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = firePoint.right * projectileSpeed;
            }
        }
    }

    // Método a ser chamado pelo botão de habilidade especial 'E'
    private void AtivarHabilidadeEspecial()
    {
        if (multiMissilController != null)
        {
            multiMissilController.AtivarHabilidadeMissilGuiado();
        }
        else
        {
            Debug.LogWarning("[NaveController] MultiMissilController não encontrado para ativar habilidade especial.");
        }
        specialAbilityUI = false; // Garante que a flag seja resetada após o clique
    }

    // --- Métodos Públicos anteriores para botões NÃO SÃO MAIS NECESSÁRIOS AQUI ---
    // Eles foram substituídos pela assinatura de eventos do UIButtonHandler.
    // public void OnPressMoveUp() { moveUpUI = true; } // ESTES PODEM SER REMOVIDOS
    // ... e os outros OnPress/OnRelease
    // public void OnPressSpecialAbility() { specialAbilityUI = true; } 
    // public void OnReleaseSpecialAbility() { specialAbilityUI = false; }
}