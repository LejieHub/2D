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

    [SerializeField] private float fadeDuration = 0.5f; // ����ʱ��

    void OnTriggerEnter2D(Collider2D other)
    {
        // ֻ�������ǽ���
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

        // ��Ҵ��쳣����ص��������磬�����������Ҳ࣬����
        if (
            hasSpawned &&
            mirrorInstance != null &&
            playerX < gateX && // ����ѻص����
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

            // ��Ҵ������������쳣���紩�ţ�����ң�
            if (playerX > gateX)
            {
                // �����ظ�����
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