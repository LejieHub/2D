using UnityEngine;

public class DoorStateController : MonoBehaviour
{
    // 关卡ID必须唯一
    public string doorID;
    public SpriteRenderer closedDoorSprite;
    public SpriteRenderer openedDoorSprite;

    [Header("Interaction Settings")]
    public KeyCode activationKey = KeyCode.E; // 触发的按键
    public bool showInteractionHint = true; // 是否显示互动提示
    public GameObject interactionHint; // 互动提示UI对象

    private bool playerInRange = false;
    private bool isInitialized = false;

    void Start()
    {
        InitializeDoorState();

        // 设置互动提示
        if (interactionHint != null)
        {
            interactionHint.SetActive(false);
        }
    }

    void InitializeDoorState()
    {
        if (string.IsNullOrEmpty(doorID) || isInitialized)
            return;

        // 从PlayerPrefs加载保存的状态
        bool isOpen = PlayerPrefs.GetInt(doorID, 0) == 1;
        UpdateDoorVisuals(isOpen);

        isInitialized = true;
    }

    void Update()
    {
        // 检查玩家是否在范围内且按下指定按键
        if (playerInRange && Input.GetKeyDown(activationKey))
        {
            // 确保门还未打开
            if (!PlayerPrefs.HasKey(doorID) || PlayerPrefs.GetInt(doorID) == 0)
            {
                MarkDoorAsOpened();
            }
        }
    }

    // 当玩家完成关卡时调用此方法
    public void MarkDoorAsOpened()
    {
        PlayerPrefs.SetInt(doorID, 1);
        PlayerPrefs.Save();
        UpdateDoorVisuals(true);

        Debug.Log($"Door opened: {doorID}");

        // 关闭互动提示
        if (interactionHint != null)
        {
            interactionHint.SetActive(false);
        }
    }

    void UpdateDoorVisuals(bool isOpen)
    {
        if (closedDoorSprite != null)
            closedDoorSprite.enabled = !isOpen;

        if (openedDoorSprite != null)
            openedDoorSprite.enabled = isOpen;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // 如果门还没打开，显示互动提示
            if (interactionHint != null && showInteractionHint)
            {
                if (!PlayerPrefs.HasKey(doorID) || PlayerPrefs.GetInt(doorID) == 0)
                {
                    interactionHint.SetActive(true);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            // 隐藏互动提示
            if (interactionHint != null)
            {
                interactionHint.SetActive(false);
            }
        }
    }

    // 全局重置所有门状态（在开始新游戏时调用）
    public static void ResetAllDoorStates()
    {
        DoorStateController[] allDoors = FindObjectsOfType<DoorStateController>();

        foreach (DoorStateController door in allDoors)
        {
            PlayerPrefs.DeleteKey(door.doorID);
            door.UpdateDoorVisuals(false);
        }

        PlayerPrefs.Save();
    }

    // 在编辑器中测试用的重置按钮
    [ContextMenu("Reset This Door State")]
    void EditorResetDoorState()
    {
        PlayerPrefs.DeleteKey(doorID);
        UpdateDoorVisuals(false);
        Debug.Log($"Reset state for door: {doorID}");
    }
}