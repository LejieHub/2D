using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
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

    private Rigidbody2D rb;
    private float horizontalInput;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool _wasGrounded;

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
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        coyoteTimeCounter = isGrounded ? coyoteTime : Mathf.Max(coyoteTimeCounter - Time.deltaTime, 0);

        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            Jump();
            jumpBufferCounter = 0;
        }

        if (!_wasGrounded && isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        _wasGrounded = isGrounded;

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
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        coyoteTimeCounter = 0;
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