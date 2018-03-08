Shader "Hidden/GlassBroken"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D _GlassTex;
			float _Power;

			struct v2f
			{
				float2 uv : TEXCOORD0;
				fixed4 vertex : SV_POSITION;
			};

			v2f vert (appdata_img v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 glass = tex2D(_GlassTex, i.uv);
				float2 offset = i.uv + (glass.rg - float2(0.5, 0.5))  * _Power;
				fixed4 col= tex2D(_MainTex, offset);
				return col;
			}
			ENDCG
		}
	}
	Fallback off
}