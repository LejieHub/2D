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

        // ��ȡ�������벢����
        float playerInput = Input.GetAxisRaw("Horizontal");
        float mirroredInput = -playerInput;
        bool jumpPressed = Input.GetButtonDown("Jump");

        // ��α���봫�ݸ���������ƶ�������
        mirrorMovement.SetMirrorInput(mirroredInput, jumpPressed);

        // ��ѡ��λ��΢�������ֶԳ�
        float offset = playerTarget.position.x - mirrorAxisX;
        float mirroredX = mirrorAxisX - offset;
        float error = mirroredX - transform.position.x;
        rb.velocity += new Vector2(error * positionCorrectionStrength, 0f) * Time.deltaTime;

        // ��ɫ��ת����ѡ��
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = mirroredInput < 0;
        }
    }
}
