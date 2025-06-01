using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Animation Controller")]
    [SerializeField] PlayerAnimationController animationController;

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

    [Header("Ground Check - Point 1")]
    [SerializeField] Transform groundCheck1;
    [SerializeField] float groundCheckRadius1 = 0.2f;
    [SerializeField] bool useGroundCheck1 = true;

    [Header("Ground Check - Point 2")]
    [SerializeField] Transform groundCheck2;
    [SerializeField] float groundCheckRadius2 = 0.2f;
    [SerializeField] bool useGroundCheck2 = true;

    [Header("Ground Layer")]
    [SerializeField] LayerMask groundLayer;

    [Header("Jump Sound")]
    [SerializeField] AudioClip jumpSound;
    [Range(0, 1)]
    [SerializeField] float jumpSoundVolume = 0.7f;
    [SerializeField] bool playOnLanding = false;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private float horizontalInput;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool _wasGrounded;
    private bool _isJumping;

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
            Vector2 jumpDirection = rb.gravityScale > 0 ? Vector2.up : Vector2.down;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(jumpDirection * Mathf.Abs(jumpForce), ForceMode2D.Impulse);
            coyoteTimeCounter = 0;
            PlayJumpSound();

            // 触发跳跃动画
            if (animationController != null)
            {
                animationController.TriggerJump();
            }
            else
            {
                TryFindAnimationController();
            }

            _isJumping = true;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        TryFindAnimationController();
        rb.freezeRotation = true;
    }

    void TryFindAnimationController()
    {
        if (animationController == null)
        {
            animationController = GetComponent<PlayerAnimationController>();
        }
    }

    void Update()
    {
        // 输入处理
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

        // 更新移动速度动画参数
        if (animationController != null)
        {
            animationController.SetMoveSpeed(horizontalInput);
        }

        bool isGrounded = IsGrounded();
        coyoteTimeCounter = isGrounded ? coyoteTime : Mathf.Max(coyoteTimeCounter - Time.deltaTime, 0);

        // 落地处理
        if (!_wasGrounded && isGrounded)
        {
            if (playOnLanding && _isJumping)
            {
                PlayJumpSound();
            }

            // 重置跳跃状态
            if (animationController != null && _isJumping)
            {
                animationController.ForceLandingCheck();
            }

            // 重置跳跃标记
            _isJumping = false;
        }
        _wasGrounded = isGrounded;

        // 跳跃触发
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            Jump();
            jumpBufferCounter = 0;
        }

        // 处理跳跃物理
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

        // 触发跳跃动画
        if (animationController != null)
        {
            animationController.TriggerJump();
        }
        else
        {
            TryFindAnimationController();
        }

        PlayJumpSound();
        _isJumping = true;
    }

    void HandleJumpPhysics()
    {
        // 下落速度调整
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        // 低跳跃速度调整
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void PlayJumpSound()
    {
        if (jumpSound != null && audioSource != null)
        {
            audioSource.volume = jumpSoundVolume;
            audioSource.PlayOneShot(jumpSound);
        }
        else if (jumpSound != null)
        {
            Debug.LogWarning("Jump sound is set but AudioSource component is missing on " + gameObject.name, gameObject);
        }
    }

    // 改进的接地检测方法
    bool IsGrounded()
    {
        // 第一个检测点
        if (useGroundCheck1 && groundCheck1 != null &&
            Physics2D.OverlapCircle(groundCheck1.position, groundCheckRadius1, groundLayer))
        {
            return true;
        }

        // 第二个检测点
        if (useGroundCheck2 && groundCheck2 != null &&
            Physics2D.OverlapCircle(groundCheck2.position, groundCheckRadius2, groundLayer))
        {
            return true;
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        // 绘制第一个检测点
        if (useGroundCheck1 && groundCheck1 != null)
        {
            Gizmos.DrawWireSphere(groundCheck1.position, groundCheckRadius1);
        }

        // 绘制第二个检测点
        if (useGroundCheck2 && groundCheck2 != null)
        {
            Gizmos.DrawWireSphere(groundCheck2.position, groundCheckRadius2);
        }
    }
}