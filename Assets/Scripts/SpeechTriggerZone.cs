using UnityEngine;

public class SpeechTriggerZone : MonoBehaviour
{
    [TextArea]
    [SerializeField] private string[] lines; // 多条候选文本
    [SerializeField] private bool triggerOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered && triggerOnce) return;
        if (!other.CompareTag("Player")) return;

        SpeechBubbleController bubble = other.GetComponentInChildren<SpeechBubbleController>();
        if (bubble != null && lines.Length > 0)
        {
            int index = Random.Range(0, lines.Length);
            bubble.ShowText(lines[index]);
            hasTriggered = true;
        }
    }
}
