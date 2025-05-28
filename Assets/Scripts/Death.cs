using UnityEngine;

public class ActivateCanvasTrigger : MonoBehaviour
{
    // 在Inspector中拖入你要激活的Canvas对象
    public Canvas targetCanvas;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && targetCanvas != null)
        {
            // 激活Canvas游戏对象
            targetCanvas.gameObject.SetActive(true);

            // 如果需要同时控制Canvas组件本身的启用状态，可以使用：
            // targetCanvas.enabled = true;
        }
    }
}