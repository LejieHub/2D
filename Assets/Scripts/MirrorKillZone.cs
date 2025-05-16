using UnityEngine;

public class MirrorKillZone : MonoBehaviour
{
    // 你可以把这个镜像体通过 inspector 或代码设置进来
    public MirrorController mirrorToDestroy;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 本体触发
        {
            if (mirrorToDestroy != null)
            {
                mirrorToDestroy.StartDestruction();
                Debug.Log("Player triggered destruction of mirror.");
            }
        }
    }
}
