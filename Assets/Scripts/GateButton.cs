using UnityEngine;
using System.Collections;

public class GateButton : MonoBehaviour
{
    public SlidingGate slidingGate;

    public enum AnchorPoint { Bottom, Top, Center }

    [Header("��ť�Ӿ�����")]
    public Transform buttonVisual;
    public AnchorPoint pressAnchor = AnchorPoint.Bottom;

    [Header("��ѹ��������")]
    public Vector2 pressedScale = new Vector2(1f, 0.6f);
    public float pressDistance = 0.0f;
    public float pressDuration = 0.2f;

    private Vector3 originalScale;
    private Vector3 originalLocalPos;
    private Coroutine currentAnim;

    private int pressCount = 0;

    private void Start()
    {
        if (buttonVisual != null)
        {
            originalScale = buttonVisual.localScale;
            originalLocalPos = buttonVisual.localPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        pressCount++;
        if (pressCount == 1)
        {
            slidingGate.OpenGate();
            PlayPressAnimation(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        pressCount--;
        if (pressCount <= 0)
        {
            slidingGate.CloseGate();
            PlayPressAnimation(false);
        }
    }

    private void PlayPressAnimation(bool pressed)
    {
        if (buttonVisual == null) return;
        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(AnimateButton(pressed));
    }

    private IEnumerator AnimateButton(bool pressed)
    {
        Vector3 startScale = buttonVisual.localScale;
        Vector3 startPos = buttonVisual.localPosition;

        // Ŀ������
        Vector3 targetScale = pressed
            ? new Vector3(originalScale.x * pressedScale.x,
                          originalScale.y * pressedScale.y,
                          originalScale.z)
            : originalScale;

        // ����߶Ȳ����ê��������
        float heightDiff = originalScale.y - targetScale.y;
        Vector3 positionOffset = Vector3.zero;

        if (pressed)
        {
            switch (pressAnchor)
            {
                case AnchorPoint.Bottom:
                    positionOffset = Vector3.down * (pressDistance + heightDiff / 2f);
                    break;
                case AnchorPoint.Top:
                    positionOffset = Vector3.up * (pressDistance - heightDiff / 2f);
                    break;
                case AnchorPoint.Center:
                    positionOffset = Vector3.down * pressDistance;
                    break;
            }
        }

        Vector3 targetPos = pressed ? originalLocalPos + positionOffset : originalLocalPos;

        float elapsed = 0f;
        while (elapsed < pressDuration)
        {
            float t = elapsed / pressDuration;
            buttonVisual.localScale = Vector3.Lerp(startScale, targetScale, t);
            buttonVisual.localPosition = Vector3.Lerp(startPos, targetPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        buttonVisual.localScale = targetScale;
        buttonVisual.localPosition = targetPos;
    }
}
