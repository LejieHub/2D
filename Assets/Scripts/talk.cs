using UnityEngine;
using System.Collections;

public class DialogueTrigger : MonoBehaviour
{
    [Header("UI Reference")]
    public CanvasGroup dialogueGroup; // 需要包含CanvasGroup的UI面板

    [Header("Timing Settings")]
    public float fadeDuration = 0.5f; // 淡入淡出持续时间
    public float displayDuration = 3f; // UI保持显示的时间

    private Coroutine fadeRoutine;      // 当前运行的淡入淡出协程
    private Coroutine displayTimer;     // 显示计时器协程

    private void Start()
    {
        // 初始化隐藏UI
        dialogueGroup.alpha = 0;
        dialogueGroup.interactable = false;
        dialogueGroup.blocksRaycasts = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 停止所有正在进行的动画
            if (fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
            }

            // 启动淡入效果
            fadeRoutine = StartCoroutine(FadeUI(1, fadeDuration));

            // 重置并启动显示计时器
            if (displayTimer != null) StopCoroutine(displayTimer);
            displayTimer = StartCoroutine(DisplayTimer());
        }
    }

    private IEnumerator DisplayTimer()
    {
        yield return new WaitForSeconds(displayDuration);

        // 时间结束后启动淡出效果
        fadeRoutine = StartCoroutine(FadeUI(0, fadeDuration));
    }

    private IEnumerator FadeUI(float targetAlpha, float duration)
    {
        float startAlpha = dialogueGroup.alpha;
        float time = 0;

        // 更新UI交互状态
        dialogueGroup.interactable = targetAlpha > 0.1f;
        dialogueGroup.blocksRaycasts = targetAlpha > 0.1f;

        while (time < duration)
        {
            time += Time.deltaTime;
            dialogueGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        // 确保最终值准确
        dialogueGroup.alpha = targetAlpha;
    }

    // 可选：如果需要玩家离开立即隐藏
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 立即中断并淡出
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeUI(0, fadeDuration));
        }
    }
}