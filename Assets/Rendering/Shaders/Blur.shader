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
        SamplerState sampler_point_clamp;
    
        float4 BlurVertical (Varyings input) : SV_Target
        {
            
            float3 color = 0;
            float pixelFactor0;
            float pixelFactor1;
            float pixelFactor2;
            //float pi = 3.14159265;
            float blurPixels = 0 * _ScreenParams.y;
            float power = 8.0f;
            float phaseOffset = 3.14159265f / 8;
            float pixelsPerWave = 3;
            
           

            color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_point_clamp, input.texcoord).rgb;

            
            pixelFactor0 = min ( (1.0f - pow(cos((input.texcoord.y) * 3.14159265f  * _ScreenParams.y / pixelsPerWave), power)) ,
                   (1.0f - pow(cos((input.texcoord.x ) * 3.14159265f  * _ScreenParams.x / pixelsPerWave), power)) )
                   * _Amplitude 
                   + _Constant;
  
            pixelFactor1 = min ( (1.0f - pow(cos(-_VerticalBlur * phaseOffset + (input.texcoord.y) * 3.14159265f  * _ScreenParams.y / pixelsPerWave), power)) ,
                   (1.0f - pow(cos(-_HorizontalBlur * phaseOffset + (input.texcoord.x ) * 3.14159265f  * _ScreenParams.x / pixelsPerWave), power)) )
                   * _Amplitude 
                   + _Constant;
            
            pixelFactor2 = min ( (1.0f - pow(cos(_VerticalBlur * phaseOffset + (input.texcoord.y) * 3.14159265f * _ScreenParams.y / pixelsPerWave), power)) ,
                   (1.0f - pow(cos(_HorizontalBlur * phaseOffset + (input.texcoord.x) * 3.14159265f  * _ScreenParams.x / pixelsPerWave), power)) )
                   * _Amplitude 
                   + _Constant;
                   
                   pixelFactor1 = pixelFactor1 < _Threshold ?  pixelFactor1 * 0.8f  : pixelFactor1;
                   pixelFactor2 = pixelFactor2 < _Threshold ?  pixelFactor2 * 0.8f  : pixelFactor2;

            color.g *= pixelFactor1;
            color.r *= pixelFactor2;
            color.b *= pixelFactor0;
           
            /*bool xBool =  floor(input.texcoord.x  * _ScreenParams.x) % pixelsPerWave == 1;
            bool yBool =  floor(input.texcoord.y  * _ScreenParams.y) % pixelsPerWave == 1;

            if (xBool && yBool)
            {
                color.rb *= _Amplitude;
                color.g *=  1.0f - _Amplitude;
            }
            else if (xBool || yBool)
            {
                color.rgb *= _Constant;
            }
            else
            {
                color.rb *=  1.0f - _Amplitude;
                color.g *=  _Amplitude;
            }*/

            return float4(color.rgb, 1);

        }

        float4 PointSample (Varyings input) : SV_Target
        {
            return float4(SAMPLE_TEXTURE2D(_BlitTexture, sampler_point_clamp, input.texcoord).rgb, 1);
        }
    
    ENDHLSL
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "CRTPass"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment BlurVertical
            
            ENDHLSL
        }

        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "SamplePass"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment PointSample
            
            ENDHLSL
        }

    }
}