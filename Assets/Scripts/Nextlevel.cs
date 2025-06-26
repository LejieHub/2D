using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelTransitionByName : MonoBehaviour
{
    public bool mainLevel;

    [Header("关卡设置")]
    [Tooltip("输入目标关卡的场景名称（区分大小写）")]
    public string targetLevelName = "Level2";

    [Header("钥匙设置")]
    [Tooltip("是否需要钥匙才能进入")]
    public bool requireKey = true;

    [Tooltip("玩家需要拥有的钥匙ID才允许进入该门")]
    public string requiredKeyID;

    [Header("玩家设置")]
    [SerializeField] private string playerTag = "Player";

    [Header("过渡效果")]
    [Tooltip("淡入淡出持续时间（秒）")]
    public float fadeDuration = 1f;
    [Tooltip("需要渐变的CanvasGroup组件")]
    public CanvasGroup fadeCanvasGroup;

    [Header("调试")]
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
                if (showDebugLogs) Debug.Log("玩家拥有钥匙，触发传送");
                StartCoroutine(TransitionRoutine());
            }
            else
            {
                if (showDebugLogs) Debug.LogWarning($"玩家缺少钥匙 {requiredKeyID}，无法通过");
            }
        }
        else
        {
            if (showDebugLogs) Debug.Log("不需要钥匙，直接传送");
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
            Debug.LogError($"场景 {targetLevelName} 未添加到 Build Settings！");
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
