using UnityEngine;

public class ActivateCanvasTrigger : MonoBehaviour
{
    // ��Inspector��������Ҫ�����Canvas����
    public Canvas targetCanvas;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && targetCanvas != null)
        {
            // ����Canvas��Ϸ����
            targetCanvas.gameObject.SetActive(true);

            // �����Ҫͬʱ����Canvas������������״̬������ʹ�ã�
            // targetCanvas.enabled = true;
        }
    }
}