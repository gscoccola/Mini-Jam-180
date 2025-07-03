using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BlurRendererFeature : ScriptableRendererFeature
{
    [SerializeField] private BlurSettings settings;
    [SerializeField] private Shader crtShader;
    //[SerializeField] private Shader upscaleShader;
    [SerializeField] private RenderPassEvent renderPassEvent;
    private Material crtMaterial;
    //private Material upscaleMaterial;
    private BlurRenderPass blurRenderPass;

    public override void Create()
    {
        if (crtShader == null /*|| upscaleShader == null*/)
        {
            return;
        }
        /*shader = Shader.Find("Blur");
        Debug.Assert(shader != null, "Blur shader not found!");*/
        crtMaterial = new Material(crtShader);
        //upscaleMaterial = new Material(upscaleShader);
        blurRenderPass = new BlurRenderPass(crtMaterial, settings);

        blurRenderPass.renderPassEvent = renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer,
        ref RenderingData renderingData)
    {
        if (blurRenderPass == null)
        {
            return;
        }
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            renderer.EnqueuePass(blurRenderPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (Application.isPlaying)
        {
            Destroy(crtMaterial);
        }
        else
        {
            DestroyImmediate(crtMaterial);
        }
    }
}

[Serializable]
public class BlurSettings
{
    [Range(0, 2f)] public float horizontalBlur;
    [Range(0, 2f)] public float verticalBlur;
    [Range(0, 2f)] public float _Amplitude;
    [Range(0, 2f)] public float _Constant;
    [Range(0, 20f)] public float _Threshold;
}