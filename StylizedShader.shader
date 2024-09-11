Shader "Custom/ToonShaderWithAccurateOutline"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)  // Kontur Rengi
        _OutlineThickness ("Outline Thickness", Float) = 0.015 // Kontur Kalýnlýðý, daha ince yapýldý
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        // Normal gölgeleme ve render
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = normalize(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = max(0, dot(i.normal, lightDir));

                // Toon step shading
                if (NdotL > 0.5)
                    NdotL = 1.0;
                else
                    NdotL = 0.5;

                fixed4 tex = tex2D(_MainTex, i.uv) * _Color;
                tex.rgb *= NdotL;
                return tex;
            }
            ENDCG
        }

        // Kontur (Stroke) Pass'i
        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }
            Cull Front  // Ön yüzeyi gizleyip arka yüzeyi çizeriz

            CGPROGRAM
            #pragma vertex vertOutline
            #pragma fragment fragOutline
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };

            float _OutlineThickness; // Kontur kalýnlýðý
            float4 _OutlineColor;    // Kontur rengi

            v2f vertOutline(appdata_t v)
            {
                // Normali dünya uzayýnda hesaplýyoruz
                v2f o;
                float3 normalDir = mul((float3x3)unity_ObjectToWorld, v.normal); // Dünyaya göre normal
                float3 newPos = v.vertex.xyz + normalDir * _OutlineThickness; // Normale göre geniþlet
                o.pos = UnityObjectToClipPos(float4(newPos, 1.0)); // Yeni pozisyonu hesapla
                o.color = _OutlineColor; // Kontur rengini uygula
                return o;
            }

            fixed4 fragOutline(v2f i) : SV_Target
            {
                return i.color; // Kontur rengi döndürülür
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
