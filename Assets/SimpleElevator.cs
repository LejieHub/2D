using UnityEngine;

public class SimpleElevator : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveHeight = 5f; // 电梯移动的高度（一层的高度）
    [SerializeField] private float moveSpeed = 2f; // 电梯移动速度

    private Vector2 startPosition; // 起始位置
    private Vector2 targetPosition; // 目标位置
    private bool playerOnElevator = false; // 玩家是否在电梯上
    private bool isMoving = false; // 是否正在移动
    private bool atTargetPosition = false; // 是否已在目标位置

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = rb.position;
        targetPosition = startPosition + Vector2.up * moveHeight;
    }

    // 检测玩家进入电梯
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnElevator = true;

            // 如果玩家在电梯上且电梯不在移动状态，开始移动
            if (!isMoving && !atTargetPosition)
            {
                isMoving = true;
            }
        }
    }

    // 检测玩家离开电梯
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnElevator = false;
        }
    }

    void FixedUpdate()
    {
        // 如果电梯正在移动
        if (isMoving)
        {
            // 计算当前位置到目标位置的方向
            Vector2 newPosition = Vector2.MoveTowards(
                rb.position,
                targetPosition,
                moveSpeed * Time.fixedDeltaTime);

            // 移动电梯
            rb.MovePosition(newPosition);

            // 检查是否到达目标位置
            if (Vector2.Distance(rb.position, targetPosition) < 0.01f)
            {
                rb.MovePosition(targetPosition); // 确保精确定位
                isMoving = false;
                atTargetPosition = true;
            }
        }
    }

    // 编辑器辅助：显示移动路径
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;

        Gizmos.color = Color.green;
        Vector3 currentPos = transform.position;
        Vector3 targetPos = currentPos + Vector3.up * moveHeight;

        // 绘制起点和终点
        Gizmos.DrawWireCube(currentPos, GetComponent<BoxCollider2D>().size);
        Gizmos.DrawWireCube(targetPos, GetComponent<BoxCollider2D>().size);

        // 绘制移动路径
        Gizmos.DrawLine(currentPos, targetPos);
    }
}