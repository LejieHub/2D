using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Transform repoint;
    public GameObject m_player;
    public string scenePaths;
    // Start is called before the first frame update
    void Start()
    {

        LoadCheckpoint(); // 在游戏开始时加载存档点
        m_player.transform.position = repoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            restart();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
    public void start()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1f;

    }
    public void restart()
    {
        SceneManager.LoadScene(scenePaths);
        Time.timeScale = 1f;
    }

    public void mainmenu()
    {

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

            // 将玩家的位置设置为存档点的位置
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = checkpointPosition;
            }
        }
    }

}
