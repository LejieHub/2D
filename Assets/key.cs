// KeyCollectible.cs
using UnityEngine;

public class KeyCollectible : MonoBehaviour
{
    [Header("钥匙属性")]
    public string keyID;
    public float rotateSpeed = 90f; // 钥匙旋转动画速度

    [Header("收集效果")]
    public ParticleSystem collectEffect;
    public AudioClip collectSound;

    private void Update()
    {
        // 添加旋转动画
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CollectKey();
        }
    }

    void CollectKey()
    {
        KeyManager.Instance.AddKey(keyID);

        // 播放收集效果
        if (collectEffect)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // 播放音效
        if (collectSound)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        Destroy(gameObject);
    }
}