using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class DoorController : MonoBehaviour
{
    public enum MovementType { Vertical, Horizontal }
    public enum Direction { Up = 1, Down = -1, Left = -1, Right = 1 }

    [Header("������")]
    public string requiredKeyID;
    public float openDuration = 1f;

    [Header("UI ��ʾ����Կ�ף�")]
    public CanvasGroup dialogueGroup;
    public float fadeDuration = 0.5f;
    public float displayDuration = 3f;

    private Coroutine fadeRoutine;
    private Coroutine displayTimer;


    [Header("�ƶ�����")]
    [Tooltip("ѡ��ֱ��ˮƽ�ƶ�")]
    public MovementType movementType = MovementType.Vertical;
    [Tooltip("��ֱ����ʱѡ�����£�ˮƽ����ʱѡ������")]
    public Direction movementDirection = Direction.Down;
    [Tooltip("�ƶ��ٶȣ���λ/�룩")]
    public float moveSpeed = 2f;

    [Header("�������")]
    [SerializeField] private Collider2D triggerCollider;
    [SerializeField] private Collider2D blockingCollider;
    [SerializeField] private SpriteRenderer doorRenderer;

    private bool _isOpen;
    private Vector3 _originalPosition;
    private float _movementDistance;

    void Start()
    {
        InitializeComponents();
        StoreOriginalPosition();
        InitializeColliders();
        CalculateMovementDistance();
        if (dialogueGroup != null)
        {
            dialogueGroup.alpha = 0;
            dialogueGroup.interactable = false;
            dialogueGroup.blocksRaycasts = false;
        }

    }

    void InitializeComponents()
    {
        if (!doorRenderer) doorRenderer = GetComponent<SpriteRenderer>();
        if (!blockingCollider) blockingCollider = GetComponent<Collider2D>();
    }

    void StoreOriginalPosition()
    {
        _originalPosition = transform.position;
    }

    void CalculateMovementDistance()
    {
        Bounds bounds = doorRenderer.bounds;
        _movementDistance = movementType == MovementType.Vertical
            ? bounds.size.y
            : bounds.size.x;
    }

    void InitializeColliders()
    {
        if (triggerCollider)
        {
            triggerCollider.isTrigger = true;
        }
        else
        {
            Debug.LogError("ȱ�ٴ�������ײ�壡");
        }

        if (blockingCollider)
        {
            blockingCollider.isTrigger = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isOpen && other.CompareTag("Player"))
        {
            TryOpenDoor();
        }
    }

    void TryOpenDoor()
    {
        if (KeyManager.Instance.CheckKey(requiredKeyID))
        {
            StartCoroutine(OpenDoorAnimation());
        }
        else
        {
            StartCoroutine(ShakeDoor());
            StartDialogueFeedback();  // ��ʾ��ʾUI
        }
    }

    void StartDialogueFeedback()
    {
        if (dialogueGroup == null) return;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeUI(1, fadeDuration));

        if (displayTimer != null) StopCoroutine(displayTimer);
        displayTimer = StartCoroutine(DisplayTimer());
    }

    IEnumerator DisplayTimer()
    {
        yield return new WaitForSeconds(displayDuration);
        fadeRoutine = StartCoroutine(FadeUI(0, fadeDuration));
    }

    IEnumerator FadeUI(float targetAlpha, float duration)
    {
        float startAlpha = dialogueGroup.alpha;
        float time = 0;

        dialogueGroup.interactable = targetAlpha > 0.1f;
        dialogueGroup.blocksRaycasts = targetAlpha > 0.1f;

        while (time < duration)
        {
            time += Time.deltaTime;
            dialogueGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        dialogueGroup.alpha = targetAlpha;
    }


    IEnumerator OpenDoorAnimation()
    {
        _isOpen = true;
        if (triggerCollider) triggerCollider.enabled = false;

        Vector3 direction = GetMovementDirection();
        Vector3 targetPosition = _originalPosition + direction * _movementDistance;

        float elapsed = 0;
        while (elapsed < openDuration)
        {
            // ����ʵ���ٶȼ����ֵ����
            float t = elapsed / openDuration;
            transform.position = Vector3.Lerp(
                _originalPosition,
                targetPosition,
                t
            );

            // �����ٶȿ��ƶ�������
            elapsed += Time.deltaTime * moveSpeed;
            yield return null;
        }

        Destroy(gameObject);
    }

    Vector3 GetMovementDirection()
    {
        switch (movementType)
        {
            case MovementType.Vertical:
                return Vector3.up * (int)movementDirection;
            case MovementType.Horizontal:
                return Vector3.right * (int)movementDirection;
            default:
                return Vector3.down;
        }
    }

    IEnumerator ShakeDoor()
    {
        Vector3 originalPos = transform.position;
        float shakeDuration = 0.5f;
        float shakePower = 0.1f;

        for (float t = 0; t < shakeDuration; t += Time.deltaTime)
        {
            transform.position = originalPos + (Vector3)Random.insideUnitCircle * shakePower;
            yield return null;
        }
        transform.position = originalPos;
    }

    void OnValidate()
    {
        if (triggerCollider == null)
        {
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (var col in colliders)
            {
                if (col.isTrigger)
                {
                    triggerCollider = col;
                }
                else
                {
                    blockingCollider = col;
                }
            }
        }
    }
}