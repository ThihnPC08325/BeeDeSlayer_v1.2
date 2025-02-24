// Tạo file shader mới tên là "TransparentDome.shader"
Shader "Custom/TransparentDome"
{
    Properties
    {
        _Color ("Color", Color) = (0,1,0,0.3)
        _RimPower ("Rim Power", Range(0.1,10.0)) = 3.0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade

        struct Input
        {
            float3 viewDir;
        };

        fixed4 _Color;
        float _RimPower;

        void surf (Input IN, inout SurfaceOutput o)
        {
            float rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
            o.Emission = _Color.rgb * pow(rim, _RimPower);
            o.Alpha = _Color.a * pow(rim, _RimPower);
        }
        ENDCG
    }
    FallBack "Diffuse"
}