using UnityEngine;

[RequireComponent(typeof(AudioSource))] // 自动添加音频组件
public class CollisionSoundEffect : MonoBehaviour
{
    [Header("声音设置")]
    [Tooltip("拖入需要播放的音频文件")]
    public AudioClip soundEffect;

    [Header("触发设置")]
    [Tooltip("勾选后允许重复触发音效")]
    public bool allowRetrigger = true;

    [Tooltip("勾选后触发一次自动禁用碰撞器")]
    public bool disableColliderAfterTrigger;

    private AudioSource audioSource;
    private Collider2D triggerCollider;
    private bool hasTriggered;

    private void Awake()
    {
        // 获取组件引用
        audioSource = GetComponent<AudioSource>();
        triggerCollider = GetComponent<Collider2D>();

        // 初始化音频源设置
        InitializeAudioSource();

        // 自动配置碰撞器为触发器
        ConfigureCollider();
    }

    void InitializeAudioSource()
    {
        // 修复初始化播放问题的关键设置
        audioSource.playOnAwake = false;
        audioSource.clip = null;       // 清空默认音频
        audioSource.Stop();            // 确保停止所有播放
        audioSource.spatialBlend = 0;  // 2D音效
    }

    void ConfigureCollider()
    {
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
        else
        {
            Debug.LogError($"缺少Collider2D组件！对象：{gameObject.name}");
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
        // 检查触发条件
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
            // 使用PlayOneShot避免影响主音频源设置
            audioSource.PlayOneShot(soundEffect);
        }
        else
        {
            Debug.LogWarning("未分配音效文件！");
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

    // 编辑器可视化提示
    private void OnDrawGizmos()
    {
        if (triggerCollider != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawCube(transform.position, triggerCollider.bounds.size);
        }
    }
}