Shader "Custom/ShockWave"
{
    Properties
    {
        _MainTex       ("Texture", 2D)    = "white" {}
        _WaveCenter    ("Wave Center", Vector) = (0,0,0,0)
        _StartTime     ("Wave Start Time", Float) = -1000
        _Speed         ("Expansion Speed", Float) = 5
        _MaxRadius     ("Max Radius", Float) = 20
        _Thickness     ("Ring Half-Thickness", Float) = 1
        _RadialAmp     ("Radial Offset Amp", Float) = 1
        _NormalAmp     ("Normal Offset Amp", Float) = 1
        _WaveColor     ("Wave Color", Color) = (1,1,1,1)
        _FadeOut       ("Fade Out Strength", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard vertex:vert
        #pragma target 3.0

        sampler2D _MainTex;
        float4   _WaveCenter;
        float    _StartTime, _Speed, _MaxRadius, _Thickness;
        float    _RadialAmp, _NormalAmp, _FadeOut;
        fixed4   _WaveColor;

        struct Input {
            float2 uv_MainTex;
            float  ringIntensity;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            float t = _Time.y - _StartTime;
            float curR = t * _Speed;
            if (t <= 0 || curR > _MaxRadius)
            {
                o.ringIntensity = 0;
                return;
            }

            float fade = saturate(1-(curR / _MaxRadius)) * _FadeOut;
            float d = distance(worldPos, _WaveCenter.xyz);
            float inner = smoothstep(curR - _Thickness, curR, d);
            float outer = smoothstep(curR, curR + _Thickness, d);
            float ring = inner * (1 - outer);
            ring *= fade;
            o.ringIntensity = ring;

            if (ring > 0)
            {
                float3 dir = normalize(worldPos - _WaveCenter.xyz);
                float3 radialOffsetWorld = dir * ring * _RadialAmp;

                float3 normalWorld = mul((float3x3)unity_ObjectToWorld, v.normal);
                float3 normalOffsetWorld = normalWorld * ring * _NormalAmp;

                float3 totalOffsetW = radialOffsetWorld + normalOffsetWorld;

                float4 objOffset = mul(unity_WorldToObject, float4(totalOffsetW,0));
                v.vertex.xyz += objOffset.xyz;
            }
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 baseCol = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo   = lerp(baseCol.rgb, _WaveColor.rgb, IN.ringIntensity);
            o.Emission = _WaveColor.rgb * IN.ringIntensity * 0.5;
            o.Alpha    = baseCol.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
