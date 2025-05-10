using UnityEngine;
using UnityEngine.Tilemaps; // ���Tilemap�����ռ�

public class TriggerHandler : MonoBehaviour
{
    [Header("�����������")]
    public GameObject[] objectsToActivate;    // ��Ҫ���������
    public bool enableCollidersOnActivate = true; // ͬʱ������ײ��
    public TilemapRenderer[] tilemapsToEnable; // ��Ҫ���õ�Tilemap��Ⱦ��

    [Header("�����������")]
    public GameObject[] objectsToDeactivate;  // ��Ҫ���ص�����
    public bool disableCollidersOnDeactivate = true; // ͬʱ������ײ��
    public TilemapRenderer[] tilemapsToDisable; // ��Ҫ���õ�Tilemap��Ⱦ��

    [Header("����������")]
    public bool singleUse = true;            // �Ƿ񵥴δ���

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ����ָ������
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

            // ����Tilemap��Ⱦ��
            foreach (TilemapRenderer renderer in tilemapsToEnable)
            {
                if (renderer != null)
                {
                    renderer.enabled = true;
                }
            }

            // ����ָ������
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

            // ����Tilemap��Ⱦ��
            foreach (TilemapRenderer renderer in tilemapsToDisable)
            {
                if (renderer != null)
                {
                    renderer.enabled = false;
                }
            }

            // ���δ������������
            if (singleUse)
            {
                GetComponent<Collider2D>().enabled = false;
                enabled = false;
            }
        }
    }
}