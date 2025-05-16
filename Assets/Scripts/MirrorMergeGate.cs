using UnityEngine;

public class MirrorMergeGate : MonoBehaviour
{
    [Tooltip("���ǵ����ã��ڳ�������ק��")]
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
                // λ��ͬ��
                mainPlayer.transform.position = mirrorTransform.position;

                // ����ͬ��
                mainRb.gravityScale = mirrorRb.gravityScale;

                // ͬ����Ծ��
                JumpForce(mirrorTransform.gameObject, mainPlayer);

                // ���ŷ���ͬ��
                Vector3 newScale = mainPlayer.transform.localScale;
                newScale.y = mirrorTransform.localScale.y;
                mainPlayer.transform.localScale = newScale;

                //sprite����ͬ��
                SpriteRenderer mainSprite = mainPlayer.GetComponentInChildren<SpriteRenderer>();
                SpriteRenderer mirrorSprite = mirrorTransform.GetComponentInChildren<SpriteRenderer>();
                if (mainSprite != null && mirrorSprite != null)
                {
                    mainSprite.flipY = mirrorSprite.flipY;
                }
            }
            // ��������������������еĻ���
            GravityReverseTrigger[] gravityTriggers = FindObjectsOfType<GravityReverseTrigger>();
            foreach (var trigger in gravityTriggers)
            {
                trigger.ForceClearGravityCache(mainPlayer);
            }

            Destroy(other.gameObject);
        }
    }

    // ͬ����Ծ
    void JumpForce(GameObject source, GameObject target)
    {
        MonoBehaviour[] sourceScripts = source.GetComponents<MonoBehaviour>();
        //������¡�����ϵ�������ҵ�jumpforce��صĲ�����ͬ����mainplayer
        foreach (MonoBehaviour sourceScript in sourceScripts)
        {
            System.Type type = sourceScript.GetType();
            System.Reflection.FieldInfo field = type.GetField("jumpForce",
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (field != null && field.FieldType == typeof(float))
            {
                //��ȡ��¡��jumpforceֱ
                float sourceValue = (float)field.GetValue(sourceScript);
                //����mainplayer���ϵ����
                MonoBehaviour targetScript = target.GetComponent(type) as MonoBehaviour;
                if (targetScript != null)
                {
                    //��ֵд��mainplayer����
                    field.SetValue(targetScript, sourceValue);
                }
            }
        }
    }
}
