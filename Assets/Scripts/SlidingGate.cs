using UnityEngine;

public class SlidingGate : MonoBehaviour
{
    public enum SlideDirection
    {
        Vertical,
        Horizontal
    }

    [Header("�Ŷ�������")]
    public SlideDirection direction = SlideDirection.Vertical;
    public float closedSize = 5f;      // �ر�ʱ�ĳߴ磨�߶Ȼ��ȣ�
    public float openSize = 0.5f;      // ��ʱ��Ŀ��ߴ�
    public float animationSpeed = 5f;  // �����ٶ�

    private float targetSize;
    private Vector3 fixedPosition;     // ���̶ֹ��˵�λ��

    void Start()
    {
        targetSize = closedSize;

        Vector3 scale = transform.localScale;

        // ��¼�̶���λ�ã���������ࣩ
        if (direction == SlideDirection.Vertical)
        {
            fixedPosition = transform.position + new Vector3(0, scale.y / 2f, 0);
        }
        else
        {
            fixedPosition = transform.position - new Vector3(scale.x / 2f, 0, 0); // �̶����
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
