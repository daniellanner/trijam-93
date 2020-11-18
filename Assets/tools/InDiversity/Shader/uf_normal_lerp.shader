Shader "Unlit/uf_normal_lerp"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
				_Color1("Color 1", Color) = (1,1,1,1)
				_Color2("Color 2", Color) = (1,1,1,1)
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
								float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
								float3 worldNormal : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
						fixed4 _Color1;
						fixed4 _Color2;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
								o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
								fixed4 col = 0;
								col.rg = i.uv;

								// normal is a 3D vector with xyz components; in -1..1
// range. To display it as color, bring the range into 0..1
// and put into red, green, blue components
								col.rgb = i.worldNormal*0.5 + 0.5;
								col.rgb = dot(col.rgb, fixed3(.894, 0, -.447));
                return lerp(_Color1, _Color2, pow(col, 2));
            }
            ENDCG
        }
    }
}
