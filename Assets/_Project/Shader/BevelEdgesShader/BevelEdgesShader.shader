Shader "Custom/SmoothBevelShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BevelWidth("Bevel Width", Range(0, 0.5)) = 0.1
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 normal : TEXCOORD1;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float _BevelWidth;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                    float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                    float3 viewDir = normalize(UnityWorldSpaceViewDir(v.vertex));
                    float3 bevelDir = normalize(viewDir - worldNormal * dot(viewDir, worldNormal));

                    o.normal = normalize(worldNormal + bevelDir * _BevelWidth);

                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    return tex2D(_MainTex, i.uv);
                }
                ENDCG
            }
        }
}
