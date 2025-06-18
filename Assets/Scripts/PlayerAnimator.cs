using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    public bool isGrounded;
    private bool isOnWall;

    [Header("检测参数")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 检测是否在地面或墙上
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);
        isOnWall = Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);

        // 处理水平移动和动画
        float moveInput = Input.GetAxis("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        // 处理跳跃
        if (Input.GetButtonDown("Jump") && isGrounded) 
        {
            //rb.velocity = new Vector2(rb.velocity.x, 10f); // 跳跃力
            animator.SetBool("IsClimbing", true);
        }

        // 落地后重置跳跃状态
        if (isGrounded)
        {
            animator.SetBool("IsJumping", false);
        }

        // 处理爬墙
        if (isOnWall && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            animator.SetBool("IsClimbing", true);
            //rb.gravityScale = 0; // 爬墙时取消重力
        }
        else
        {
            animator.SetBool("IsClimbing", false);
            rb.gravityScale = 3; // 恢复重力
        }

        // 角色朝向翻转
        if (moveInput != 0)
        {
            float rotationY = (moveInput > 0) ? 0 : 180;
            transform.rotation = Quaternion.Euler(0, rotationY, 0);
        }
    }
}