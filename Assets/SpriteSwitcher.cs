using UnityEngine;
using UnityEngine.SceneManagement; // 添加这行

public class SpriteSwitcher : MonoBehaviour
{
    public SpriteRenderer objectToShow;
    public SpriteRenderer objectToHide;
    public KeyCode activationKey = KeyCode.E;

    private bool playerInRange = false;

    void Start()
    {
        // 从GameManager获取保存的状态
        string doorID = $"{SceneManager.GetActiveScene().name}_{this.gameObject.name}";
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
        string doorID = $"{SceneManager.GetActiveScene().name}_{this.gameObject.name}";
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