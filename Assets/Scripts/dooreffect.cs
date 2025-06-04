using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Cinemachine; // 添加Cinemachine命名空间

public class DoorTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    public Image transitionOverlay;
    public float transitionDuration = 2f;

    [Header("Camera Shake Settings")]
    public bool enableCameraShake = true;
    public CinemachineImpulseSource impulseSource; // 需要关联Cinemachine Impulse Source
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
        // 触发镜头抖动
        if (enableCameraShake && impulseSource != null)
        {
            impulseSource.GenerateImpulse(shakeForce);
        }

        // 第一阶段：淡入黑色
        yield return StartCoroutine(FadeTransition(Color.black, 0, 1, transitionDuration / 3));

        // 第二阶段：保持黑色
        yield return new WaitForSeconds(transitionDuration / 3);

        // 第三阶段：淡出到白色
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