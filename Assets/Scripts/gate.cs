using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class DoorController : MonoBehaviour
{
    public enum MovementType { Vertical, Horizontal }
    public enum Direction { Up = 1, Down = -1, Left = -1, Right = 1 }

    [Header("门设置")]
    public string requiredKeyID;
    public float openDuration = 1f;

    [Header("移动设置")]
    [Tooltip("选择垂直或水平移动")]
    public MovementType movementType = MovementType.Vertical;
    [Tooltip("垂直方向时选择上下，水平方向时选择左右")]
    public Direction movementDirection = Direction.Down;
    [Tooltip("移动速度（单位/秒）")]
    public float moveSpeed = 2f;

    [Header("组件引用")]
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
            Debug.LogError("缺少触发用碰撞体！");
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
        }
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
            // 根据实际速度计算插值比例
            float t = elapsed / openDuration;
            transform.position = Vector3.Lerp(
                _originalPosition,
                targetPosition,
                t
            );

            // 根据速度控制动画进度
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