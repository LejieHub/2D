using UnityEngine;

[RequireComponent(typeof(AudioSource))] // �Զ���ӱ�Ҫ����Ƶ���
public class PlayerSoundController : MonoBehaviour
{
    [Header("��������")]
    [Tooltip("����Ҫ���ŵ���Ƶ�ļ���ק������")]
    public AudioClip audioClip;  // Ҫ���ŵ������ļ�

    private AudioSource audioSource;

    private void Awake()
    {
        // ��ȡ��ƵԴ���
        audioSource = GetComponent<AudioSource>();

        // �����ƵԴδ���ã����а�ȫ����
        if (audioSource == null)
        {
            Debug.LogWarning("δ�ҵ�AudioSource��������Զ����");
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ��ʼ����ƵԴ����
        audioSource.playOnAwake = false;
        audioSource.clip = audioClip;
    }

    private void Update()
    {
        // ���ո�����£�֧�ֶ̰�������
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlaySound();
        }
    }

    private void PlaySound()
    {
        // ��ȫУ��
        if (audioClip == null)
        {
            Debug.LogWarning("δ������Ƶ�ļ���");
            return;
        }

        // �����������������ص����ţ�
        audioSource.PlayOneShot(audioClip);

        // ���ϣ����ֹ�����ص����������·�ʽ��
        // if(!audioSource.isPlaying)
        // {
        //     audioSource.Play();
        // }
    }
}