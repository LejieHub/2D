using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))] // 添加AudioSource组件要求
public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float airMoveSpeed = 7f;
    [SerializeField] float groundAcceleration = 20f;
    [SerializeField] float airAcceleration = 15f;

    [Header("Jump Parameters")]
    [SerializeField] float jumpForce = 13f;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2f;
    [SerializeField] float coyoteTime = 0.15f;
    [SerializeField] float jumpBufferTime = 0.2f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckRadius = 0.2f;

    [Header("Jump Sound")] // 新增音效部分
    [SerializeField] AudioClip jumpSound;  // 跳跃音效
    [Range(0, 1)]
    [SerializeField] float jumpSoundVolume = 0.7f; // 音量控制
    [SerializeField] bool playOnLanding = false; // 是否在落地时播放音效

    private Rigidbody2D rb;
    private AudioSource audioSource; // 音频源组件
    private float horizontalInput;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool _wasGrounded;
    private bool _isJumping; // 跟踪跳跃状态

    private bool useFakeInput = false;
    private float fakeHorizontal = 0f;
    private bool fakeJump = false;

    public void SetMirrorInput(float hInput, bool jumpPressed)
    {
        useFakeInput = true;
        fakeHorizontal = hInput;
        fakeJump = jumpPressed;
    }

    public void ForceJump()
    {
        if (IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            coyoteTimeCounter = 0;
            PlayJumpSound(); // 播放跳跃音效
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>(); // 获取音频源组件
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (useFakeInput)
        {
            horizontalInput = fakeHorizontal;
            if (fakeJump)
            {
                jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                jumpBufferCounter = Mathf.Max(jumpBufferCounter - Time.deltaTime, 0);
            }
            useFakeInput = false;
        }
        else
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            if (Input.GetButtonDown("Jump"))
            {
                jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                jumpBufferCounter = Mathf.Max(jumpBufferCounter - Time.deltaTime, 0);
            }
        }

        bool isGrounded = IsGrounded();
        Debug.Log($"{gameObject.name} grounded: {isGrounded}");
        coyoteTimeCounter = isGrounded ? coyoteTime : Mathf.Max(coyoteTimeCounter - Time.deltaTime, 0);

        // 当落地且之前处于跳跃状态时
        if (!_wasGrounded && isGrounded && playOnLanding && _isJumping)
        {
            PlayJumpSound(); // 在落地时播放音效
            _isJumping = false; // 重置跳跃状态
        }

        // 保存当前帧的地面状态用于下一帧比较
        _wasGrounded = isGrounded;

        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            Debug.Log($"{gameObject.name} JUMP triggered!");
            Jump();
            jumpBufferCounter = 0;
        }

        HandleJumpPhysics();
    }

    void FixedUpdate()
    {
        HandleMovement();
        rb.angularVelocity = 0;
    }

    void HandleMovement()
    {
        float targetSpeed = horizontalInput * (IsGrounded() ? moveSpeed : airMoveSpeed);
        float acceleration = IsGrounded() ? groundAcceleration : airAcceleration;

        float speedDifference = targetSpeed - rb.velocity.x;
        rb.AddForce(Vector2.right * speedDifference * acceleration);

        rb.velocity = new Vector2(
            Mathf.Clamp(rb.velocity.x, -moveSpeed, moveSpeed),
            rb.velocity.y
        );
    }

    void Jump()
    {
        Vector2 jumpDirection = rb.gravityScale > 0 ? Vector2.up : Vector2.down;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(jumpDirection * Mathf.Abs(jumpForce), ForceMode2D.Impulse);
        coyoteTimeCounter = 0;
        PlayJumpSound(); // 播放跳跃音效
        _isJumping = true; // 设置跳跃状态
    }

    void HandleJumpPhysics()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    // 播放跳跃音效的方法
    void PlayJumpSound()
    {
        if (jumpSound != null && audioSource != null)
        {
            audioSource.volume = jumpSoundVolume;
            audioSource.PlayOneShot(jumpSound);
        }
        else if (jumpSound != null)
        {
            Debug.LogWarning("Jump sound is set but AudioSource component is missing");
        }
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}