Shader "indiversity/gamejam/uf_game_ground"
{
	Properties
	{
		_MainTex ("Diffuse Texture", 2D) = "white" {}
		[Header(Spring Colours)]
		[HDR] _ColorSpringR("Color Spring Red Channel", Color) = (1,0,0,1)
		[HDR] _ColorSpringG("Color Spring Green Channel", Color) = (0,1,0,1)
		[HDR] _ColorSpringB("Color Spring Blue Channel", Color) = (0,0,1,1)

		[Header(Clipping options)]
		[HDR] _ColorFacing("Cut off Color", Color) = (1,1,1,1)
		_VisibleRadius("Visible Radius", float) = 1
		_NoiseTex("Season change Noise", 2D) = "white" {}

		_ScreenNoiseTex("Screen Position Noise", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100


		Pass
		{
			Cull Off // turn off backface culling
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "id_utilities.cginc"
			#include "id_gamejam_collection.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 vertex : SV_POSITION;
			};

			// Textures
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
			sampler2D _ScreenNoiseTex;
			float4 _ScreenNoiseTex_ST;

			// Colors
			float4 _ColorSpringR;
			float4 _ColorSpringG;
			float4 _ColorSpringB;
			
			float4 _ColorFacing;
			float _VisibleRadius;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv, _NoiseTex);
				o.uv.zw = ComputeScreenPos(o.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			fixed4 frag(v2f i, fixed facing : VFACE) : SV_Target
			{
				// clip
				float n = tex2D(_ScreenNoiseTex, i.uv.zw * 0.1).r;
				ClipCircle(i.worldPos, float3(1.0, 0.0, 0.0), -_VisibleRadius);
				ClipPlane(i.worldPos, float3(-1.0, 0.0, 0.0), -CLIP_DISTANCE);
				
				// spring
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 springColor = _ColorSpringR;
				springColor = lerp(springColor, _ColorSpringG, col.g);
				springColor = lerp(springColor, _ColorSpringB, col.b);

				//discard surface above plane
				fixed4 visibleColor = springColor;

				// lerp with backface color
				fixed4 objectColor = lerp(_ColorFacing, visibleColor, facing * 0.5 + 0.5);

				// screen noise
				float screenNoise = tex2D(_ScreenNoiseTex, i.uv.zw * 0.05).r;

				//return lerp(objectColor, (1, 1, 1, 1), screenNoise);
				fixed4 finishedColor = lerp(objectColor, fixed4(1, 1, 1, 1), screenNoise);
				finishedColor.a = col.a;

				return finishedColor;
			}
			ENDCG
		}
	}
}
