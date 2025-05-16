using UnityEngine;

[RequireComponent(typeof(GateButton))]
public class MirrorButtonExtension : MonoBehaviour
{
    public Color activeColor = Color.red;

    private GateButton gateButton;
    private SlidingGate gate;
    private SpriteRenderer sr;

    private bool mirrorActivated = false;

    void Awake()
    {
        gateButton = GetComponent<GateButton>();
        gate = gateButton.slidingGate;
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<MirrorTag>() != null)
        {
            // ֻ�败��һ��
            if (!mirrorActivated)
            {
                mirrorActivated = true;
                sr.color = activeColor;
                gate.OpenGate();
            }
        }
    }

    void Update()
    {
        if (mirrorActivated)
        {
            // ���������ſ���
            gate.OpenGate();

            // ��鳡�����Ƿ��� MirrorPlayer
            MirrorTag[] mirrors = FindObjectsOfType<MirrorTag>();
            if (mirrors.Length == 0)
            {
                mirrorActivated = false;
                sr.color = Color.white;
                gate.CloseGate();
            }
        }
    }
}
