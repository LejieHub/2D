using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player; // ��ק��Ҷ���Inspector��
    public Transform leftBoundary; // ��ק��߽����嵽Inspector��
    public Transform rightBoundary; // ��ק�ұ߽����嵽Inspector��

    private float cameraHalfWidth;
    private float minX;
    private float maxX;

    void Start()
    {
        // ��������İ��
        Camera mainCamera = Camera.main;
        cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;

        // ���㾵ͷ�ƶ��ı߽�
        minX = leftBoundary.position.x + cameraHalfWidth;
        maxX = rightBoundary.position.x - cameraHalfWidth;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // ��ȡĿ��X���겢�����ڱ߽緶Χ��
        float targetX = Mathf.Clamp(player.position.x, minX, maxX);

        // �������λ�ã�����Y�᲻�䣩
        transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
    }
}