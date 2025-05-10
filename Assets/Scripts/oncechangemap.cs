using UnityEngine;
using UnityEngine.Tilemaps; // 添加Tilemap命名空间

public class TriggerHandler : MonoBehaviour
{
    [Header("激活相关设置")]
    public GameObject[] objectsToActivate;    // 需要激活的物体
    public bool enableCollidersOnActivate = true; // 同时启用碰撞体
    public TilemapRenderer[] tilemapsToEnable; // 需要启用的Tilemap渲染器

    [Header("隐藏相关设置")]
    public GameObject[] objectsToDeactivate;  // 需要隐藏的物体
    public bool disableCollidersOnDeactivate = true; // 同时禁用碰撞体
    public TilemapRenderer[] tilemapsToDisable; // 需要禁用的Tilemap渲染器

    [Header("触发器设置")]
    public bool singleUse = true;            // 是否单次触发

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 激活指定物体
            foreach (GameObject obj in objectsToActivate)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                    if (enableCollidersOnActivate)
                    {
                        Collider2D col = obj.GetComponent<Collider2D>();
                        if (col != null) col.enabled = true;
                    }
                }
            }

            // 启用Tilemap渲染器
            foreach (TilemapRenderer renderer in tilemapsToEnable)
            {
                if (renderer != null)
                {
                    renderer.enabled = true;
                }
            }

            // 隐藏指定物体
            foreach (GameObject obj in objectsToDeactivate)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                    if (disableCollidersOnDeactivate)
                    {
                        Collider2D col = obj.GetComponent<Collider2D>();
                        if (col != null) col.enabled = false;
                    }
                }
            }

            // 禁用Tilemap渲染器
            foreach (TilemapRenderer renderer in tilemapsToDisable)
            {
                if (renderer != null)
                {
                    renderer.enabled = false;
                }
            }

            // 单次触发后禁用自身
            if (singleUse)
            {
                GetComponent<Collider2D>().enabled = false;
                enabled = false;
            }
        }
    }
}