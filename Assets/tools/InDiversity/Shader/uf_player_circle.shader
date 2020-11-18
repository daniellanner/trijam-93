Shader "indiversity/gamejam/uf_player_circle"
{
		Properties
		{
				_NoiseTex ("Texture", 2D) = "white" {}
				[HDR] _Color1 ("Color 1", Color) = (1,1,1,1)
				[HDR] _Color2("Color 2", Color) = (1,1,1,1)
				_Radius("Radius", Range(0.1, 0.5)) = 0.5
		}
		SubShader
		{
			Tags
			{
				"RenderType" = "Transparent"
				"Queue" = "Transparent"
			}
			LOD 100

			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

				Pass
				{
						CGPROGRAM
						#pragma vertex vert
						#pragma fragment frag

						#include "UnityCG.cginc"
						#include "id_utilities.cginc"

						struct appdata
						{
								float4 vertex : POSITION;
								float2 uv : TEXCOORD0;
						};

						struct v2f
						{
								float4 uv : TEXCOORD0;
								float4 vertex : SV_POSITION;
						};

						sampler2D _NoiseTex;
						float4 _NoiseTex_ST;

						float4 _Color1;
						float4 _Color2;
						float _Radius;

						v2f vert (appdata v)
						{
								v2f o;
								o.vertex = UnityObjectToClipPos(v.vertex);
								o.uv.xy = TRANSFORM_TEX(v.uv, _NoiseTex);
								o.uv.zw = ComputeScreenPos(o.vertex);
								return o;
						}

						fixed4 frag (v2f i) : SV_Target
						{
								// calc alpha for circular shape
								float2 center = float2(.5, .5);
								float2 untileduv = i.uv.xy;
								untileduv.x *= 1.0 / _NoiseTex_ST.x;
								untileduv.y *= 1.0 / _NoiseTex_ST.y;

								float2 offset = untileduv - center;
								float sqrdistance = offset.x * offset.x + offset.y * offset.y;
								float alpha = _Radius * _Radius;

								float2 uv = i.uv.zw;
								uv.x *= _NoiseTex_ST.x;
								uv.y *= _NoiseTex_ST.y;

								float noise = tex2D(_NoiseTex, uv).r;

								fixed4 col = lerp(_Color1, _Color2, noise);
								col.a = LessThan(sqrdistance, alpha);

								return col;
						}
						ENDCG
				}
		}
}
