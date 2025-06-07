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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeUI();
        LoadCheckpoint();
        PositionPlayer();
    }

    void Update()
    {
        HandleInput();
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

    // ==== 新增的暂停功能 ====
    public void TogglePause()
    {
        if (deathUI != null && deathUI.activeSelf) return; // 死亡状态下不可暂停

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (pauseUI != null) pauseUI.SetActive(isPaused);

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
            deathUI.SetActive(true);

        Time.timeScale = 0f;
    }

    // ==== 场景管理 ====
    public void Restart()
    {
        Time.timeScale = 1f;
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