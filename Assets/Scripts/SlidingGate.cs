using UnityEngine;

public class SlidingGate : MonoBehaviour
{
    [Header("门动画设置")]
    public float closedHeight = 5f;     // 默认关闭时的高度
    public float openHeight = 0.5f;     // 开启时的目标高度
    public float animationSpeed = 5f;   // 移动速度

    private float targetHeight;
    private Vector3 basePosition;       // 固定底部位置

    void Start()
    {
        targetHeight = closedHeight;

        Vector3 scale = transform.localScale;
        basePosition = transform.position - new Vector3(0, scale.y / 2f, 0);
    }

    void Update()
    {
        Vector3 scale = transform.localScale;
        float currentHeight = scale.y;
        float newHeight = Mathf.MoveTowards(currentHeight, targetHeight, Time.deltaTime * animationSpeed);

        // 更新缩放
        scale.y = newHeight;
        transform.localScale = scale;

        // 更新位置以保持底部不变
        transform.position = basePosition + new Vector3(0, newHeight / 2f, 0);
    }

    public void OpenGate()
    {
        targetHeight = openHeight;
    }

    public void CloseGate()
    {
        targetHeight = closedHeight;
    }
}
