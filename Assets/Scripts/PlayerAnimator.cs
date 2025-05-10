using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isOnWall;

    [Header("������")]
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
        // ����Ƿ��ڵ����ǽ��
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        isOnWall = Physics2D.OverlapCircle(wallCheck.position, 0.1f, wallLayer);

        // ����ˮƽ�ƶ��Ͷ���
        float moveInput = Input.GetAxis("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        // ������Ծ
        if (Input.GetButtonDown("Jump") && (isGrounded || isOnWall))
        {
            rb.velocity = new Vector2(rb.velocity.x, 10f); // ��Ծ��
            animator.SetBool("IsJumping", true);
        }

        // ��غ�������Ծ״̬
        if (isGrounded || isOnWall)
        {
            animator.SetBool("IsJumping", false);
        }

        // ������ǽ
        if (isOnWall && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            animator.SetBool("IsClimbing", true);
            //rb.gravityScale = 0; // ��ǽʱȡ������
        }
        else
        {
            animator.SetBool("IsClimbing", false);
            rb.gravityScale = 3; // �ָ�����
        }

        // ��ɫ����ת
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);
        }
    }
}