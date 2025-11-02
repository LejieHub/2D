using UnityEngine;
using System.Collections;

public class EndCreditScroll : MonoBehaviour
{
    public GameObject creditTextObject;   // 拖入字幕 GameObject
    public float scrollSpeed = 10f;        // 控制滚动速度
    public float targetYPosition = 5f;     // 滚动到这个 Y 值后停下

    public GameObject byTextObject;

    public float waitBeforeFade = 2f;
    public float fadeDuration = 2f;
    private bool hasFinished = false;

    private SpriteRenderer creditRenderer;
    private SpriteRenderer byRenderer;

    private bool isScrolling = false;

    void Start()
    {
        // 初始时隐藏字幕
        creditTextObject.SetActive(false);
        byTextObject.SetActive(false);

        creditRenderer = creditTextObject.GetComponent<SpriteRenderer>();
        byRenderer = byTextObject.GetComponent<SpriteRenderer>();

        // 确保初始透明状态
        if (creditRenderer != null)
            creditRenderer.color = new Color(1, 1, 1, 1);

        if (byRenderer != null)
            byRenderer.color = new Color(1, 1, 1, 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasFinished) // 确保只有玩家触发
        {
            creditTextObject.SetActive(true);
            isScrolling = true;
            hasFinished = true;
        }
    }

    void Update()
    {
        if (isScrolling)
        {
            Vector3 currentPos = creditTextObject.transform.position;
            if (currentPos.y < targetYPosition)
            {
                // 缓慢向上移动
                currentPos.y += scrollSpeed * Time.deltaTime;
                creditTextObject.transform.position = currentPos;
            }
            else
            {
                // 达到目标高度，停止滚动
                isScrolling = false;
                StartCoroutine(FadeSequence());
            }
        }
    }

    IEnumerator FadeSequence()
    {
        yield return new WaitForSeconds(waitBeforeFade);

        // 渐隐滚动字幕
        yield return StartCoroutine(FadeOut(creditRenderer, fadeDuration));
        creditTextObject.SetActive(false);

        // 渐显 by xxx
        byTextObject.SetActive(true);
        yield return StartCoroutine(FadeIn(byRenderer, fadeDuration));
    }

    IEnumerator FadeOut(SpriteRenderer renderer, float duration)
    {
        if (renderer == null) yield break;

        float elapsed = 0f;
        Color startColor = renderer.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            renderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        renderer.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
    }

    IEnumerator FadeIn(SpriteRenderer renderer, float duration)
    {
        if (renderer == null) yield break;

        float elapsed = 0f;
        Color startColor = renderer.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            renderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        renderer.color = new Color(startColor.r, startColor.g, startColor.b, 1f);
    }
}
