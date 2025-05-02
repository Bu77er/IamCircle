Shader "Custom/HatchMatShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Hatch0 ("Hatch 0 (Light)", 2D) = "white" {}
        _Hatch1 ("Hatch 1", 2D) = "white" {}
        //_UVBuffer ("UV Buffer", 2D) = "white" {}
        _HatchScale ("Hatch UV Scale", Float) = 8
        _IntensityMultiplier ("Brightness Multiplier", Float) = 1
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uvFlipY : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _Hatch0;
            sampler2D _Hatch1;
            sampler2D _UVBuffer;

            float _HatchScale;
            float _IntensityMultiplier;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uvFlipY = o.uv;
#if defined(UNITY_UV_STARTS_AT_TOP) && !defined(SHADER_API_MOBILE)
                o.uvFlipY.y = 1.0 - o.uv.y;
#endif
                return o;
            }

            fixed3 Hatching(float2 uv, half intensity)
            {
                half3 hatch0 = tex2D(_Hatch0, uv).rgb;
                half3 hatch1 = tex2D(_Hatch1, uv).rgb;

                half overbright = max(0, intensity - 1.0);

                half3 weight0 = saturate(intensity * 2.0 - half3(0, 0.5, 1));
                half3 weight1 = saturate(intensity * 2.0 - half3(1.5, 2.0, 2.5));

                fixed3 hatch = hatch0 * weight0 + hatch1 * weight1;

                return hatch + overbright;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float4 uv = tex2D(_UVBuffer, i.uvFlipY);
                half intensity = dot(col.rgb, float3(0.2326, 0.7152, 0.0722)) * _IntensityMultiplier;
                fixed3 hatch = Hatching(uv.xy * _HatchScale, intensity);

                col.rgb = hatch;
                return col;
            }
            ENDCG
        }
    }
}
