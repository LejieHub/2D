using UnityEngine;
using Cinemachine;

public class DynamicCameraZoom : MonoBehaviour
{
    [Header("Camera References")]
    private CinemachineVirtualCamera virtualCam;

    [Header("Zoom Settings")]
    public float minZoom = 5f;
    public float maxZoom = 10f;
    public float zoomMultiplier = 0.1f;
    public float zoomSpeed = 3f;

    [Header("Targets")]
    public Transform player;
    private Transform mirror;

    private bool zoomEnabled = false;

    void Awake()
    {
        virtualCam = GetComponent<CinemachineVirtualCamera>();
    }

    public void SetMirror(Transform mirrorTarget)
    {
        mirror = mirrorTarget;
        zoomEnabled = true;

        // 取消自动跟随，手动控制位置
        virtualCam.Follow = null;
    }

    void Update()
    {
        if (!zoomEnabled || player == null)
            return;

        // 如果镜像体已经消失（被销毁或隐藏）
        if (mirror == null || !mirror.gameObject.activeInHierarchy)
        {
            // 切换回自动跟随本体
            zoomEnabled = false;
            virtualCam.Follow = player;

            // 缩放回默认值
            virtualCam.m_Lens.OrthographicSize = Mathf.Lerp(
                virtualCam.m_Lens.OrthographicSize,
                minZoom,
                Time.deltaTime * zoomSpeed
            );

            return;
        }

        // --- 镜像体存在时 ---
        float distance = Mathf.Abs(player.position.x - mirror.position.x);
        float targetZoom = Mathf.Clamp(minZoom + distance * zoomMultiplier, minZoom, maxZoom);

        virtualCam.m_Lens.OrthographicSize = Mathf.Lerp(
            virtualCam.m_Lens.OrthographicSize,
            targetZoom,
            Time.deltaTime * zoomSpeed
        );
    }

    void LateUpdate()
    {
        if (!zoomEnabled || player == null || mirror == null || !mirror.gameObject.activeInHierarchy)
            return;

        // 计算中点位置并手动设置相机位置
        Vector3 center = (player.position + mirror.position) / 2f;
        Vector3 cameraPos = new Vector3(center.x, center.y, virtualCam.transform.position.z);

        virtualCam.transform.position = cameraPos;
    }
}
