Shader "Custom/TranslucentUIShader"
{
    Properties
    {
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}
        [MainColor] _Color ("Tint", Color) = (1,1,1,1)
        
        _BlurSize ("Blur Size", Range(0.0, 20.0)) = 2.0
        _BlurRadius ("Blur Radius", Range(0.0, 8.0)) = 4.0

        [Toggle] _IS_TRANSPARENT ("Is Transparent", Float) = 0
        
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        
        _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        
        Pass
        {
            
            Name "Default"
            
            HLSLPROGRAM
            #pragma multi_compile __ _IS_TRANSPARENT_ON
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            struct appdata
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                half4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float2 screenPosition : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            half4 _Color;
            float _BlurSize;
            float _BlurRadius;
            
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.worldPosition = v.vertex;
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.vertex = TransformObjectToHClip(o.worldPosition);
                
                float4 screenPos = ComputeScreenPos(o.vertex);
                o.screenPosition = screenPos.xy / screenPos.w;
                
                o.color = v.color * _Color;
                
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 c = tex2D(_MainTex, i.texcoord) * i.color;
                
                clip(c.a - 0.001);
                
                half3 color = half3(0,0,0);
                float step = _BlurSize / 5000.0;

                int radius = (int)_BlurRadius;
                
                for (int x = -radius; x <= radius; x++)
                {
                    for (int y = -radius; y <= radius; y++)
                    {
                        color += SampleSceneColor(i.screenPosition + float2(x, y) * step);
                    }
                }

                float kernel = pow((float)(radius * 2 + 1), 2);
                color /= kernel;
                #if _IS_TRANSPARENT_ON

                color = lerp(color, i.color.rgb, i.color.a);
                //color = saturate(color * i.color.rgb);
                return half4(color.rgb, i.color.a);
                #else
                color = lerp(color, i.color.rgb, i.color.a);
                return half4(color.rgb, 1);
                #endif
                
            }
            ENDHLSL
        }
    }
}