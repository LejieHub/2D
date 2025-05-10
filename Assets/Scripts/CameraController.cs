using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player; // 拖拽玩家对象到Inspector中
    public Transform leftBoundary; // 拖拽左边界物体到Inspector中
    public Transform rightBoundary; // 拖拽右边界物体到Inspector中

    private float cameraHalfWidth;
    private float minX;
    private float maxX;

    void Start()
    {
        // 计算相机的半宽
        Camera mainCamera = Camera.main;
        cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;

        // 计算镜头移动的边界
        minX = leftBoundary.position.x + cameraHalfWidth;
        maxX = rightBoundary.position.x - cameraHalfWidth;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // 获取目标X坐标并限制在边界范围内
        float targetX = Mathf.Clamp(player.position.x, minX, maxX);

        // 更新相机位置（保持Y轴不变）
        transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
    }
}