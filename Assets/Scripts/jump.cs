using UnityEngine;

[RequireComponent(typeof(AudioSource))] // 自动添加必要的音频组件
public class PlayerSoundController : MonoBehaviour
{
    [Header("声音设置")]
    [Tooltip("将需要播放的音频文件拖拽到这里")]
    public AudioClip audioClip;  // 要播放的声音文件

    private AudioSource audioSource;

    private void Awake()
    {
        // 获取音频源组件
        audioSource = GetComponent<AudioSource>();

        // 如果音频源未配置，进行安全设置
        if (audioSource == null)
        {
            Debug.LogWarning("未找到AudioSource组件，已自动添加");
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 初始化音频源设置
        audioSource.playOnAwake = false;
        audioSource.clip = audioClip;
    }

    private void Update()
    {
        // 检测空格键按下（支持短按触发）
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlaySound();
        }
    }

    private void PlaySound()
    {
        // 安全校验
        if (audioClip == null)
        {
            Debug.LogWarning("未分配音频文件！");
            return;
        }

        // 播放声音（允许多次重叠播放）
        audioSource.PlayOneShot(audioClip);

        // 如果希望禁止声音重叠，改用以下方式：
        // if(!audioSource.isPlaying)
        // {
        //     audioSource.Play();
        // }
    }
}