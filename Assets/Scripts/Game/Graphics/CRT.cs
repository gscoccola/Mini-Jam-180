using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRT : MonoBehaviour {
    public Shader crtShader;

    [Range(1.0f, 10.0f)]
    public float curvature = 1.0f;

    [Range(1.0f, 100.0f)]
    public float vignetteWidth = 30.0f;

    [SerializeField] RenderTexture input;
    [SerializeField] RenderTexture output;

    private Material crtMat;

    void Start() {
        crtMat ??= new Material(crtShader);
        crtMat.hideFlags = HideFlags.HideAndDontSave;
    }

    private void Update()
    {
        crtMat.SetFloat("_Curvature", curvature);
        crtMat.SetFloat("_VignetteWidth", vignetteWidth);
        Graphics.Blit(input, output, crtMat);
    }

}
