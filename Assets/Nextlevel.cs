using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelTransitionByName : MonoBehaviour
{
    [Header("�ؿ�����")]
    [Tooltip("����Ŀ��ؿ��ĳ������ƣ����ִ�Сд��")]
    public string targetLevelName = "Level2";

    [Header("�������")]
    [SerializeField] private string playerTag = "Player";

    [Header("����Ч��")]
    [Tooltip("���뵭������ʱ�䣨�룩")]
    public float fadeDuration = 1f;
    [Tooltip("��Ҫ�����CanvasGroup���")]
    public CanvasGroup fadeCanvasGroup;

    [Header("����")]
    [SerializeField] private bool showDebugLogs = true;

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !isTransitioning)
        {
            if (showDebugLogs) Debug.Log("��Ҵ����˹ؿ�����");
            StartCoroutine(TransitionRoutine());
        }
    }

    private IEnumerator TransitionRoutine()
    {
        isTransitioning = true;

        // ����Ч��
        if (fadeCanvasGroup != null)
        {
            float timer = 0;
            while (timer < fadeDuration)
            {
                fadeCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
                timer += Time.deltaTime;
                yield return null;
            }
            fadeCanvasGroup.alpha = 1;
        }

        // �����³���
        if (IsSceneInBuildSettings(targetLevelName))
        {
            SceneManager.LoadScene(targetLevelName);
        }
        else
        {
            Debug.LogError($"���� {targetLevelName} δ��ӵ�Build Settings��");
            isTransitioning = false; // ����״̬
        }
    }

    private bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            var sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneNameInBuild == sceneName)
            {
                return true;
            }
        }
        return false;
    }

  
}