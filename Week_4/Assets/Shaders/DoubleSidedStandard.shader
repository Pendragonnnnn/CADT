// Standard lit shader that draws both faces. Used for cloth (flags, banners, sheets),
// because a single-layer cloth mesh would be invisible from behind with back-face culling.
Shader "PhysicsDemo/DoubleSidedStandard"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows addshadow
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _Color;
        half _Glossiness;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Smoothness = _Glossiness;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
