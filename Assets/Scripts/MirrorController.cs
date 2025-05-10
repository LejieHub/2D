using UnityEngine;

public class MirrorController : MonoBehaviour
{
    private Transform playerTarget;
    private float mirrorAxisX;
    private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private PlayerMovementController mirrorMovement;

    [SerializeField] private float positionCorrectionStrength = 5f;

    public void Init(Transform player, float axisX)
    {
        playerTarget = player;
        mirrorAxisX = axisX;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        mirrorMovement = GetComponent<PlayerMovementController>();
    }

    void Update()
    {
        if (playerTarget == null || mirrorMovement == null) return;

        // 获取主角输入并镜像
        float playerInput = Input.GetAxisRaw("Horizontal");
        float mirroredInput = -playerInput;
        bool jumpPressed = Input.GetButtonDown("Jump");

        // 将伪输入传递给镜像体的移动控制器
        mirrorMovement.SetMirrorInput(mirroredInput, jumpPressed);

        // 可选：位置微调，保持对称
        float offset = playerTarget.position.x - mirrorAxisX;
        float mirroredX = mirrorAxisX - offset;
        float error = mirroredX - transform.position.x;
        rb.velocity += new Vector2(error * positionCorrectionStrength, 0f) * Time.deltaTime;

        // 角色翻转（可选）
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = mirroredInput < 0;
        }
    }
}
