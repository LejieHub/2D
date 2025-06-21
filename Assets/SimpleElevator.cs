using UnityEngine;

public class SimpleElevator : MonoBehaviour
{
    [Header("�ƶ�����")]
    [SerializeField] private float moveHeight = 5f; // �����ƶ��ĸ߶ȣ�һ��ĸ߶ȣ�
    [SerializeField] private float moveSpeed = 2f; // �����ƶ��ٶ�

    private Vector2 startPosition; // ��ʼλ��
    private Vector2 targetPosition; // Ŀ��λ��
    private bool playerOnElevator = false; // ����Ƿ��ڵ�����
    private bool isMoving = false; // �Ƿ������ƶ�
    private bool atTargetPosition = false; // �Ƿ�����Ŀ��λ��

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = rb.position;
        targetPosition = startPosition + Vector2.up * moveHeight;
    }

    // �����ҽ������
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnElevator = true;

            // �������ڵ������ҵ��ݲ����ƶ�״̬����ʼ�ƶ�
            if (!isMoving && !atTargetPosition)
            {
                isMoving = true;
            }
        }
    }

    // �������뿪����
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnElevator = false;
        }
    }

    void FixedUpdate()
    {
        // ������������ƶ�
        if (isMoving)
        {
            // ���㵱ǰλ�õ�Ŀ��λ�õķ���
            Vector2 newPosition = Vector2.MoveTowards(
                rb.position,
                targetPosition,
                moveSpeed * Time.fixedDeltaTime);

            // �ƶ�����
            rb.MovePosition(newPosition);

            // ����Ƿ񵽴�Ŀ��λ��
            if (Vector2.Distance(rb.position, targetPosition) < 0.01f)
            {
                rb.MovePosition(targetPosition); // ȷ����ȷ��λ
                isMoving = false;
                atTargetPosition = true;
            }
        }
    }

    // �༭����������ʾ�ƶ�·��
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;

        Gizmos.color = Color.green;
        Vector3 currentPos = transform.position;
        Vector3 targetPos = currentPos + Vector3.up * moveHeight;

        // ���������յ�
        Gizmos.DrawWireCube(currentPos, GetComponent<BoxCollider2D>().size);
        Gizmos.DrawWireCube(targetPos, GetComponent<BoxCollider2D>().size);

        // �����ƶ�·��
        Gizmos.DrawLine(currentPos, targetPos);
    }
}