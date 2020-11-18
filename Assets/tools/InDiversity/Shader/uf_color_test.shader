Shader "indiversity/unlit/uf_color_test"
{
		Properties
		{
			[HDR] _Color ("Color", Color) = (1,1,1,1)
			[HDR] _Color1 ("Color", Color) = (1,1,1,1)
			_NoiseTex("Texture", 2D) = "white" {}
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
						#include "id_utilities.cginc"

						float4 _Color;
						float4 _Color1;
						sampler2D _NoiseTex;
						float4 _NoiseTex_ST;

						struct appdata
						{
								float4 vertex : POSITION;
								float2 uv : TEXCOORD0;
						};

						struct v2f
						{
								float4 vertex : SV_POSITION;
								float3 worldPos : TEXCOORD0;
								float4 uv : TEXCOORD1;
						};

						v2f vert (appdata v)
						{
								v2f o;
								o.worldPos = mul(unity_ObjectToWorld, v.vertex);
								o.vertex = UnityObjectToClipPos(v.vertex);
								o.uv.xy = TRANSFORM_TEX(v.uv, _NoiseTex);
								o.uv.zw = ComputeScreenPos(o.vertex);
								return o;
						}

						fixed4 frag (v2f i) : SV_Target
						{
								float2 screensample = i.uv.zw;
								screensample = screensample * 0.125;

								screensample.x += _Time.y;
								screensample.y -= _Time.y;

								float noise = tex2D(_NoiseTex, screensample).r;

								noise = noise * 1.0;

								float3 plane1 = float3(-PI/4, 0.0, -PI/4);
								float distance = dot(i.worldPos, plane1);

								float3 plane2 = float3(-PI / 4, 0.0, PI / 4);
								float distance2 = dot(i.worldPos, plane2);

								//discard surface above plane
								float t = LessThan(-distance - noise, 0.0);
								float t2 = LessThan(-distance2 - noise, 0.0);
								// sample the texture
								return t * t2;
						}
						ENDCG
				}
		}
}
