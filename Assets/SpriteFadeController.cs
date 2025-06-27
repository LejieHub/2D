using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpriteFadeController : MonoBehaviour
{
    [Header("Fade Target Settings")]
    [Tooltip("Ŀ�꾫����Ⱦ�����ڲ�ͬ��Ϸ�����ϣ�")]
    public SpriteRenderer targetSpriteRenderer;

    [Header("Fade Parameters")]
    [Tooltip("���뵭������ʱ�䣨�룩")]
    public float fadeDuration = 0.5f;

    [Tooltip("��ʼ����ǰ���ӳ�ʱ�䣨�룩")]
    public float fadeOutDelay = 0.2f;

    [Header("Debug Settings")]
    [Tooltip("�ڱ༭������ʾ������Ϣ")]
    public bool showDebug = true;

    private float fadeTimer;
    private float fadeTargetAlpha;
    private float currentAlpha;
    private bool isFading;
    private bool playerInside;

    // ��ʼ��ɫ����
    private Color originalColor;

    void Start()
    {
        // ��֤����
        if (targetSpriteRenderer == null)
        {
            Debug.LogError("SpriteFadeController: δ����Ŀ�� SpriteRenderer��", this);
            enabled = false;
            return;
        }

        // ȷ����ײ����Ϊ������
        Collider2D collider = GetComponent<Collider2D>();
        if (!collider.isTrigger && showDebug)
        {
            Debug.LogWarning("SpriteFadeController: ��ײ��δ��Ϊ���������Զ�����'Is Trigger'", this);
            collider.isTrigger = true;
        }

        // ����ԭʼ��ɫ�����ó�ʼ͸����
        originalColor = targetSpriteRenderer.color;
        currentAlpha = 0f;
        UpdateSpriteAlpha(0f);
    }

    void Update()
    {
        // ����뿪��δ�����ӳ�
        if (!playerInside && !isFading && currentAlpha > 0)
        {
            // �����ӳٵ���
            isFading = true;
            fadeTimer = fadeOutDelay;
            fadeTargetAlpha = 0f;

            if (showDebug) Debug.Log("�����ӳٵ���", this);
        }

        if (isFading)
        {
            // ���¼�ʱ��
            fadeTimer -= Time.deltaTime;

            if (fadeTimer <= 0f)
            {
                // ����Ŀ��͸���ȷ���
                float direction = (fadeTargetAlpha > currentAlpha) ? 1f : -1f;

                // �����µ�͸����ֵ
                float fadeSpeed = Time.deltaTime / fadeDuration;
                currentAlpha = Mathf.Clamp01(currentAlpha + direction * fadeSpeed);

                // ���¾���͸����
                UpdateSpriteAlpha(currentAlpha);

                // ����Ƿ���ɹ���
                if ((direction > 0 && currentAlpha >= fadeTargetAlpha) ||
                    (direction < 0 && currentAlpha <= fadeTargetAlpha))
                {
                    currentAlpha = fadeTargetAlpha;
                    isFading = false;

                    if (fadeTargetAlpha <= 0.01f)
                    {
                        // ��ȫ͸��ʱ������Ⱦ������ѡ��
                        // targetSpriteRenderer.enabled = false;
                    }

                    if (showDebug) Debug.Log($"���뵭�����: Alpha = {currentAlpha}", this);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            // ׼������Ч��
            isFading = true;
            fadeTimer = 0f;
            fadeTargetAlpha = 1f;

            // ȷ����Ⱦ������
            targetSpriteRenderer.enabled = true;

            if (showDebug) Debug.Log("��ҽ�����ײ�� - ��������", this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            if (showDebug) Debug.Log("����뿪��ײ�� - ׼������", this);
        }
    }

    private void UpdateSpriteAlpha(float alpha)
    {
        Color newColor = originalColor;
        newColor.a = alpha;
        targetSpriteRenderer.color = newColor;
    }

    // �ڱ༭���п��ӻ���������
    void OnDrawGizmos()
    {
        if (showDebug)
        {
            Collider2D collider = GetComponent<Collider2D>();
            if (collider == null) return;

            Gizmos.color = playerInside ? new Color(0, 1, 0, 0.3f) : new Color(1, 0.5f, 0, 0.3f);

            // ������ײ����״
            if (collider is BoxCollider2D box)
            {
                Gizmos.DrawCube(transform.position + new Vector3(box.offset.x, box.offset.y, 0),
                               box.size);
            }
            else if (collider is CircleCollider2D circle)
            {
                Gizmos.DrawSphere(transform.position + new Vector3(circle.offset.x, circle.offset.y, 0),
                                 circle.radius);
            }
            else if (collider is PolygonCollider2D)
            {
                // �������ײ����Ҫ���⴦��
                Gizmos.DrawWireCube(transform.position, collider.bounds.size);
            }

            // ��ʾ������
            if (targetSpriteRenderer != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, targetSpriteRenderer.transform.position);
            }
        }
    }
}