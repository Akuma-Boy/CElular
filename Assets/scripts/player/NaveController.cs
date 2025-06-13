using UnityEngine;
using System.Collections.Generic;

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

    [Header("Botões de Movimento UI")]
    public UIButtonHandler btnMoveUp;
    public UIButtonHandler btnMoveDown;
    public UIButtonHandler btnMoveLeft;
    public UIButtonHandler btnMoveRight;
    public UIButtonHandler btnShoot;
    public UIButtonHandler btnSpecialAbility;

    // Variáveis para controle de movimento via UI
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
    public MultiMissilController multiMissilController;

    // Nova variável para controlar se o teclado está habilitado
    private bool keyboardEnabled = true;

    private void Awake()
    {
        SubscribeToUIButtons();
    }

    private void OnDestroy()
    {
        UnsubscribeFromUIButtons();
    }

    private void SubscribeToUIButtons()
    {
        if (btnMoveUp != null) { 
            btnMoveUp.OnButtonDown += () => moveUpUI = true; 
            btnMoveUp.OnButtonUp += () => moveUpUI = false; 
        }
        if (btnMoveDown != null) { 
            btnMoveDown.OnButtonDown += () => moveDownUI = true; 
            btnMoveDown.OnButtonUp += () => moveDownUI = false; 
        }
        if (btnMoveLeft != null) { 
            btnMoveLeft.OnButtonDown += () => moveLeftUI = true; 
            btnMoveLeft.OnButtonUp += () => moveLeftUI = false; 
        }
        if (btnMoveRight != null) { 
            btnMoveRight.OnButtonDown += () => moveRightUI = true; 
            btnMoveRight.OnButtonUp += () => moveRightUI = false; 
        }
        
        if (btnShoot != null) { 
            btnShoot.OnButtonDown += () => shootUI = true; 
            btnShoot.OnButtonUp += () => shootUI = false; 
        }
        
        if (btnSpecialAbility != null) { 
            btnSpecialAbility.OnButtonDown += AtivarHabilidadeEspecial; 
        }
    }

    private void UnsubscribeFromUIButtons()
    {
        if (btnMoveUp != null) { 
            btnMoveUp.OnButtonDown -= () => moveUpUI = true; 
            btnMoveUp.OnButtonUp -= () => moveUpUI = false; 
        }
        // ... (desinscrever os outros botões da mesma forma)
    }

    private void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        objectWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        objectHeight = GetComponent<SpriteRenderer>().bounds.extents.y;

        if (tiroMultiplo == null)
            tiroMultiplo = GetComponent<TiroMultiplo>();
        if (multiMissilController == null)
            multiMissilController = GetComponent<MultiMissilController>();

        if (firePoint == null)
        {
            firePoint = new GameObject("FirePoint").transform;
            firePoint.SetParent(transform);
            firePoint.localPosition = new Vector3(0.5f, 0, 0);
            firePoint.localRotation = Quaternion.identity;
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        // Verifica se o teclado está sendo usado
        CheckKeyboardUsage();

        // Controles de Tiro (teclado e UI)
        if ((keyboardEnabled && Input.GetKey(KeyCode.Space) || shootUI) && Time.time > nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        // Habilidade Especial (teclado e UI)
        if (keyboardEnabled && Input.GetKeyDown(KeyCode.E))
        {
            AtivarHabilidadeEspecial();
        }
        
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        MoveShip();
    }

    // Novo método para verificar se o teclado está sendo usado
    private void CheckKeyboardUsage()
    {
        if (Input.anyKeyDown)
        {
            keyboardEnabled = true;
        }
    }

    private void MoveShip()
    {
        Vector3 moveDirection = Vector3.zero;

        // Movimento com teclado (se habilitado)
        if (keyboardEnabled)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) moveDirection += Vector3.up;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) moveDirection += Vector3.down;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveDirection += Vector3.left;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveDirection += Vector3.right;
        }

        // Movimento com UI (sempre habilitado)
        if (moveUpUI) moveDirection += Vector3.up;
        if (moveDownUI) moveDirection += Vector3.down;
        if (moveLeftUI) moveDirection += Vector3.left;
        if (moveRightUI) moveDirection += Vector3.right;
        
        moveDirection.Normalize();

        transform.Translate(moveDirection * speed * Time.fixedDeltaTime);

        // Atualiza flags de animação
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
                // Correção aplicada aqui - substituído velocity por linearVelocity
                rb.linearVelocity = firePoint.right * projectileSpeed;
            }
        }
    }

    private void AtivarHabilidadeEspecial()
    {
        if (multiMissilController != null)
        {
            multiMissilController.AtivarHabilidadeMissilGuiado();
        }
        specialAbilityUI = false;
    }
}