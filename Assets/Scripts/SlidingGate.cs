using UnityEngine;

public class SlidingGate : MonoBehaviour
{
    [Header("�Ŷ�������")]
    public float closedHeight = 5f;     // Ĭ�Ϲر�ʱ�ĸ߶�
    public float openHeight = 0.5f;     // ����ʱ��Ŀ��߶�
    public float animationSpeed = 5f;   // �ƶ��ٶ�

    private float targetHeight;
    private Vector3 basePosition;       // �̶��ײ�λ��

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

        // ��������
        scale.y = newHeight;
        transform.localScale = scale;

        // ����λ���Ա��ֵײ�����
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
