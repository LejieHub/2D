using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRewind : MonoBehaviour
{
    [Header("Player Controls")]
    public GameObject player;
    public MonoBehaviour movementScript;  // ��Ҫ���õ��ƶ��ű�

    [Header("Rewind Settings")]
    [SerializeField] private float recordInterval = 0.1f;
    [SerializeField] private float rewindSpeed = 0.05f;

    private bool isRewinding = false;
    private List<Vector3> positionHistory = new List<Vector3>();
    private List<Quaternion> rotationHistory = new List<Quaternion>();
    private Coroutine recordingCoroutine;
    private Rigidbody2D rb; // ���� Rigidbody2D ����

    void Start()
    {
        // ��ȡ��ҵ� Rigidbody2D ���
        if (player != null)
        {
            rb = player.GetComponent<Rigidbody2D>();
        }
        StartRecording();
    }

    void StartRecording()
    {
        if (recordingCoroutine != null) StopCoroutine(recordingCoroutine);
        recordingCoroutine = StartCoroutine(RecordPlayerPosition());
    }

    IEnumerator RecordPlayerPosition()
    {
        while (true)
        {
            if (player != null && !isRewinding)
            {
                positionHistory.Add(player.transform.position);
                rotationHistory.Add(player.transform.rotation);
            }
            yield return new WaitForSeconds(recordInterval);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isRewinding)
        {
            StartRewind();
        }
    }

    public void StartRewind()
    {
        if (isRewinding) return;

        // ������ҿ���
        if (movementScript != null)
            movementScript.enabled = false;

        // ��������״̬
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true; // ʹ����������ʱʧЧ
        }

        StartCoroutine(Rewind());
    }

    IEnumerator Rewind()
    {
        isRewinding = true;
        StopCoroutine(recordingCoroutine);

        List<Vector3> rewindPositions = new List<Vector3>(positionHistory);
        List<Quaternion> rewindRotations = new List<Quaternion>(rotationHistory);

        for (int i = rewindPositions.Count - 1; i >= 0; i--)
        {
            if (player == null) yield break;

            // ʹ�� Rigidbody �ƶ���ȷ����ײ���
            if (rb != null)
            {
                rb.MovePosition(rewindPositions[i]);
                rb.MoveRotation(rewindRotations[i]);
            }
            else
            {
                player.transform.position = rewindPositions[i];
                player.transform.rotation = rewindRotations[i];
            }

            yield return new WaitForSeconds(rewindSpeed);
        }

        positionHistory.Clear();
        rotationHistory.Clear();

        // �ָ��������
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector2.zero; // ȷ���ٶȹ���
        }

        // �ָ���ҿ���
        if (movementScript != null)
            movementScript.enabled = true;

        isRewinding = false;
        StartRecording();
    }

    public void ClearOldRecords(int keepSeconds = 5)
    {
        int maxRecords = Mathf.FloorToInt(keepSeconds / recordInterval);
        while (positionHistory.Count > maxRecords)
        {
            positionHistory.RemoveAt(0);
            rotationHistory.RemoveAt(0);
        }
    }
}