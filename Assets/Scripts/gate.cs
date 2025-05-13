// DoorController.cs
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class DoorController : MonoBehaviour
{
    [Header("������")]
    public string requiredKeyID;    // ��Ҫƥ���Կ��ID
    public float openDuration = 1f; // ���Ŷ���ʱ��

    [Header("�������")]
    [SerializeField] private Collider2D triggerCollider; // �������ײ��
    [SerializeField] private Collider2D blockingCollider; // �赲����ײ��
    [SerializeField] private SpriteRenderer doorRenderer;

    // ״̬����
    private bool _isOpen;
    private Vector3 _originalPosition;
    private float _doorHeight;

    void Start()
    {
        InitializeComponents();
        StoreOriginalPosition();
        InitializeColliders();
    }

    void InitializeComponents()
    {
        if (!doorRenderer) doorRenderer = GetComponent<SpriteRenderer>();
        if (!blockingCollider) blockingCollider = GetComponent<Collider2D>();
    }

    void StoreOriginalPosition()
    {
        _originalPosition = transform.position;
        _doorHeight = doorRenderer.bounds.size.y;
    }

    void InitializeColliders()
    {
        // ȷ��������ײ������Ϊ������
        if (triggerCollider)
        {
            triggerCollider.isTrigger = true;
        }
        else
        {
            Debug.LogError("ȱ�ٴ�������ײ�壡");
        }

        // �赲��ײ�屣�ַǴ�����״̬
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
        DisableColliders();
        Vector3 targetPosition = _originalPosition + Vector3.down * _doorHeight;

        float elapsed = 0;
        while (elapsed < openDuration)
        {
            transform.position = Vector3.Lerp(
                _originalPosition,
                targetPosition,
                elapsed / openDuration
            );
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    void DisableColliders()
    {
        if (triggerCollider) triggerCollider.enabled = false;
        if (blockingCollider) blockingCollider.enabled = false;
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
        // �༭���Զ������������
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