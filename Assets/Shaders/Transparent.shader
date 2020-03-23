Shader "Custom/Transparent"
{
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Tint", Color) = (0, 0, 0, 1)
    }
    
    SubShader {
        Tags{"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade

        sampler2D _MainTex;
        float4 _Color;

        struct Input {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }

    Fallback "Mobile/VertexLit"
}
