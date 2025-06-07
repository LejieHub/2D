using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Settings")]
    public Transform repoint;
    public GameObject m_player;
    public MonoBehaviour playerMovementScript; // ��ҿ��ƽű�������PlayerMovement��

    [Header("UI Settings")]
    public GameObject deathUI;
    public GameObject pauseUI; // ������ͣUI

    [Header("Scene Settings")]
    public string scenePaths;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ȷ���糡��ʱ������
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
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause(); // ESC���л���ͣ
    }

    // ==== ��������ͣ���� ====
    public void TogglePause()
    {
        if (deathUI != null && deathUI.activeSelf) return; // ����״̬�²�����ͣ

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (pauseUI != null) pauseUI.SetActive(isPaused);

        // �л���ҿ���
        if (playerMovementScript != null)
            playerMovementScript.enabled = !isPaused;
    }

    // ==== ��ť���õ���ͣ/�ָ����� ====
    public void PauseGame()
    {
        if (!isPaused) TogglePause();
    }

    public void ResumeGame()
    {
        if (isPaused) TogglePause();
    }

    // ==== �������� ====
    public void PlayerDied()
    {
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (deathUI != null)
            deathUI.SetActive(true);

        Time.timeScale = 0f;
    }

    // ==== �������� ====
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