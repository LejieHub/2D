using UnityEngine;
using UnityEngine.SceneManagement;

public class SpriteSwitcher : MonoBehaviour
{
    public SpriteRenderer objectToShow;
    public SpriteRenderer objectToHide;
    public KeyCode activationKey = KeyCode.E;

    private bool playerInRange = false;

    void Start()
    {
        // 从GameManager获取保存的状态
        string doorID = GenerateDoorID();
        bool savedState = GameManager.Instance.GetDoorState(doorID);

        if (savedState)
        {
            SetDoorState(true);
        }
        else
        {
            SetInitialState();
        }
    }

    public string GenerateDoorID()
    {
        // 使用场景名+位置信息生成唯一ID（保留1位小数）
        Vector3 pos = transform.position;
        return $"{SceneManager.GetActiveScene().name}_{pos.x:F1}_{pos.y:F1}_{pos.z:F1}";
    }

    private void SetInitialState()
    {
        if (objectToHide != null) objectToHide.enabled = true;
        if (objectToShow != null) objectToShow.enabled = false;
    }

    // 公开方法用于从GameManager设置状态
    public void SetDoorState(bool state)
    {
        if (state)
        {
            if (objectToHide != null) objectToHide.enabled = false;
            if (objectToShow != null) objectToShow.enabled = true;
        }
        else
        {
            SetInitialState();
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(activationKey))
        {
            SwitchSprites();
        }
    }

    private void SwitchSprites()
    {
        if (objectToHide != null) objectToHide.enabled = false;
        if (objectToShow != null) objectToShow.enabled = true;

        // 保存状态到GameManager
        string doorID = GenerateDoorID();
        GameManager.Instance.SaveDoorState(doorID, true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}