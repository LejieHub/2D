using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Settings")]
    public Transform repoint;
    public GameObject m_player;
    public MonoBehaviour playerMovementScript; // 玩家控制脚本（比如PlayerMovement）

    [Header("Scene Settings")]
    public string scenePaths;
    public GameObject deathUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadCheckpoint();
        if (m_player != null && repoint != null)
        {
            m_player.transform.position = repoint.position;
        }

        // 初始化隐藏死亡UI
        if (deathUI != null) deathUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    // 新增的死亡功能
    public void PlayerDied()
    {
        // 禁用玩家控制
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        // 显示死亡UI
        if (deathUI != null)
            deathUI.SetActive(true);

        // 暂停游戏
        Time.timeScale = 0f;
    }

    

    // 修改后的重启方法
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 保留原有功能
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