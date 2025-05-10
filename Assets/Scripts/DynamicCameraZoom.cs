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
    }

    void Update()
    {
        if (!zoomEnabled || player == null || mirror == null || !mirror.gameObject.activeInHierarchy)
            return;

        float distance = Mathf.Abs(player.position.x - mirror.position.x);

        float targetZoom = Mathf.Clamp(minZoom + distance * zoomMultiplier, minZoom, maxZoom);

        virtualCam.m_Lens.OrthographicSize = Mathf.Lerp(
            virtualCam.m_Lens.OrthographicSize,
            targetZoom,
            Time.deltaTime * zoomSpeed
        );
    }
}
