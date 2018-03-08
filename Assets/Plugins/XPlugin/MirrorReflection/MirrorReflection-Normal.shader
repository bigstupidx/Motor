Shader "FX/MirrorReflection-Normal"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		_ReflectionTex ("", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "IgnoreProjector" = "False" }
		LOD 100
 
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BumpMap;
			float4 _BumpMap_ST;
			fixed4 _Color;
			sampler2D _ReflectionTex;

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 bumpUV : TEXCOORD2;
				float4 refl : TEXCOORD1;
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
			};
			v2f vert(float4 pos : POSITION, float2 uv : TEXCOORD0,float3 normal:NORMAL)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, pos);
				o.uv = TRANSFORM_TEX(uv, _MainTex);
				o.bumpUV = TRANSFORM_TEX(uv, _BumpMap);
				o.refl = ComputeScreenPos (o.pos);
				o.normal = normal;
				return o;
			}
			fixed4 frag(v2f i) : SV_Target
			{
				fixed3 normal = UnpackNormal(tex2D(_BumpMap, i.bumpUV)).rgb;
				fixed4 tex = tex2D(_MainTex, i.uv);
				fixed4 refuv = i.refl;
				refuv.xy -= normal.xy;

				fixed4 rtRefl = tex2D(_ReflectionTex, (i.refl.xy / i.refl.w) + normal.xy)*_Color;

				return tex*rtRefl;

				//fixed4 refl = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(refuv))*_Color;
				//return tex * refl;
			}
			ENDCG
	    }
	}
}