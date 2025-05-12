// KeyCollectible.cs
using UnityEngine;

public class KeyCollectible : MonoBehaviour
{
    [Header("Կ������")]
    public string keyID;
    public float rotateSpeed = 90f; // Կ����ת�����ٶ�

    [Header("�ռ�Ч��")]
    public ParticleSystem collectEffect;
    public AudioClip collectSound;

    private void Update()
    {
        // �����ת����
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

        // �����ռ�Ч��
        if (collectEffect)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // ������Ч
        if (collectSound)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        Destroy(gameObject);
    }
}