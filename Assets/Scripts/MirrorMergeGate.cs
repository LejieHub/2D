using UnityEngine;

public class MirrorMergeGate : MonoBehaviour
{
    [Tooltip("主角的引用（在场景中拖拽）")]
    public GameObject mainPlayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
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

                // 同步跳跃力
                JumpForce(mirrorTransform.gameObject, mainPlayer);

                // 缩放方向同步
                Vector3 newScale = mainPlayer.transform.localScale;
                newScale.y = mirrorTransform.localScale.y;
                mainPlayer.transform.localScale = newScale;

                //sprite方向同步
                SpriteRenderer mainSprite = mainPlayer.GetComponentInChildren<SpriteRenderer>();
                SpriteRenderer mirrorSprite = mirrorTransform.GetComponentInChildren<SpriteRenderer>();
                if (mainSprite != null && mirrorSprite != null)
                {
                    mainSprite.flipY = mirrorSprite.flipY;
                }
            }
            // 清除主角在所有重力门中的缓存
            GravityReverseTrigger[] gravityTriggers = FindObjectsOfType<GravityReverseTrigger>();
            foreach (var trigger in gravityTriggers)
            {
                trigger.ForceClearGravityCache(mainPlayer);
            }

            Destroy(other.gameObject);
        }
    }

    // 同步跳跃
    void JumpForce(GameObject source, GameObject target)
    {
        MonoBehaviour[] sourceScripts = source.GetComponents<MonoBehaviour>();
        //遍历克隆体身上的组件，找到jumpforce相关的参数，同步给mainplayer
        foreach (MonoBehaviour sourceScript in sourceScripts)
        {
            System.Type type = sourceScript.GetType();
            System.Reflection.FieldInfo field = type.GetField("jumpForce",
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (field != null && field.FieldType == typeof(float))
            {
                //获取克隆体jumpforce直
                float sourceValue = (float)field.GetValue(sourceScript);
                //查找mainplayer身上的组件
                MonoBehaviour targetScript = target.GetComponent(type) as MonoBehaviour;
                if (targetScript != null)
                {
                    //把值写到mainplayer身上
                    field.SetValue(targetScript, sourceValue);
                }
            }
        }
    }
}
