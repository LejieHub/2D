using UnityEngine;

public class DoorStateController : MonoBehaviour
{
    // �ؿ�ID����Ψһ
    public string doorID;
    public SpriteRenderer closedDoorSprite;
    public SpriteRenderer openedDoorSprite;

    [Header("Interaction Settings")]
    public KeyCode activationKey = KeyCode.E; // �����İ���
    public bool showInteractionHint = true; // �Ƿ���ʾ������ʾ
    public GameObject interactionHint; // ������ʾUI����

    private bool playerInRange = false;
    private bool isInitialized = false;

    void Start()
    {
        InitializeDoorState();

        // ���û�����ʾ
        if (interactionHint != null)
        {
            interactionHint.SetActive(false);
        }
    }

    void InitializeDoorState()
    {
        if (string.IsNullOrEmpty(doorID) || isInitialized)
            return;

        // ��PlayerPrefs���ر����״̬
        bool isOpen = PlayerPrefs.GetInt(doorID, 0) == 1;
        UpdateDoorVisuals(isOpen);

        isInitialized = true;
    }

    void Update()
    {
        // �������Ƿ��ڷ�Χ���Ұ���ָ������
        if (playerInRange && Input.GetKeyDown(activationKey))
        {
            // ȷ���Ż�δ��
            if (!PlayerPrefs.HasKey(doorID) || PlayerPrefs.GetInt(doorID) == 0)
            {
                MarkDoorAsOpened();
            }
        }
    }

    // �������ɹؿ�ʱ���ô˷���
    public void MarkDoorAsOpened()
    {
        PlayerPrefs.SetInt(doorID, 1);
        PlayerPrefs.Save();
        UpdateDoorVisuals(true);

        Debug.Log($"Door opened: {doorID}");

        // �رջ�����ʾ
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

            // ����Ż�û�򿪣���ʾ������ʾ
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

            // ���ػ�����ʾ
            if (interactionHint != null)
            {
                interactionHint.SetActive(false);
            }
        }
    }

    // ȫ������������״̬���ڿ�ʼ����Ϸʱ���ã�
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

    // �ڱ༭���в����õ����ð�ť
    [ContextMenu("Reset This Door State")]
    void EditorResetDoorState()
    {
        PlayerPrefs.DeleteKey(doorID);
        UpdateDoorVisuals(false);
        Debug.Log($"Reset state for door: {doorID}");
    }
}