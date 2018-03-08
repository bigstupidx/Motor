Shader "LS/AlphaMask"
{
	Properties 
	{
		_MainTex ("Main Tex", 2D) = "white" {}
		_MaskTex("Mask Tex",2D) = "white"{}
		[Enum(R,0,G,1,B,2,A,3,Gray,4)]_BlendType("Mask Type", Float) = 4
		_Strength("Strength",Range(0,1)) = 1
	}
	SubShader 
	{
		Tags 
		{
			"Queue"="Transparent" 
		}

		Lighting off
		ZWrite off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				half2 texcoord : TEXCOORD0;
			};

			struct v2f
			{	
				float4 vertex : SV_POSITION;
				float2 uv_MainTex : TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _MaskTex;
			float4 _MainTex_ST;
			float _Strength;
			float _MaskType;
			
			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
		
			fixed4 frag(v2f IN) : COLOR
			{
				fixed4 col = tex2D(_MainTex, IN.uv_MainTex);
				fixed4 mask = tex2D(_MaskTex, IN.uv_MainTex);
				fixed maskValue = 1;
				maskValue = (mask.r + mask.g + mask.b) / 3;
				col.a *=  (maskValue + 1 - _Strength);
				return col;
			}

			ENDCG
		} 
	}
}