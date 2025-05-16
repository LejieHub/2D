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

        // ȡ���Զ����棬�ֶ�����λ��
        virtualCam.Follow = null;
    }

    void Update()
    {
        if (!zoomEnabled || player == null)
            return;

        // ����������Ѿ���ʧ�������ٻ����أ�
        if (mirror == null || !mirror.gameObject.activeInHierarchy)
        {
            // �л����Զ����汾��
            zoomEnabled = false;
            virtualCam.Follow = player;

            // ���Ż�Ĭ��ֵ
            virtualCam.m_Lens.OrthographicSize = Mathf.Lerp(
                virtualCam.m_Lens.OrthographicSize,
                minZoom,
                Time.deltaTime * zoomSpeed
            );

            return;
        }

        // --- ���������ʱ ---
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

        // �����е�λ�ò��ֶ��������λ��
        Vector3 center = (player.position + mirror.position) / 2f;
        Vector3 cameraPos = new Vector3(center.x, center.y, virtualCam.transform.position.z);

        virtualCam.transform.position = cameraPos;
    }
}
