using UnityEngine;

public class MirrorMergeGate : MonoBehaviour
{
    [Tooltip("���ǵ����ã��ڳ�������ק��")]
    public GameObject mainPlayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ֻ����ӵ�� MirrorTag �Ķ���
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

                // ���ŷ���ͬ�������巭ת��
                Vector3 newScale = mainPlayer.transform.localScale;
                newScale.y = mirrorTransform.localScale.y;
                mainPlayer.transform.localScale = newScale;

                // SpriteRenderer �Ӿ�����ͬ����ͷ���£�
                SpriteRenderer mainSprite = mainPlayer.GetComponentInChildren<SpriteRenderer>();
                SpriteRenderer mirrorSprite = mirrorTransform.GetComponentInChildren<SpriteRenderer>();
                if (mainSprite != null && mirrorSprite != null)
                {
                    mainSprite.flipY = mirrorSprite.flipY;
                }
            }

            // ���پ�����
            Destroy(other.gameObject);
        }
    }
}
