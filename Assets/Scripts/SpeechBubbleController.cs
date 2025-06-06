using UnityEngine;
using TMPro;
using System.Collections;

public class SpeechBubbleController : MonoBehaviour
{
    [Header("组件")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text textElement;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float displayDuration = 5f;

    private Coroutine currentRoutine;

    private void Start()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void ShowText(string content)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        textElement.text = content;
        currentRoutine = StartCoroutine(FadeInAndOut());
    }

    IEnumerator FadeInAndOut()
    {
        // 淡入
        float t = 0f;
        while (t < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(displayDuration);

        // 淡出
        t = 0f;
        while (t < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;

        currentRoutine = null;
    }
}
