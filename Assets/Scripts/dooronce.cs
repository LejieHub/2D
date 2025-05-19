// GateController.cs
using UnityEngine;

public class GateController : MonoBehaviour
{
    public enum AnchorPoint { Bottom, Top, Center }

    [Header("Compression Settings")]
    public AnchorPoint anchor = AnchorPoint.Bottom;
    public Vector2 compressionScale = new Vector2(1f, 0.2f);
    public float compressionDuration = 1f;

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private bool isCompressed = false;

    void Start()
    {
        originalScale = transform.localScale;
        originalPosition = transform.position;
    }

    public void CompressGate()
    {
        if (!isCompressed)
        {
            StartCoroutine(CompressionAnimation());
        }
    }

    private System.Collections.IEnumerator CompressionAnimation()
    {
        float elapsedTime = 0;
        Vector3 targetScale = new Vector3(
            originalScale.x * compressionScale.x,
            originalScale.y * compressionScale.y,
            originalScale.z
        );

        // ����ê�����λ��ƫ��
        float heightDifference = originalScale.y - targetScale.y;
        Vector3 positionOffset = Vector3.zero;

        switch (anchor)
        {
            case AnchorPoint.Bottom:
                positionOffset = Vector3.down * (heightDifference / 2f);
                break;
            case AnchorPoint.Top:
                positionOffset = Vector3.up * (heightDifference / 2f);
                break;
            case AnchorPoint.Center:
                // λ�ñ��ֲ���
                break;
        }

        Vector3 targetPosition = originalPosition + positionOffset;

        while (elapsedTime < compressionDuration)
        {
            float t = elapsedTime / compressionDuration;

            // ͬʱ�������ź�λ�ò�ֵ
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            transform.position = Vector3.Lerp(originalPosition, targetPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        transform.position = targetPosition;
        isCompressed = true;
    }
}