using UnityEngine;
using System.Collections;

public class HideSpriteTrigger : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private SpriteRenderer targetSprite; // Ҫ���ص�ͼƬ
    [SerializeField] private float fadeDuration = 0.3f;   // ����ʱ��

    private Color originalColor;
    private bool isVisible = true;
    private Coroutine activeCoroutine;

    private void Start()
    {
        if (targetSprite == null)
        {
            Debug.LogWarning("δ����Ŀ��ͼƬ! �뽫SpriteRenderer��ק��Ŀ��ͼƬ�ֶΡ�");
            return;
        }

        // ����ԭʼ��ɫ
        originalColor = targetSprite.color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && targetSprite != null && isVisible)
        {
            HideSprite();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && targetSprite != null && !isVisible)
        {
            ShowSprite();
        }
    }

    private void HideSprite()
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        isVisible = false;
        activeCoroutine = StartCoroutine(FadeSprite(false));
    }

    private void ShowSprite()
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        isVisible = true;
        activeCoroutine = StartCoroutine(FadeSprite(true));
    }

    private IEnumerator FadeSprite(bool show)
    {
        Color startColor = targetSprite.color;
        Color targetColor = show ? originalColor : new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        float elapsed = 0;

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;

            // ʹ��ƽ���Ļ�������
            float smoothT = t * t * (3f - 2f * t);

            targetSprite.color = Color.Lerp(startColor, targetColor, smoothT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        targetSprite.color = targetColor;
    }

    // ���Կ��ӻ���������
    private void OnDrawGizmos()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null) return;

        Gizmos.color = new Color(1, 0.5f, 0, 0.3f); // ��ɫ��͸��

        if (collider is BoxCollider2D boxCollider)
        {
            Vector2 size = boxCollider.size;
            Vector2 offset = boxCollider.offset;
            Vector2 position = transform.position;

            Gizmos.matrix = Matrix4x4.TRS(
                new Vector3(position.x + offset.x, position.y + offset.y, 0),
                transform.rotation,
                transform.lossyScale
            );

            Gizmos.DrawCube(Vector3.zero, new Vector3(size.x, size.y, 0.1f));
        }
        else if (collider is CircleCollider2D circleCollider)
        {
            Gizmos.matrix = Matrix4x4.TRS(
                transform.position,
                transform.rotation,
                transform.lossyScale
            );

            Gizmos.DrawSphere(Vector3.zero, circleCollider.radius);
        }
    }
}