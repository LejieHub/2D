using UnityEngine;

public class MirrorController : MonoBehaviour
{
    private Transform playerTarget;
    private float mirrorAxisX;
    private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private PlayerMovementController mirrorMovement;

    [SerializeField] private float positionCorrectionStrength = 5f;
    [SerializeField] private Transform visualTransform;


    private bool wasOnLeftSide;
    private bool hasCrossed = false;

    private Vector3 originalScale; // ��¼��ʼ���ţ���ֹ�α�

    public void Init(Transform player, float axisX)
    {
        playerTarget = player;
        mirrorAxisX = axisX;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        mirrorMovement = GetComponent<PlayerMovementController>();

        wasOnLeftSide = transform.position.x < mirrorAxisX;

        originalScale = transform.localScale; // ��¼��ʼ����ֵ���� 2,2,2��
    }

    void Update()
    {
        if (playerTarget == null || mirrorMovement == null) return;

        // ��ȡ��������
        float playerInput = Input.GetAxisRaw("Horizontal");
        float mirroredInput = -playerInput;
        bool jumpPressed = Input.GetButtonDown("Jump");

        // ������Ӧ�þ�������
        mirrorMovement.SetMirrorInput(mirroredInput, jumpPressed);

        // λ���������������ھ���λ��
        float offset = playerTarget.position.x - mirrorAxisX;
        float mirroredX = mirrorAxisX - offset;
        float error = mirroredX - transform.position.x;
        rb.velocity += new Vector2(error * positionCorrectionStrength, 0f) * Time.deltaTime;

        // ��ȷ��ת�Ӿ����򣨲�����Σ�
        if (Mathf.Abs(playerInput) > 0.01f)
        {
            Vector3 newScale = originalScale;
            newScale.x = -Mathf.Sign(playerTarget.localScale.x) * Mathf.Abs(originalScale.x);
            transform.localScale = newScale;
        }



        // ���ż��
        CheckGateCross();
    }

    void CheckGateCross()
    {
        float mirrorX = transform.position.x;
        bool nowOnLeftSide = mirrorX < mirrorAxisX;

        if (nowOnLeftSide != wasOnLeftSide && !hasCrossed)
        {
            hasCrossed = true;
            StartCoroutine(FadeAndDestroy());
            Debug.Log("Mirror crossed gate, fading out.");
        }
    }

    public void StartDestruction()
    {
        if (!hasCrossed) // ��ֹ�ظ�����
        {
            hasCrossed = true;
            StartCoroutine(FadeAndDestroy());
            Debug.Log("Mirror manually destroyed by trigger.");
        }
    }


    System.Collections.IEnumerator FadeAndDestroy()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null)
        {
            Destroy(gameObject);
            yield break;
        }

        Color originalColor = sr.color;
        float timer = 0f;
        float fadeDuration = 0.5f;

        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        Destroy(gameObject);
    }
}
