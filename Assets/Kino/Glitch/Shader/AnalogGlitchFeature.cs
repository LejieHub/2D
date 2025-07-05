using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AnalogGlitchFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class AnalogGlitchSettings
    {
        public Material glitchMaterial;

        [Range(0, 1)] public float scanLineJitter = 0.1f;
        [Range(0, 1)] public float verticalJump = 0.1f;
        [Range(0, 1)] public float horizontalShake = 0.1f;
        [Range(0, 1)] public float colorDrift = 0.1f;

        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public AnalogGlitchSettings settings = new AnalogGlitchSettings();

    AnalogGlitchPass glitchPass;

    public override void Create()
    {
        if (settings.glitchMaterial == null)
        {
            Debug.LogError("AnalogGlitchFeature: Please assign a glitch material.");
            return;
        }

        glitchPass = new AnalogGlitchPass(settings);
        glitchPass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (glitchPass != null)
        {
            glitchPass.ConfigureInput(ScriptableRenderPassInput.Color); // ?? ªÒ»° camera color buffer
            renderer.EnqueuePass(glitchPass);
        }
    }

    class AnalogGlitchPass : ScriptableRenderPass
    {
        AnalogGlitchSettings settings;
        RenderTargetHandle tempTexture;
        float verticalJumpTime;

        public AnalogGlitchPass(AnalogGlitchSettings settings)
        {
            this.settings = settings;
            tempTexture.Init("_TemporaryColorTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (settings.glitchMaterial == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("AnalogGlitchPass");

            var source = renderingData.cameraData.renderer.cameraColorTargetHandle;
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(tempTexture.id, descriptor, FilterMode.Bilinear);

            // Update glitch effect parameters
            verticalJumpTime += Time.deltaTime * settings.verticalJump * 11.3f;

            float sl_thresh = Mathf.Clamp01(1.0f - settings.scanLineJitter * 1.2f);
            float sl_disp = 0.002f + Mathf.Pow(settings.scanLineJitter, 3) * 0.05f;
            settings.glitchMaterial.SetVector("_ScanLineJitter", new Vector2(sl_disp, sl_thresh));
            settings.glitchMaterial.SetVector("_VerticalJump", new Vector2(settings.verticalJump, verticalJumpTime));
            settings.glitchMaterial.SetFloat("_HorizontalShake", settings.horizontalShake * 0.2f);
            settings.glitchMaterial.SetVector("_ColorDrift", new Vector2(settings.colorDrift * 0.04f, Time.time * 606.11f));

            // Perform glitch blit
            Blit(cmd, source, tempTexture.Identifier(), settings.glitchMaterial);
            Blit(cmd, tempTexture.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
