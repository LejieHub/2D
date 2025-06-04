using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Cinemachine; // ���Cinemachine�����ռ�

public class DoorTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    public Image transitionOverlay;
    public float transitionDuration = 2f;

    [Header("Camera Shake Settings")]
    public bool enableCameraShake = true;
    public CinemachineImpulseSource impulseSource; // ��Ҫ����Cinemachine Impulse Source
    public float shakeForce = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("door"))
        {
            StartCoroutine(TransitionEffect());
        }
    }

    private IEnumerator TransitionEffect()
    {
        // ������ͷ����
        if (enableCameraShake && impulseSource != null)
        {
            impulseSource.GenerateImpulse(shakeForce);
        }

        // ��һ�׶Σ������ɫ
        yield return StartCoroutine(FadeTransition(Color.black, 0, 1, transitionDuration / 3));

        // �ڶ��׶Σ����ֺ�ɫ
        yield return new WaitForSeconds(transitionDuration / 3);

        // �����׶Σ���������ɫ
        yield return StartCoroutine(FadeTransition(Color.black, 1, 0, transitionDuration / 3));
    }

    private IEnumerator FadeTransition(Color targetColor, float startAlpha, float endAlpha, float duration)
    {
        float timer = 0f;
        Color startColor = new Color(targetColor.r, targetColor.g, targetColor.b, startAlpha);
        Color endColor = new Color(targetColor.r, targetColor.g, targetColor.b, endAlpha);

        while (timer < duration)
        {
            transitionOverlay.color = Color.Lerp(startColor, endColor, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        transitionOverlay.color = endColor;
    }
}