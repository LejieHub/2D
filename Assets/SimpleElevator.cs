using UnityEngine;

public class SimpleElevator : MonoBehaviour
{
    [Header("�ƶ�����")]
    [SerializeField] private float moveHeight = 5f;
    [SerializeField] private float moveSpeed = 2f;

    private Vector2 startPosition;
    private Vector2 targetPosition;
    private bool playerOnElevator = false;
    private bool isMoving = false;
    private bool atTargetPosition = false;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = rb.position;
        targetPosition = startPosition + Vector2.up * moveHeight;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnElevator = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnElevator = false;
        }
    }

    void Update()
    {
        // ֻ�е�����ڵ����ϡ�û�ڶ���û�����յ�ʱ������ F ���Ŵ����ƶ�
        if (playerOnElevator && !isMoving && !atTargetPosition)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                isMoving = true;
                Debug.Log("���� F ��������");
            }
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            Vector2 newPosition = Vector2.MoveTowards(
                rb.position,
                targetPosition,
                moveSpeed * Time.fixedDeltaTime);

            rb.MovePosition(newPosition);

            if (Vector2.Distance(rb.position, targetPosition) < 0.01f)
            {
                rb.MovePosition(targetPosition);
                isMoving = false;
                atTargetPosition = true;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;

        Gizmos.color = Color.green;
        Vector3 currentPos = transform.position;
        Vector3 targetPos = currentPos + Vector3.up * moveHeight;

        Gizmos.DrawWireCube(currentPos, GetComponent<BoxCollider2D>().size);
        Gizmos.DrawWireCube(targetPos, GetComponent<BoxCollider2D>().size);
        Gizmos.DrawLine(currentPos, targetPos);
    }
}
