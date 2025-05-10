using UnityEngine;

public class GateButton : MonoBehaviour
{
    public SlidingGate slidingGate;

    private int pressCount = 0; // ֧�ֶ��˻��������վ��

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        pressCount++;
        if (pressCount == 1)
        {
            slidingGate.OpenGate();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        pressCount--;
        if (pressCount <= 0)
        {
            slidingGate.CloseGate();
        }
    }
}
