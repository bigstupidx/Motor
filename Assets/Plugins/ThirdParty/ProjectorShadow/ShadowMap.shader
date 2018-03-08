// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "ShadowMap" {
	Properties{
			_MainTex("_MainTex", 2D) = "gray" {}
			_Strength("_Strength", Range(0, 1)) = 0.1
			_Falloff("_Falloff",2D) = "gray"{}
			[Toggle(Blur_On)] _EnableBlur("Enable Blur ?", Float) = 1
			_Blur("_Blur",Range(0,0.01)) = 0.001
		}
		Subshader{
		Tags{ "Queue" = "Transparent" }
		Pass{

		ZWrite Off
		ColorMask RGB

		Offset -1, -1

		//AlphaTest Greater -0.5
		Blend SrcAlpha OneMinusSrcAlpha
	

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		#pragma shader_feature Blur_On
		struct v2f {
			float4 uvShadow : TEXCOORD0;
		//	float4 uvFalloff : TEXCOORD1;
			float4 pos : SV_POSITION;
		};

		uniform float4x4 ShadowMatrix;
		sampler2D _MainTex;
		sampler2D _Falloff;
		float _Strength;
		float _Blur;

		v2f vert(appdata_base v)
		{
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			float4x4 matWVP = mul(ShadowMatrix, unity_ObjectToWorld);
			o.uvShadow = mul(matWVP, v.vertex);
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			half2 uv = i.uvShadow.xy / i.uvShadow.w * 0.5 + 0.5;
#if UNITY_UV_STARTS_AT_TOP
			uv.y = 1 - uv.y;
#endif
			fixed4 res = fixed4(0, 0, 0, 0);

			//fixed4 texS = tex2Dproj(_MainTex, UNITY_PROJ_COORD(fixed4(uv,0,0)));
			fixed4 texS = tex2D(_MainTex, uv);

#if Blur_On
			fixed4 texL = tex2D(_MainTex, uv + fixed2(_Blur, 0));
			fixed4 texR = tex2D(_MainTex, uv + fixed2(-_Blur, 0));
			fixed4 texU = tex2D(_MainTex, uv + fixed2(0, _Blur));
			fixed4 texB = tex2D(_MainTex, uv + fixed2(-0, -_Blur));
			texS.a += texL.a + texR.a + texU.a + texB.a;
			texS.a /= 5;
#endif

			fixed4 falloff = tex2D(_Falloff, uv);
			res.a = _Strength*texS.a*falloff.a;
			return res;
		}
			ENDCG
		}
		}
}