using UnityEngine;

public class MirrorKillZone : MonoBehaviour
{
    // ����԰����������ͨ�� inspector ��������ý���
    public MirrorController mirrorToDestroy;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // ���崥��
        {
            if (mirrorToDestroy != null)
            {
                mirrorToDestroy.StartDestruction();
                Debug.Log("Player triggered destruction of mirror.");
            }
        }
    }
}
