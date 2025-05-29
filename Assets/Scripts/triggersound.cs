using UnityEngine;

[RequireComponent(typeof(AudioSource))] // �Զ������Ƶ���
public class CollisionSoundEffect : MonoBehaviour
{
    [Header("��������")]
    [Tooltip("������Ҫ���ŵ���Ƶ�ļ�")]
    public AudioClip soundEffect;

    [Header("��������")]
    [Tooltip("��ѡ�������ظ�������Ч")]
    public bool allowRetrigger = true;

    [Tooltip("��ѡ�󴥷�һ���Զ�������ײ��")]
    public bool disableColliderAfterTrigger;

    private AudioSource audioSource;
    private Collider2D triggerCollider;
    private bool hasTriggered;

    private void Awake()
    {
        // ��ȡ�������
        audioSource = GetComponent<AudioSource>();
        triggerCollider = GetComponent<Collider2D>();

        // ��ʼ����ƵԴ����
        InitializeAudioSource();

        // �Զ�������ײ��Ϊ������
        ConfigureCollider();
    }

    void InitializeAudioSource()
    {
        // �޸���ʼ����������Ĺؼ�����
        audioSource.playOnAwake = false;
        audioSource.clip = null;       // ���Ĭ����Ƶ
        audioSource.Stop();            // ȷ��ֹͣ���в���
        audioSource.spatialBlend = 0;  // 2D��Ч
    }

    void ConfigureCollider()
    {
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
        else
        {
            Debug.LogError($"ȱ��Collider2D���������{gameObject.name}");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HandleSoundTrigger();
        }
    }

    void HandleSoundTrigger()
    {
        // ��鴥������
        if (ShouldPlaySound())
        {
            PlaySoundEffect();
            UpdateTriggerState();
            HandleColliderDisable();
        }
    }

    bool ShouldPlaySound()
    {
        return allowRetrigger || !hasTriggered;
    }

    void PlaySoundEffect()
    {
        if (soundEffect != null)
        {
            // ʹ��PlayOneShot����Ӱ������ƵԴ����
            audioSource.PlayOneShot(soundEffect);
        }
        else
        {
            Debug.LogWarning("δ������Ч�ļ���");
        }
    }

    void UpdateTriggerState()
    {
        if (!allowRetrigger)
        {
            hasTriggered = true;
        }
    }

    void HandleColliderDisable()
    {
        if (disableColliderAfterTrigger && triggerCollider != null)
        {
            triggerCollider.enabled = false;
        }
    }

    // �༭�����ӻ���ʾ
    private void OnDrawGizmos()
    {
        if (triggerCollider != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawCube(transform.position, triggerCollider.bounds.size);
        }
    }
}