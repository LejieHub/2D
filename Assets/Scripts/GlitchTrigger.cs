using UnityEngine;

public class GlitchTrigger : MonoBehaviour
{
    public AnalogGlitchFeature analogGlitchFeature;
    public DigitalGlitchFeature digitalGlitchFeature;

    public float effectDuration = 1.5f;

    [Header("Analog Glitch Parameters")]
    public float scanLineJitter = 0.1f;
    public float verticalJump = 0.1f;
    public float horizontalShake = 0.1f;
    public float colorDrift = 0.1f;

    [Header("Digital Glitch Parameter")]
    public float digitalIntensity = 0.1f;

    private float timer = 0f;
    private bool isTriggered = false;

    void Update()
    {
        if (isTriggered)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                if (analogGlitchFeature != null)
                {
                    analogGlitchFeature.settings.scanLineJitter = 0f;
                    analogGlitchFeature.settings.verticalJump = 0f;
                    analogGlitchFeature.settings.horizontalShake = 0f;
                    analogGlitchFeature.settings.colorDrift = 0f;
                }

                if (digitalGlitchFeature != null)
                {
                    digitalGlitchFeature.settings.intensity = 0f;
                }

                isTriggered = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("door"))
        {
            if (analogGlitchFeature != null)
            {
                analogGlitchFeature.settings.scanLineJitter = scanLineJitter;
                analogGlitchFeature.settings.verticalJump = verticalJump;
                analogGlitchFeature.settings.horizontalShake = horizontalShake;
                analogGlitchFeature.settings.colorDrift = colorDrift;
            }

            if (digitalGlitchFeature != null)
            {
                digitalGlitchFeature.settings.intensity = digitalIntensity;
            }

            timer = effectDuration;
            isTriggered = true;
        }
    }
}
