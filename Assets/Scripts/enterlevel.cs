using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    [Header("过渡设置")]
    public float fadeDuration = 1f;
    public Color fadeColor = Color.black;

    [Header("组件引用")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private UnityEngine.UI.Image fadeImage;

    private void Start()
    {
        // 初始化时直接开始淡入效果
        StartCoroutine(FadeInRoutine());
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        // 淡出效果（变黑）
        yield return FadeOutRoutine();

        // 加载新场景
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeOutRoutine()
    {
        float timer = 0;
        while (timer < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    private IEnumerator FadeInRoutine()
    {
        // 初始设为全黑
        canvasGroup.alpha = 1;
        fadeImage.color = fadeColor;

        // 渐变到透明
        float timer = 0;
        while (timer < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
}