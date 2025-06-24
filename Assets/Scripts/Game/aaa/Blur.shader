Shader "CustomEffects/Blur"
{
    HLSLINCLUDE
    
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        // The Blit.hlsl file provides the vertex shader (Vert),
        // the input structure (Attributes), and the output structure (Varyings)
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        float _VerticalBlur;
        float _HorizontalBlur;

        float _Amplitude;
        float _Constant;
        float _Threshold;
    
        float4 BlurVertical (Varyings input) : SV_Target
        {
            
            float3 color = 0;
            float pixelFactor1;
            float pixelFactor2;
            //float pi = 3.14159265;
            float blurPixels = 0 * _ScreenParams.y;
            float power = 8.0f;
            float phaseOffset = 3.14159265f / 8;
            float pixelsPerWave = 6;
           

            color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord).rgb;

  
            pixelFactor1 = min ( (1.0f - pow(cos(-_VerticalBlur * phaseOffset + (input.texcoord.y) * 3.14159265f  * _ScreenParams.y / pixelsPerWave), power)) ,
                   (1.0f - pow(cos(-_VerticalBlur * phaseOffset + (input.texcoord.x ) * 3.14159265f  * _ScreenParams.x / pixelsPerWave), power)) )
                   * _Amplitude 
                   + _Constant;
            
                   pixelFactor2 = min ( (1.0f - pow(cos(_VerticalBlur * phaseOffset + (input.texcoord.y) * 3.14159265f * _ScreenParams.y / pixelsPerWave), power)) ,
                   (1.0f - pow(cos(_HorizontalBlur * phaseOffset + (input.texcoord.x) * 3.14159265f  * _ScreenParams.x / pixelsPerWave), power)) )
                   * _Amplitude 
                   + _Constant;
                   
                   pixelFactor1 = pixelFactor1 < _Threshold ?  pixelFactor1 * 0.8f  : pixelFactor1;
                   pixelFactor2 = pixelFactor2 < _Threshold ?  pixelFactor2 * 0.8f  : pixelFactor2;

                   color.g *= pixelFactor1;
                   color.rb *= pixelFactor2;

            return float4(color.rgb, 1);
        }
    
    ENDHLSL
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "BlurPassVertical"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment BlurVertical
            
            ENDHLSL
        }

    }
}