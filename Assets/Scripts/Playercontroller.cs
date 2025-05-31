using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))] // ���AudioSource���Ҫ��
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

    [Header("Jump Sound")] // ������Ч����
    [SerializeField] AudioClip jumpSound;  // ��Ծ��Ч
    [Range(0, 1)]
    [SerializeField] float jumpSoundVolume = 0.7f; // ��������
    [SerializeField] bool playOnLanding = false; // �Ƿ������ʱ������Ч

    private Rigidbody2D rb;
    private AudioSource audioSource; // ��ƵԴ���
    private float horizontalInput;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool _wasGrounded;
    private bool _isJumping; // ������Ծ״̬

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
            PlayJumpSound(); // ������Ծ��Ч
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>(); // ��ȡ��ƵԴ���
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

        // �������֮ǰ������Ծ״̬ʱ
        if (!_wasGrounded && isGrounded && playOnLanding && _isJumping)
        {
            PlayJumpSound(); // �����ʱ������Ч
            _isJumping = false; // ������Ծ״̬
        }

        // ���浱ǰ֡�ĵ���״̬������һ֡�Ƚ�
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
        PlayJumpSound(); // ������Ծ��Ч
        _isJumping = true; // ������Ծ״̬
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

    // ������Ծ��Ч�ķ���
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

    // �Ľ��Ľӵؼ�ⷽ����֧�ֶ������
    bool IsGrounded()
    {
        // �����һ�����������Ҽ�⵽���棬����true
        if (useGroundCheck1 && groundCheck1 != null &&
            Physics2D.OverlapCircle(groundCheck1.position, groundCheckRadius1, groundLayer))
        {
            return true;
        }

        // ����ڶ������������Ҽ�⵽���棬����true
        if (useGroundCheck2 && groundCheck2 != null &&
            Physics2D.OverlapCircle(groundCheck2.position, groundCheckRadius2, groundLayer))
        {
            return true;
        }

        // �������㶼û�м�⵽����
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        // ���Ƶ�һ������
        if (useGroundCheck1 && groundCheck1 != null)
        {
            Gizmos.DrawWireSphere(groundCheck1.position, groundCheckRadius1);
        }

        // ���Ƶڶ�������
        if (useGroundCheck2 && groundCheck2 != null)
        {
            Gizmos.DrawWireSphere(groundCheck2.position, groundCheckRadius2);
        }
    }
}