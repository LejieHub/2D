using UnityEngine;

public class MirrorMergeGate : MonoBehaviour
{
    [Tooltip("主角的引用（在场景中拖拽）")]
    public GameObject mainPlayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 只处理拥有 MirrorTag 的对象
        if (other.GetComponent<MirrorTag>() != null)
        {
            Transform mirrorTransform = other.transform;
            Rigidbody2D mirrorRb = mirrorTransform.GetComponent<Rigidbody2D>();
            Rigidbody2D mainRb = mainPlayer.GetComponent<Rigidbody2D>();

            if (mirrorRb != null && mainRb != null)
            {
                // 位置同步
                mainPlayer.transform.position = mirrorTransform.position;

                // 重力同步
                mainRb.gravityScale = mirrorRb.gravityScale;

                // 缩放方向同步（整体翻转）
                Vector3 newScale = mainPlayer.transform.localScale;
                newScale.y = mirrorTransform.localScale.y;
                mainPlayer.transform.localScale = newScale;

                // SpriteRenderer 视觉朝向同步（头朝下）
                SpriteRenderer mainSprite = mainPlayer.GetComponentInChildren<SpriteRenderer>();
                SpriteRenderer mirrorSprite = mirrorTransform.GetComponentInChildren<SpriteRenderer>();
                if (mainSprite != null && mirrorSprite != null)
                {
                    mainSprite.flipY = mirrorSprite.flipY;
                }
            }

            // 销毁镜像体
            Destroy(other.gameObject);
        }
    }
}
