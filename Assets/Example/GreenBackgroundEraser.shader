Shader "GreenBackgroundEraser"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "GreenBackgroundEraserPass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // The Blit.hlsl file provides the vertex shader (Vert),
            // input structure (Attributes) and output strucutre (Varyings)
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag
            
            sampler2D _Texture;

            float get_inverted_color_mask(const float3 color)
            {
                const float3 mask_color = float3(0, 1, 0);
                return abs(1 - saturate(1 - (distance(mask_color, color) - 0.4f) / 1e-5));
            }

            half4 frag (Varyings input) : SV_Target
            {
                const float non_green_screen_alpha = get_inverted_color_mask(tex2D(_Texture, input.texcoord).xyz);
                return half4(tex2D(_Texture, input.texcoord).xyz, non_green_screen_alpha);
            }
            ENDHLSL
        }
    }
}