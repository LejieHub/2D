// ButtonController.cs
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public enum AnchorPoint { Bottom, Top, Center }

    [Header("Settings")]
    public GateController targetGate;
    public AnchorPoint pressAnchor = AnchorPoint.Bottom;

    [Header("Press Animation")]
    [SerializeField] private float pressDistance = 0.2f;
    [SerializeField] private Vector2 pressScale = new Vector2(1.2f, 0.6f);
    [SerializeField] private float pressDuration = 0.5f;

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private bool isPressed = false;

    void Start()
    {
        originalScale = transform.localScale;
        originalPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isPressed && other.CompareTag("Player"))
        {
            StartCoroutine(ButtonPressAnimation());
        }
    }

    private System.Collections.IEnumerator ButtonPressAnimation()
    {
        isPressed = true;

        // 计算目标状态
        Vector3 targetScale = new Vector3(
            originalScale.x * pressScale.x,
            originalScale.y * pressScale.y,
            originalScale.z
        );

        // 根据锚点计算位置偏移
        float heightDifference = originalScale.y - targetScale.y;
        Vector3 positionOffset = Vector3.zero;

        switch (pressAnchor)
        {
            case AnchorPoint.Bottom:
                positionOffset = Vector3.down * (pressDistance + heightDifference / 2f);
                break;
            case AnchorPoint.Top:
                positionOffset = Vector3.up * (pressDistance - heightDifference / 2f);
                break;
            case AnchorPoint.Center:
                positionOffset = Vector3.down * pressDistance;
                break;
        }

        Vector3 targetPosition = originalPosition + positionOffset;

        float elapsedTime = 0;
        while (elapsedTime < pressDuration)
        {
            float t = elapsedTime / pressDuration;

            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            transform.position = Vector3.Lerp(originalPosition, targetPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        transform.position = targetPosition;

        if (targetGate != null)
        {
            targetGate.CompressGate();
        }

        GetComponent<Collider2D>().enabled = false;
    }
}