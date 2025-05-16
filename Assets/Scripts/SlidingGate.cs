using UnityEngine;

public class SlidingGate : MonoBehaviour
{
    public enum SlideDirection
    {
        Vertical,
        Horizontal
    }

    [Header("门动画设置")]
    public SlideDirection direction = SlideDirection.Vertical;
    public float closedSize = 5f;      // 关闭时的尺寸（高度或宽度）
    public float openSize = 0.5f;      // 打开时的目标尺寸
    public float animationSpeed = 5f;  // 动画速度

    private float targetSize;
    private Vector3 fixedPosition;     // 保持固定端的位置

    void Start()
    {
        targetSize = closedSize;

        Vector3 scale = transform.localScale;

        // 记录固定边位置（顶部或左侧）
        if (direction == SlideDirection.Vertical)
        {
            fixedPosition = transform.position + new Vector3(0, scale.y / 2f, 0);
        }
        else
        {
            fixedPosition = transform.position - new Vector3(scale.x / 2f, 0, 0); // 固定左侧
        }
    }

    void Update()
    {
        Vector3 scale = transform.localScale;
        Vector3 position = transform.position;

        if (direction == SlideDirection.Vertical)
        {
            float current = scale.y;
            float newValue = Mathf.MoveTowards(current, targetSize, Time.deltaTime * animationSpeed);
            scale.y = newValue;
            position = fixedPosition - new Vector3(0, newValue / 2f, 0);
        }
        else // Horizontal
        {
            float current = scale.x;
            float newValue = Mathf.MoveTowards(current, targetSize, Time.deltaTime * animationSpeed);
            scale.x = newValue;
            position = fixedPosition + new Vector3(newValue / 2f, 0, 0);
        }

        transform.localScale = scale;
        transform.position = position;
    }

    public void OpenGate()
    {
        targetSize = openSize;
    }

    public void CloseGate()
    {
        targetSize = closedSize;
    }
}
