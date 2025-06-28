using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Player Position")]
    public Vector2 lastPlayerPosition;
    public bool shouldLoadPosition;

    [Header("Player Settings")]
    public Transform repoint;
    public GameObject m_player;
    public MonoBehaviour playerMovementScript; // ��ҿ��ƽű�������PlayerMovement��

    [Header("UI Settings")]
    public GameObject deathUI;
    public GameObject pauseUI; // ������ͣUI

    [Header("Scene Settings")]
    public string scenePaths;

    [Header("Door State Management")]
    public Dictionary<string, bool> doorStates = new Dictionary<string, bool>();

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ȷ���糡��ʱ������
            SceneManager.sceneLoaded += OnSceneLoaded; // ��ӳ��������¼�����
            doorStates = new Dictionary<string, bool>(); // ��ʼ���ֵ�
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // �Ƴ��¼�����
    }

    // �����������ʱ�Ļص�
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ���»�ȡ�����еĹؼ���������
        FindSceneReferences();
        InitializeUI();
        PositionPlayer();
        RestoreDoorStates(); // �ָ��ŵ�״̬
    }

    void Start()
    {
        FindSceneReferences();
        InitializeUI();
        LoadCheckpoint();
        PositionPlayer();
        RestoreDoorStates(); // ȷ����ʼʱ�ָ���״̬
    }

    void Update()
    {
        HandleInput();
    }

    // ���ҳ����еĹؼ���������
    private void FindSceneReferences()
    {
        // ������Ҷ���
        if (m_player == null)
        {
            m_player = GameObject.FindGameObjectWithTag("Player");
        }

        // ����������
        if (repoint == null)
        {
            GameObject checkpointObj = GameObject.FindGameObjectWithTag("Respawn");
            if (checkpointObj) repoint = checkpointObj.transform;
        }

        // ����UI����
        if (deathUI == null)
        {
            deathUI = GameObject.Find("DeathUI");
        }

        if (pauseUI == null)
        {
            pauseUI = GameObject.Find("PauseUI");
        }

        // ������ҿ��ƽű�������Ҫָ���������ͣ�
        if (playerMovementScript == null && m_player != null)
        {
            // ������Ҷ����ϵĵ�һ���ű���ͨ���ǿ��ƽű�
            MonoBehaviour[] scripts = m_player.GetComponents<MonoBehaviour>();
            if (scripts.Length > 0)
            {
                playerMovementScript = scripts[0]; // ʹ�õ�һ���ű�
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
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause(); // ESC���л���ͣ
    }

    // ==== ��״̬���� ====
    // �����ŵ�״̬
    public void SaveDoorState(string doorID, bool state)
    {
        if (doorStates.ContainsKey(doorID))
        {
            doorStates[doorID] = state;
        }
        else
        {
            doorStates.Add(doorID, state);
        }

        Debug.Log($"Saved door state: {doorID} = {state}");
    }

    // ��ȡ�ŵ�״̬
    public bool GetDoorState(string doorID)
    {
        if (doorStates.ContainsKey(doorID))
        {
            return doorStates[doorID];
        }
        return false;
    }

    // �ָ������ŵ�״̬
    private void RestoreDoorStates()
    {
        // ��ȡ���������е�SimpleSpriteSwitcher���
        SpriteSwitcher[] switchers = FindObjectsOfType<SpriteSwitcher>();

        if (switchers == null || switchers.Length == 0) return;

        Debug.Log($"Found {switchers.Length} door switches to restore");

        foreach (var switcher in switchers)
        {
            // ����ΨһID�������� + ������
            string doorID = $"{SceneManager.GetActiveScene().name}_{switcher.gameObject.name}";
            bool state = GetDoorState(doorID);

            if (state)
            {
                switcher.SetDoorState(state);
                Debug.Log($"Restored door {doorID}: {state}");
            }
        }
    }

    // �����״̬����������Ϸ��
    public void ClearDoorStates()
    {
        doorStates.Clear();
        Debug.Log("Cleared all door states");
    }

    // ==== ��ͣ���� ====
    public void TogglePause()
    {
        // ȷ����������ȷ��UI����
        if (deathUI != null && deathUI.activeSelf) return; // ����״̬�²�����ͣ

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (pauseUI != null)
        {
            pauseUI.SetActive(isPaused);
        }

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
        {
            deathUI.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    // ==== �������� ====
    public void Restart()
    {
        Time.timeScale = 1f;
        isPaused = false; // ������ͣ״̬
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    public void StartGame()
    {
        ClearDoorStates(); // ����Ϸʱ�����״̬
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