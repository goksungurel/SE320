using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float forwardSpeed = 5f;
    public float jumpForce = 18f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.25f;
    public LayerMask groundLayer;

    [Header("VFX & Damage")]
    public SpriteRenderer characterSprite;
    public float flickerDuration = 0.1f;
    public int flickerAmount = 6;
    private bool isInvincible = false;

    [Header("Sound Settings")]
    private AudioSource audioSource;
    public AudioClip jumpSound; 

    private Rigidbody2D rb;
    public bool isGrounded;
    private bool wantJump;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        if (characterSprite == null)
            characterSprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            wantJump = true;
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        rb.linearVelocity = new Vector2(GameManager3.Instance.currentForwardSpeed, rb.linearVelocity.y);

        if (wantJump)
        {
            if (isGrounded)
            {
                GameManager3.Instance.PlayJumpSound();
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

                if (jumpSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(jumpSound);
                }
            }
            wantJump = false;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // 1. NORMAL ENGEL ÇARPIÞMASI
        if (col.CompareTag("Obstacle") && !isInvincible)
        {
            if (GameManager3.Instance != null)
            {
                GameManager3.Instance.TakeDamage(1); // Bu fonksiyon zaten normal damage sesini çalar

                SpriteRenderer obstacleSprite = col.GetComponent<SpriteRenderer>();
                if (obstacleSprite != null) StartCoroutine(ObstacleFlicker(obstacleSprite));
                StartCoroutine(InvincibilityCooldown());
            }
        }

        // 2. MERMÝ ÇARPIÞMASI (BURASI YENÝ)
        if (col.CompareTag("Bullet"))
        {
            if (GameManager3.Instance != null)
            {
                GameManager3.Instance.TakeDamage(1); // Caný azalt
                GameManager3.Instance.PlayBulletHitSound(); // ÖZEL MERMÝ SESÝNÝ ÇAL
                Destroy(col.gameObject); // Mermiyi yok et
            }
        }

        if (col.CompareTag("Coin"))
        {
            if (GameManager3.Instance != null)
            {
                GameManager3.Instance.AddCoin(1);
                Destroy(col.gameObject);
            }
        }
    }

    System.Collections.IEnumerator ObstacleFlicker(SpriteRenderer targetSprite)
    {
        for (int i = 0; i < flickerAmount; i++)
        {
            targetSprite.color = new Color(1, 1, 1, 0.1f);
            yield return new WaitForSeconds(flickerDuration);
            targetSprite.color = Color.white;
            yield return new WaitForSeconds(flickerDuration);
        }
    }

    System.Collections.IEnumerator InvincibilityCooldown()
    {
        isInvincible = true;
        yield return new WaitForSeconds(1f);
        isInvincible = false;
    }
}