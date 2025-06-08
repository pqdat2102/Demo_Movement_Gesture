Shader "Custom/SphereBloodFadeShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Health ("Health (0-1)", Range(0, 1)) = 1.0 // Giá trị sức khỏe từ 0 (hết máu) đến 1 (đầy máu)
        _FadeThreshold ("Fade Threshold", Range(0, 1)) = 0.3 // Ngưỡng máu thấp để kích hoạt nhiễu
        _FadeSpeed ("Fade Speed", Range(0, 1)) = 0.5 // Tốc độ mờ dần
        _BloodIntensity ("Blood Intensity", Range(0, 1)) = 0.8 // Cường độ nhiễu máu
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        // Bật render cả hai mặt
        Cull Off

        Blend SrcAlpha OneMinusSrcAlpha // Kích hoạt alpha blending cho hiệu ứng mờ
        ZWrite Off // Tắt ghi độ sâu để tránh che khuất

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
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1; // Vị trí trong không gian thế giới
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Health;
            float _FadeThreshold;
            float _FadeSpeed;
            float _BloodIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 center = float3(0, 0, 0); // Tâm hình cầu
                float distance = length(i.worldPos - center);
                float sphere = 1.0 - smoothstep(0.0, 1.5, distance); // Mở rộng phạm vi hình cầu

                float fadeFactor = 1.0 - _Health; // Đảo ngược, 0 là đầy máu, 1 là hết máu
                fadeFactor = saturate(fadeFactor * _FadeSpeed); // Giới hạn và điều chỉnh tốc độ mờ

                // Tăng độ đỏ cao hơn
                float redIntensity = saturate(fadeFactor * 3.0); // Tăng hệ số để đỏ đậm hơn
                float3 baseColor = lerp(float3(0.0, 0.0, 0.0), float3(1.2, 0.0, 0.0), redIntensity); // Đỏ đậm hơn (1.2)

                // Tạo nhiễu lấp đầy màn hình khi máu thấp
                float bloodEffect = step(_FadeThreshold, 1.0 - _Health); // Kích hoạt khi Health < _FadeThreshold
                float noiseX = frac(sin(dot(i.worldPos.xy, float2(70, 78.233))) * 60000); // Nhiễu theo trục X
                float noiseY = frac(sin(dot(i.worldPos.yz, float2(70, 78.233))) * 60000); // Nhiễu theo trục Y
                float noise = (noiseX + noiseY) * 0.5; // Kết hợp nhiễu đa chiều
                float bloodCoverage = bloodEffect * _BloodIntensity * noise; // Nhiễu lấp đầy màn hình

                // Kết hợp màu và alpha
                fixed4 col = tex2D(_MainTex, i.uv) * float4(baseColor, 1.0);
                col.a = sphere * (1.0 - fadeFactor) + bloodCoverage; // Nhiễu lấp đầy khi máu thấp
                col.a = saturate(col.a); // Giới hạn alpha từ 0 đến 1
                col.rgb = lerp(col.rgb, float3(1.0, 0.0, 0.0), redIntensity); // Đảm bảo đỏ đậm khi máu giảm

                return col;
            }
            ENDCG
        }
    }
}