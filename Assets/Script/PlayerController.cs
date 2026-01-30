using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public InputAction playerMovement;
    public InputAction playerJump;
    public InputAction attack = new InputAction(binding: "<Keyboard>/space");
    public Rigidbody2D rg;
    public Animator animator;
    public float speed;
    public float fallSpeed;
    public bool onGround;
    public bool moving;
    public bool lookingRight;
    public bool lookingLeft;
    public AnimatorOverrideController rightController;
    public AnimatorOverrideController leftController;
    public bool stunned;
    [SerializeField] public float jumpHeight;

    [Header("Attack - Carrot Shooting")]
    public GameObject carrotPrefab;
    public Transform shootPoint;
    public float shootCooldown = 0.3f;
    private float lastShootTime = 0f;

    [Header("Ammo System")]
    public int maxAmmo = 3;
    public int currentAmmo = 3;
    public float ammoRegenTime = 3f;
    public Image[] ammoImages;
    public Color ammoFullColor = new Color(1f, 0.5f, 0f, 1f);  // Orange
    public Color ammoEmptyColor = new Color(0.3f, 0.3f, 0.3f, 1f);  // Gray
    private Coroutine regenCoroutine;

    private void Awake()
    {
        jumpHeight = 23f;
        onGround = true;
        moving = false;
        lookingRight = true;
        lookingLeft = false;
        stunned = false;
        fallSpeed = 8f;
        speed = 9f;
    }

    private void OnEnable()
    {
        playerMovement.Enable();
        attack.Enable();
        attack.performed += Shoot;
        playerJump.performed += jump;
        playerJump.canceled += cancelJump;
        playerJump.Enable();
    }

    private void OnDisable()
    {
        playerMovement.Disable();
        playerJump.Disable();
        attack.Disable();

        attack.performed -= Shoot;
        playerJump.performed -= jump;
        playerJump.canceled -= cancelJump;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        
        // Create shoot point if not assigned
        if (shootPoint == null)
        {
            GameObject shootPointObj = new GameObject("ShootPoint");
            shootPointObj.transform.SetParent(transform);
            shootPointObj.transform.localPosition = new Vector3(0.8f, 0, 0);
            shootPoint = shootPointObj.transform;
        }

        // Initialize ammo
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    void Update()
    {
        // Update shoot point position based on facing direction
        if (shootPoint != null)
        {
            float xOffset = lookingRight ? 0.8f : -0.8f;
            shootPoint.localPosition = new Vector3(xOffset, 0, 0);
        }
    }
    void FixedUpdate()
    {
        if (lookingRight) {
            animator.runtimeAnimatorController = rightController;
        }
        else {
            animator.runtimeAnimatorController = leftController;
        }
        int direction = playerMovement.ReadValue<Vector2>().x > 0 ? 1 : playerMovement.ReadValue<Vector2>().x < 0 ? -1 : 0;
        if (direction != 0)
        {
            moving = true;
            if (direction == 1)
            {
                lookingRight = true;
                lookingLeft = false;
            }
            else
            {
                lookingRight = false;
                lookingLeft = true;
            }
        }
        else
        {
            moving = false;
        }
        rg.linearVelocity = new Vector2(playerMovement.ReadValue<Vector2>().x * speed, rg.linearVelocity.y);

        
       
        //slow down the jump
        if (rg.linearVelocity.y < -Mathf.Abs(fallSpeed))
        {
            rg.linearVelocity = new Vector2(rg.linearVelocity.x, Mathf.Clamp(rg.linearVelocity.y, -Mathf.Abs(fallSpeed), Mathf.Infinity));
                
        }
          
    }
    void jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump action performed");
        if (onGround)
        {
            Debug.Log("Jumping");
            rg.linearVelocity = new Vector2(rg.linearVelocity.x, jumpHeight);

         

        }
    }
    void cancelJump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump action canceled");
        if (rg.linearVelocity.y > 0)
        {
            rg.linearVelocity = new Vector2(rg.linearVelocity.x, 0);
        }
    }

    void Shoot(InputAction.CallbackContext context)
    {
        // Check cooldown
        if (Time.time - lastShootTime < shootCooldown)
        {
            return;
        }

        // Check ammo
        if (currentAmmo <= 0)
        {
            Debug.Log("No ammo!");
            return;
        }

        // Check if carrot prefab is assigned
        if (carrotPrefab == null)
        {
            Debug.LogWarning("Carrot prefab not assigned to PlayerController!");
            return;
        }

        lastShootTime = Time.time;

        // Consume ammo
        currentAmmo--;
        UpdateAmmoUI();

        // Start regen if not already running
        if (regenCoroutine == null)
        {
            regenCoroutine = StartCoroutine(RegenAmmo());
        }

        // Determine shoot direction
        Vector2 shootDirection = lookingRight ? Vector2.right : Vector2.left;

        // Spawn carrot
        Vector3 spawnPosition = shootPoint != null ? shootPoint.position : transform.position;
        GameObject carrot = Instantiate(carrotPrefab, spawnPosition, Quaternion.identity);

        // Initialize carrot
        Carrot carrotScript = carrot.GetComponent<Carrot>();
        if (carrotScript != null)
        {
            carrotScript.Initialize(shootDirection);
        }

        Debug.Log($"Shot carrot! Ammo: {currentAmmo}/{maxAmmo}");
    }

    private void UpdateAmmoUI()
    {
        if (ammoImages == null) return;

        for (int i = 0; i < ammoImages.Length; i++)
        {
            if (ammoImages[i] == null) continue;

            if (i < currentAmmo)
            {
                ammoImages[i].color = ammoFullColor;
            }
            else
            {
                ammoImages[i].color = ammoEmptyColor;
            }
        }
    }

    private IEnumerator RegenAmmo()
    {
        while (currentAmmo < maxAmmo)
        {
            yield return new WaitForSeconds(ammoRegenTime);
            currentAmmo++;
            UpdateAmmoUI();
            Debug.Log($"Ammo regenerated! {currentAmmo}/{maxAmmo}");
        }
        regenCoroutine = null;
    }

    // Called when picking up carrot power-up
    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
        UpdateAmmoUI();
        Debug.Log($"Picked up ammo! {currentAmmo}/{maxAmmo}");
    }

    // Called by Body.cs when player takes damage
    public void OnDamaged(int damage)
    {
        // Forward to PlayerHealth component
        PlayerHealth health = GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.OnDamaged(damage);
        }
    }
}
