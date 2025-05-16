using UnityEngine;

public class MirrorGateTrigger : MonoBehaviour
{
    public GameObject mirrorPlayerPrefab;
    public Transform mirrorSpawnPointLeft;
    public Transform gateCenter;

    private GameObject mirrorInstance;
    private bool playerInside = false;
    private Transform playerRef;
    private bool hasSpawned = false;
    private float playerEntryX;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        playerRef = other.transform;
        playerEntryX = playerRef.position.x;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        playerRef = null;
    }

    void Update()
    {
        if (hasSpawned && mirrorInstance == null)
        {
            hasSpawned = false;
        }

        if (playerInside && !hasSpawned && playerRef != null)
        {
            float currentX = playerRef.position.x;
            float gateX = gateCenter.position.x;

            // 玩家从左往右穿越门中心
            if (playerEntryX < gateX && currentX > gateX)
            {
                if (mirrorInstance != null) return;

                mirrorInstance = Instantiate(mirrorPlayerPrefab, mirrorSpawnPointLeft.position, Quaternion.identity);
                MirrorController mirrorCtrl = mirrorInstance.GetComponent<MirrorController>();
                if (mirrorCtrl != null)
                {
                    mirrorCtrl.Init(playerRef, gateX);

                    FindObjectOfType<DynamicCameraZoom>()?.SetMirror(mirrorInstance.transform);

                    MirrorKillZone killZone = FindObjectOfType<MirrorKillZone>();

                    if (killZone != null)
                    {
                        killZone.mirrorToDestroy = mirrorCtrl;
                    }
                }

                hasSpawned = true;
                Debug.Log("Mirror spawned.");
            }
        }
    }
}
