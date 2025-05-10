using UnityEngine;
using UnityEngine.Tilemaps;

public class DirectionalMapTrigger : MonoBehaviour
{
    [Header("Left Trigger Settings")]
    public GameObject[] leftObjectsToActivate;
    public TilemapRenderer[] leftTilemapsToEnable;
    public GameObject[] leftObjectsToDeactivate;
    public TilemapRenderer[] leftTilemapsToDisable;

    [Header("Right Trigger Settings")]
    public GameObject[] rightObjectsToActivate;
    public TilemapRenderer[] rightTilemapsToEnable;
    public GameObject[] rightObjectsToDeactivate;
    public TilemapRenderer[] rightTilemapsToDisable;

    private Collider2D triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Vector2 triggerCenter = triggerCollider.bounds.center;
            Vector2 playerPosition = other.transform.position;

            // 计算进入方向（左/右）
            bool fromLeft = playerPosition.x < triggerCenter.x;

            if (fromLeft)
            {
                ToggleGroup(leftObjectsToActivate, leftTilemapsToEnable, true);
                ToggleGroup(leftObjectsToDeactivate, leftTilemapsToDisable, false);
            }
            else
            {
                ToggleGroup(rightObjectsToActivate, rightTilemapsToEnable, true);
                ToggleGroup(rightObjectsToDeactivate, rightTilemapsToDisable, false);
            }
        }
    }

    private void ToggleGroup(GameObject[] objects, TilemapRenderer[] tilemaps, bool state)
    {
        // 处理GameObject
        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                // 设置物体激活状态
                obj.SetActive(state);

                // 控制所有SpriteRenderer组件
                SpriteRenderer[] spriteRenderers = obj.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer sr in spriteRenderers)
                {
                    sr.enabled = state;
                }

                // 控制碰撞体
                ToggleColliders(obj, state);
            }
        }

        // 处理Tilemap
        foreach (TilemapRenderer renderer in tilemaps)
        {
            if (renderer != null)
            {
                renderer.enabled = state;
                ToggleTilemapCollider(renderer, state);
            }
        }
    }

    private void ToggleColliders(GameObject obj, bool state)
    {
        Collider2D[] colliders = obj.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = state;
        }
    }

    private void ToggleTilemapCollider(TilemapRenderer renderer, bool state)
    {
        TilemapCollider2D collider = renderer.GetComponent<TilemapCollider2D>();
        if (collider != null) collider.enabled = state;
    }
}