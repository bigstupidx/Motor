Shader "Unlit/Indicator-Trans-VertColor"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}

		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		// Blending state
		[HideInInspector] _Mode("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100
			Blend[_SrcBlend][_DstBlend]
			ZWrite[_ZWrite]
		Cull Off Lighting Off Fog{ Color(0,0,0,0) }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _Indicator_Move;

#if _ALPHATEST_ON
			fixed _Cutoff;
#endif
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.x += _Indicator_Move.x;
				o.uv.x *= -_Indicator_Move.y;
				o.color = v.color;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv)*i.color;
#if _ALPHATEST_ON
			clip(col.a - _Cutoff);
#endif
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
			CustomEditor "NormalShaderEditor"
}
