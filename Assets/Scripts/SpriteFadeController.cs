using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpriteFadeController : MonoBehaviour
{
    [Header("Fade Target Settings")]
    [Tooltip("目标精灵渲染器（在不同游戏对象上）")]
    public SpriteRenderer targetSpriteRenderer;

    [Header("Fade Parameters")]
    [Tooltip("淡入淡出持续时间（秒）")]
    public float fadeDuration = 0.5f;

    [Tooltip("开始淡出前的延迟时间（秒）")]
    public float fadeOutDelay = 0.2f;

    [Header("Debug Settings")]
    [Tooltip("在编辑器中显示调试信息")]
    public bool showDebug = true;

    private float fadeTimer;
    private float fadeTargetAlpha;
    private float currentAlpha;
    private bool isFading;
    private bool playerInside;

    // 初始颜色缓存
    private Color originalColor;

    void Start()
    {
        // 验证设置
        if (targetSpriteRenderer == null)
        {
            Debug.LogError("SpriteFadeController: 未分配目标 SpriteRenderer！", this);
            enabled = false;
            return;
        }

        // 确保碰撞体设为触发器
        Collider2D collider = GetComponent<Collider2D>();
        if (!collider.isTrigger && showDebug)
        {
            Debug.LogWarning("SpriteFadeController: 碰撞体未设为触发器，自动启用'Is Trigger'", this);
            collider.isTrigger = true;
        }

        // 保存原始颜色并设置初始透明度
        originalColor = targetSpriteRenderer.color;
        currentAlpha = 0f;
        UpdateSpriteAlpha(0f);
    }

    void Update()
    {
        // 玩家离开且未触发延迟
        if (!playerInside && !isFading && currentAlpha > 0)
        {
            // 启动延迟淡出
            isFading = true;
            fadeTimer = fadeOutDelay;
            fadeTargetAlpha = 0f;

            if (showDebug) Debug.Log("启动延迟淡出", this);
        }

        if (isFading)
        {
            // 更新计时器
            fadeTimer -= Time.deltaTime;

            if (fadeTimer <= 0f)
            {
                // 计算目标透明度方向
                float direction = (fadeTargetAlpha > currentAlpha) ? 1f : -1f;

                // 计算新的透明度值
                float fadeSpeed = Time.deltaTime / fadeDuration;
                currentAlpha = Mathf.Clamp01(currentAlpha + direction * fadeSpeed);

                // 更新精灵透明度
                UpdateSpriteAlpha(currentAlpha);

                // 检查是否完成过渡
                if ((direction > 0 && currentAlpha >= fadeTargetAlpha) ||
                    (direction < 0 && currentAlpha <= fadeTargetAlpha))
                {
                    currentAlpha = fadeTargetAlpha;
                    isFading = false;

                    if (fadeTargetAlpha <= 0.01f)
                    {
                        // 完全透明时禁用渲染器（可选）
                        // targetSpriteRenderer.enabled = false;
                    }

                    if (showDebug) Debug.Log($"淡入淡出完成: Alpha = {currentAlpha}", this);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            // 准备淡入效果
            isFading = true;
            fadeTimer = 0f;
            fadeTargetAlpha = 1f;

            // 确保渲染器启用
            targetSpriteRenderer.enabled = true;

            if (showDebug) Debug.Log("玩家进入碰撞体 - 启动淡入", this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            if (showDebug) Debug.Log("玩家离开碰撞体 - 准备淡出", this);
        }
    }

    private void UpdateSpriteAlpha(float alpha)
    {
        Color newColor = originalColor;
        newColor.a = alpha;
        targetSpriteRenderer.color = newColor;
    }

    // 在编辑器中可视化触发区域
    void OnDrawGizmos()
    {
        if (showDebug)
        {
            Collider2D collider = GetComponent<Collider2D>();
            if (collider == null) return;

            Gizmos.color = playerInside ? new Color(0, 1, 0, 0.3f) : new Color(1, 0.5f, 0, 0.3f);

            // 绘制碰撞体形状
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
                // 多边形碰撞体需要特殊处理
                Gizmos.DrawWireCube(transform.position, collider.bounds.size);
            }

            // 显示连接线
            if (targetSpriteRenderer != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, targetSpriteRenderer.transform.position);
            }
        }
    }
}