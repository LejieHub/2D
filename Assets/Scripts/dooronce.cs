using UnityEngine;

public class GateController : MonoBehaviour
{
    public enum AnchorPoint { Left, Right, Bottom, Top, Center }

    [Header("Compression Settings")]
    public AnchorPoint anchor = AnchorPoint.Left;
    public Vector2 compressionScale = new Vector2(0.2f, 1f);
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

        Vector3 positionOffset = Vector3.zero;
        float scaleDifference;

        // 精确计算边缘锚点偏移
        switch (anchor)
        {
            case AnchorPoint.Left:
                scaleDifference = originalScale.x - targetScale.x;
                positionOffset = Vector3.left * (scaleDifference * 0.5f);
                break;

            case AnchorPoint.Right:
                scaleDifference = originalScale.x - targetScale.x;
                positionOffset = Vector3.right * (scaleDifference * 0.5f);
                break;

            case AnchorPoint.Bottom:
                scaleDifference = originalScale.y - targetScale.y;
                positionOffset = Vector3.down * (scaleDifference * 0.5f);
                break;

            case AnchorPoint.Top:
                scaleDifference = originalScale.y - targetScale.y;
                positionOffset = Vector3.up * (scaleDifference * 0.5f);
                break;

            case AnchorPoint.Center:
                // 中心锚点不需要偏移
                break;
        }

        Vector3 targetPosition = originalPosition + positionOffset;

        while (elapsedTime < compressionDuration)
        {
            float t = elapsedTime / compressionDuration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            transform.position = Vector3.Lerp(originalPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保最终状态精确
        transform.localScale = targetScale;
        transform.position = targetPosition;
        isCompressed = true;
    }

    public void ResetGate()
    {
        transform.localScale = originalScale;
        transform.position = originalPosition;
        isCompressed = false;
    }
}