using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DigitalGlitchFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class DigitalGlitchSettings
    {
        public Material glitchMaterial;

        [Range(0f, 1f)]
        public float intensity = 0.5f;

        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public DigitalGlitchSettings settings = new DigitalGlitchSettings();
    DigitalGlitchPass glitchPass;

    public override void Create()
    {
        if (settings.glitchMaterial == null)
        {
            Debug.LogError("DigitalGlitchFeature: Missing glitch material.");
            return;
        }

        glitchPass = new DigitalGlitchPass(settings);
        glitchPass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (glitchPass != null)
        {
            glitchPass.ConfigureInput(ScriptableRenderPassInput.Color);
            renderer.EnqueuePass(glitchPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && glitchPass != null)
        {
            glitchPass.Dispose();
            glitchPass = null;
        }
    }

    class DigitalGlitchPass : ScriptableRenderPass
    {
        DigitalGlitchSettings settings;
        RenderTargetHandle tempTexture;
        RenderTexture trash1;
        RenderTexture trash2;
        Texture2D noiseTex;
        int screenWidth = -1;
        int screenHeight = -1;

        public DigitalGlitchPass(DigitalGlitchSettings settings)
        {
            this.settings = settings;
            tempTexture.Init("_TempDigitalGlitch");

            CreateNoiseTex();
        }

        void CreateNoiseTex()
        {
            noiseTex = new Texture2D(64, 32, TextureFormat.ARGB32, false);
            noiseTex.wrapMode = TextureWrapMode.Clamp;
            noiseTex.filterMode = FilterMode.Point;
            noiseTex.hideFlags = HideFlags.DontSave;
        }

        void UpdateNoise()
        {
            Color c = RandomColor();

            for (int y = 0; y < noiseTex.height; y++)
            {
                for (int x = 0; x < noiseTex.width; x++)
                {
                    if (Random.value > 0.89f) c = RandomColor();
                    noiseTex.SetPixel(x, y, c);
                }
            }

            noiseTex.Apply();
        }

        static Color RandomColor()
        {
            return new Color(Random.value, Random.value, Random.value, Random.value);
        }

        void UpdateTrashTexturesIfNeeded(int width, int height)
        {
            if (trash1 == null || trash1.width != width || trash1.height != height)
            {
                ReleaseTrashTextures();

                trash1 = new RenderTexture(width, height, 0);
                trash2 = new RenderTexture(width, height, 0);
                trash1.hideFlags = HideFlags.DontSave;
                trash2.hideFlags = HideFlags.DontSave;

                screenWidth = width;
                screenHeight = height;
            }

            if (Time.frameCount % 13 == 0)
                Graphics.Blit(null, trash1);

            if (Time.frameCount % 73 == 0)
                Graphics.Blit(null, trash2);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (settings.glitchMaterial == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("DigitalGlitchPass");

            var source = renderingData.cameraData.renderer.cameraColorTargetHandle;
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(tempTexture.id, desc, FilterMode.Bilinear);

            UpdateNoise();
            UpdateTrashTexturesIfNeeded(desc.width, desc.height);

            settings.glitchMaterial.SetFloat("_Intensity", settings.intensity);
            settings.glitchMaterial.SetTexture("_NoiseTex", noiseTex);
            settings.glitchMaterial.SetTexture("_TrashTex", Random.value > 0.5f ? trash1 : trash2);

            Blit(cmd, source, tempTexture.Identifier(), settings.glitchMaterial);
            Blit(cmd, tempTexture.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempTexture.id); // Ωˆ Õ∑≈¡Ÿ ± RT
        }

        public void Dispose()
        {
            ReleaseTrashTextures();

            if (noiseTex != null)
            {
                Object.DestroyImmediate(noiseTex);
                noiseTex = null;
            }
        }

        void ReleaseTrashTextures()
        {
            if (trash1 != null)
            {
                trash1.Release();
                Object.DestroyImmediate(trash1);
                trash1 = null;
            }

            if (trash2 != null)
            {
                trash2.Release();
                Object.DestroyImmediate(trash2);
                trash2 = null;
            }
        }
    }
}
