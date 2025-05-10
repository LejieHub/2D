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

    [SerializeField] private float fadeDuration = 0.5f; // 淡出时间

    void OnTriggerEnter2D(Collider2D other)
    {
        // 只允许主角进入
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        playerRef = other.transform;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        float playerX = other.transform.position.x;
        float gateX = gateCenter.position.x;

        playerInside = false;
        playerRef = null;

        // 玩家从异常世界回到正常世界，镜像体仍在右侧，销毁
        if (
            hasSpawned &&
            mirrorInstance != null &&
            playerX < gateX && // 玩家已回到左侧
            mirrorInstance.transform.position.x > gateX + 0.2f
        )
        {
            StartCoroutine(FadeAndDestroy(mirrorInstance));
            mirrorInstance = null;
            hasSpawned = false;
            Debug.Log("Mirror fading and destroyed when player returned.");
        }
    }

    void Update()
    {
        if (playerInside && !hasSpawned && playerRef != null)
        {
            float playerX = playerRef.position.x;
            float gateX = gateCenter.position.x;

            // 玩家从正常世界往异常世界穿门（左→右）
            if (playerX > gateX)
            {
                // 避免重复生成
                if (mirrorInstance != null) return;

                mirrorInstance = Instantiate(mirrorPlayerPrefab, mirrorSpawnPointLeft.position, Quaternion.identity);
                MirrorController mirrorCtrl = mirrorInstance.GetComponent<MirrorController>();
                if (mirrorCtrl != null)
                {
                    mirrorCtrl.Init(playerRef, gateCenter.position.x);
                }
                FindObjectOfType<DynamicCameraZoom>()?.SetMirror(mirrorInstance.transform);

                hasSpawned = true;
                Debug.Log("Mirror spawned.");
            }
        }
    }

    System.Collections.IEnumerator FadeAndDestroy(GameObject obj)
    {
        SpriteRenderer sr = obj.GetComponentInChildren<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogWarning("No SpriteRenderer found on mirror object.");
            Destroy(obj);
            yield break;
        }

        Color originalColor = sr.color;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        Destroy(obj);
    }
}