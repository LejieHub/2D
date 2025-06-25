using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelTransitionByName : MonoBehaviour
{
    public bool mainLevel;

    [Header("�ؿ�����")]
    [Tooltip("����Ŀ��ؿ��ĳ������ƣ����ִ�Сд��")]
    public string targetLevelName = "Level2";

    [Header("Կ������")]
    [Tooltip("�Ƿ���ҪԿ�ײ��ܽ���")]
    public bool requireKey = true;

    [Tooltip("�����Ҫӵ�е�Կ��ID������������")]
    public string requiredKeyID;

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

    public GameObject player;

    private bool isPlayerInTrigger = false;

    private void Update()
    {
        if (isPlayerInTrigger && !isTransitioning && Input.GetKeyDown(KeyCode.F))
        {
            TryTransition();
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInTrigger = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInTrigger = false;
        }
    }

    private void TryTransition()
    {
        if (requireKey)
        {
            if (KeyManager.Instance.CheckKey(requiredKeyID))
            {
                if (showDebugLogs) Debug.Log("���ӵ��Կ�ף���������");
                StartCoroutine(TransitionRoutine());
            }
            else
            {
                if (showDebugLogs) Debug.LogWarning($"���ȱ��Կ�� {requiredKeyID}���޷�ͨ��");
            }
        }
        else
        {
            if (showDebugLogs) Debug.Log("����ҪԿ�ף�ֱ�Ӵ���");
            StartCoroutine(TransitionRoutine());
        }
    }

    private IEnumerator TransitionRoutine()
    {
        isTransitioning = true;

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

        if (player != null && mainLevel)
        {
            GameManager.Instance.lastPlayerPosition = player.transform.position;
            GameManager.Instance.shouldLoadPosition = true;
        }

        if (IsSceneInBuildSettings(targetLevelName))
        {
            SceneManager.LoadScene(targetLevelName);
        }
        else
        {
            Debug.LogError($"���� {targetLevelName} δ��ӵ� Build Settings��");
            isTransitioning = false;
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
