Shader "Custom/ToonShaderWithOutline"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range (.002, 0.03)) = .005
        _Shininess ("Shininess", Range (0.03, 1)) = 0.078125
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        // Pass for the outline
        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }
            Cull Front
            ZWrite On
            ZTest LEqual
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha
            
            // Outline logic
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            uniform float _OutlineWidth;
            uniform float4 _OutlineColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                // Expand the vertices along their normals for the outline effect
                v2f o;
                float3 norm = mul((float3x3) unity_ObjectToWorld, v.normal);
                o.pos = UnityObjectToClipPos(v.vertex + float4(norm * _OutlineWidth, 0));
                o.color = _OutlineColor;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }

        // Pass for the toon shading
        Pass
        {
            Name "TOON"
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            #pragma surface surf ToonRamp

            sampler2D _MainTex;
            fixed4 _Color;

            struct Input
            {
                float2 uv_MainTex;
            };

            void surf (Input IN, inout SurfaceOutput o)
            {
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                o.Alpha = c.a;
            }

            inline half4 LightingToonRamp (SurfaceOutput s, half3 lightDir, half atten)
            {
                // Simple toon shading, 3 steps
                half NdotL = dot (s.Normal, lightDir);
                half diff = saturate(NdotL);

                half4 c;
                if (diff > 0.5)
                    c.rgb = s.Albedo * _LightColor0.rgb;
                else if (diff > 0.25)
                    c.rgb = s.Albedo * _LightColor0.rgb * 0.7;
                else
                    c.rgb = s.Albedo * _LightColor0.rgb * 0.5;

                c.a = s.Alpha;
                return c;
            }
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}
