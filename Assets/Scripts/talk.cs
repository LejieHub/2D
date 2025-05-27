using UnityEngine;
using System.Collections;

public class DialogueTrigger : MonoBehaviour
{
    [Header("UI Reference")]
    public CanvasGroup dialogueGroup; // ��Ҫ����CanvasGroup��UI���

    [Header("Timing Settings")]
    public float fadeDuration = 0.5f; // ���뵭������ʱ��
    public float displayDuration = 3f; // UI������ʾ��ʱ��

    private Coroutine fadeRoutine;      // ��ǰ���еĵ��뵭��Э��
    private Coroutine displayTimer;     // ��ʾ��ʱ��Э��

    private void Start()
    {
        // ��ʼ������UI
        dialogueGroup.alpha = 0;
        dialogueGroup.interactable = false;
        dialogueGroup.blocksRaycasts = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ֹͣ�������ڽ��еĶ���
            if (fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
            }

            // ��������Ч��
            fadeRoutine = StartCoroutine(FadeUI(1, fadeDuration));

            // ���ò�������ʾ��ʱ��
            if (displayTimer != null) StopCoroutine(displayTimer);
            displayTimer = StartCoroutine(DisplayTimer());
        }
    }

    private IEnumerator DisplayTimer()
    {
        yield return new WaitForSeconds(displayDuration);

        // ʱ���������������Ч��
        fadeRoutine = StartCoroutine(FadeUI(0, fadeDuration));
    }

    private IEnumerator FadeUI(float targetAlpha, float duration)
    {
        float startAlpha = dialogueGroup.alpha;
        float time = 0;

        // ����UI����״̬
        dialogueGroup.interactable = targetAlpha > 0.1f;
        dialogueGroup.blocksRaycasts = targetAlpha > 0.1f;

        while (time < duration)
        {
            time += Time.deltaTime;
            dialogueGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        // ȷ������ֵ׼ȷ
        dialogueGroup.alpha = targetAlpha;
    }

    // ��ѡ�������Ҫ����뿪��������
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // �����жϲ�����
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeUI(0, fadeDuration));
        }
    }
}