using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    [Header("��������")]
    public float fadeDuration = 1f;
    public Color fadeColor = Color.black;

    [Header("�������")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private UnityEngine.UI.Image fadeImage;

    private void Start()
    {
        // ��ʼ��ʱֱ�ӿ�ʼ����Ч��
        StartCoroutine(FadeInRoutine());
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        // ����Ч������ڣ�
        yield return FadeOutRoutine();

        // �����³���
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
        // ��ʼ��Ϊȫ��
        canvasGroup.alpha = 1;
        fadeImage.color = fadeColor;

        // ���䵽͸��
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