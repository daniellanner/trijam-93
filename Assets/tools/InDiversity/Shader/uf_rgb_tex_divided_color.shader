Shader "indiversity/unlit/uf_rgb_tex_divided_color"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
				[HDR] _ColorR("ColorR", Color) = (1,1,1,1)
				[HDR] _ColorG("ColorG", Color) = (1,1,1,1)
				[HDR] _ColorB("ColorB", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
						float4 _ColorR;
						float4 _ColorG;
						float4 _ColorB;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                
						fixed4 r = _ColorR * col.r;
						fixed4 g = _ColorB * col.b;
						fixed4 b = _ColorG * col.g;

                return r + g + b;
            }
            ENDCG
        }
    }
}
