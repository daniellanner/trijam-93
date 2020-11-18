Shader "indiversity/gamejam/uft_seasonal"
{
	Properties
	{
		_MainTex ("Diffuse Texture", 2D) = "white" {}
		[Header(Spring Colours)]
		[HDR] _ColorSpringR("Color Spring Red Channel", Color) = (1,0,0,1)
		[HDR] _ColorSpringG("Color Spring Green Channel", Color) = (0,1,0,1)
		[HDR] _ColorSpringB("Color Spring Blue Channel", Color) = (0,0,1,1)

		[Header(Winter Colours)]
		[HDR] _ColorWinterR("Color Winter Red Channel", Color) = (1,0,0,1)
		[HDR] _ColorWinterG("Color Winter Green Channel", Color) = (0,1,0,1)
		[HDR] _ColorWinterB("Color Winter Blue Channel", Color) = (0,0,1,1)

		[Header(Clipping options)]
		[HDR] _ColorFacing("Cut off Color", Color) = (1,1,1,1)
		_NoiseTex("Season change Noise", 2D) = "white" {}

		_ScreenNoiseTex("Screen Position Noise", 2D) = "white" {}
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
			
			float4 _ColorWinterR;
			float4 _ColorWinterG;
			float4 _ColorWinterB;
			
			float4 _ColorFacing;


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
				ClipPlane(i.worldPos, float3(1.0, 0.0, 0.0), -CLIP_DISTANCE);
				ClipPlane(i.worldPos, float3(-1.0, 0.0, 0.0), -CLIP_DISTANCE);
				//ClipPlane(i.worldPos, float3(0.0, 1.0, 0.0), -CLIP_DISTANCE);
				//ClipPlane(i.worldPos, float3(0.0, -1.0, 0.0), -CLIP_DISTANCE);

				
				// spring
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 springColor = _ColorSpringR;
				springColor = lerp(springColor, _ColorSpringG, col.g);
				springColor = lerp(springColor, _ColorSpringB, col.b);

				// winter
				float normal = dot(i.worldNormal*0.5 + 0.5, fixed3(1, 0, 0));

				fixed4 white = fixed4(1, 1, 1, 1);
				fixed4 winter = springColor;

				winter = lerp(winter, white, 0.25);
				winter = lerp(winter, white, normal);
				winter = lerp(winter, white, GreaterThan(normal, 0.75));

				fixed4 winterColor = _ColorWinterR;
				winterColor = lerp(winterColor, _ColorWinterG, col.g);
				winterColor = lerp(winterColor, _ColorWinterB, col.b);


				// season lerp
				float2 screensample = i.uv.zw;
				screensample = screensample * 0.125;
				screensample.x += _Time.y;
				screensample.y -= _Time.y;

				float noise = tex2D(_NoiseTex, screensample).r;
				noise = noise * 1.0;

				float3 plane1 = float3(-PI / 4, 0.0, -PI / 4);
				float distance1 = dot(i.worldPos, plane1);

				float3 plane2 = float3(-PI / 4, 0.0, PI / 4);
				float distance2 = dot(i.worldPos, plane2);

				//discard surface above plane
				float t = LessThan(-distance1 - noise, 0.0);
				float t2 = LessThan(-distance2 - noise, 0.0);

				float iswinter = t * t2;
				fixed4 visibleColor = lerp(winterColor, springColor, iswinter);

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
