using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelTransitionByName : MonoBehaviour
{
    [Header("关卡设置")]
    [Tooltip("输入目标关卡的场景名称（区分大小写）")]
    public string targetLevelName = "Level2";

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !isTransitioning)
        {
            if (showDebugLogs) Debug.Log("玩家触发了关卡传送");
            StartCoroutine(TransitionRoutine());
        }
    }

    private IEnumerator TransitionRoutine()
    {
        isTransitioning = true;

        // 淡出效果
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

        // 加载新场景
        if (IsSceneInBuildSettings(targetLevelName))
        {
            SceneManager.LoadScene(targetLevelName);
        }
        else
        {
            Debug.LogError($"场景 {targetLevelName} 未添加到Build Settings！");
            isTransitioning = false; // 重置状态
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