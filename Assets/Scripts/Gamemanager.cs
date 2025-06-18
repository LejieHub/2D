using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Settings")]
    public Transform repoint;
    public GameObject m_player;
    public MonoBehaviour playerMovementScript; // 玩家控制脚本（比如PlayerMovement）

    [Header("UI Settings")]
    public GameObject deathUI;
    public GameObject pauseUI; // 新增暂停UI

    [Header("Scene Settings")]
    public string scenePaths;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 确保跨场景时不销毁
            SceneManager.sceneLoaded += OnSceneLoaded; // 添加场景加载事件监听
        }
        
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 移除事件监听
    }

    // 场景加载完成时的回调
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 重新获取场景中的关键对象引用
        FindSceneReferences();
        InitializeUI();
        PositionPlayer();
    }

    void Start()
    {
        FindSceneReferences();
        InitializeUI();
        LoadCheckpoint();
        PositionPlayer();
    }

    void Update()
    {
        HandleInput();
    }

    // 查找场景中的关键对象引用
    private void FindSceneReferences()
    {
        // 查找玩家对象
        if (m_player == null)
        {
            m_player = GameObject.FindGameObjectWithTag("Player");
        }

        // 查找重生点
        if (repoint == null)
        {
            GameObject checkpointObj = GameObject.FindGameObjectWithTag("Respawn");
            if (checkpointObj) repoint = checkpointObj.transform;
        }

        // 查找UI对象
        if (deathUI == null)
        {
            deathUI = GameObject.Find("DeathUI");
        }

        if (pauseUI == null)
        {
            pauseUI = GameObject.Find("PauseUI");
        }

        // 查找玩家控制脚本（不需要指定具体类型）
        if (playerMovementScript == null && m_player != null)
        {
            // 查找玩家对象上的第一个脚本，通常是控制脚本
            MonoBehaviour[] scripts = m_player.GetComponents<MonoBehaviour>();
            if (scripts.Length > 0)
            {
                playerMovementScript = scripts[0]; // 使用第一个脚本
                Debug.Log("Found player control script: " + playerMovementScript.GetType().Name);
            }
        }
    }

    private void InitializeUI()
    {
        if (deathUI != null) deathUI.SetActive(false);
        if (pauseUI != null) pauseUI.SetActive(false);
    }

    private void PositionPlayer()
    {
        if (m_player != null && repoint != null)
        {
            m_player.transform.position = repoint.position;
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.R)) Restart();
        if (Input.GetKeyDown(KeyCode.P)) PlayerPrefs.DeleteAll();
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause(); // ESC键切换暂停
    }

    // ==== 暂停功能 ====
    public void TogglePause()
    {
        // 确保引用了正确的UI对象
        if (deathUI != null && deathUI.activeSelf) return; // 死亡状态下不可暂停

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (pauseUI != null)
        {
            pauseUI.SetActive(isPaused);
        }

        // 切换玩家控制
        if (playerMovementScript != null)
            playerMovementScript.enabled = !isPaused;
    }

    // ==== 按钮调用的暂停/恢复方法 ====
    public void PauseGame()
    {
        if (!isPaused) TogglePause();
    }

    public void ResumeGame()
    {
        if (isPaused) TogglePause();
    }

    // ==== 死亡功能 ====
    public void PlayerDied()
    {
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (deathUI != null)
        {
            deathUI.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    // ==== 场景管理 ====
    public void Restart()
    {
        Time.timeScale = 1f;
        isPaused = false; // 重置暂停状态
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("menu");
    }

    void LoadCheckpoint()
    {
        if (PlayerPrefs.HasKey("CheckpointX") && PlayerPrefs.HasKey("CheckpointY") && PlayerPrefs.HasKey("CheckpointZ"))
        {
            float x = PlayerPrefs.GetFloat("CheckpointX");
            float y = PlayerPrefs.GetFloat("CheckpointY");
            float z = PlayerPrefs.GetFloat("CheckpointZ");

            Vector3 checkpointPosition = new Vector3(x, y, z);
            if (m_player != null)
            {
                m_player.transform.position = checkpointPosition;
            }
        }
    }
}