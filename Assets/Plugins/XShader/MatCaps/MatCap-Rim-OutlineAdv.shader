Shader "XShader/MatCap-Rim-OutlineAdv"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}

		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5

		[Toggle(Color_On)] _EnableColor("EnableColor?", Float) = 0
		_Color("Color",color) = (1,1,1,0.5)

		[Toggle(MatCap_On)] _EnableMatCap("EnableMatCap?", Float) = 1
		_MatCap("MatCap Lookup", 2D) = "white" {}
		_Intensity("Intensity",Range(0,10)) = 2

		[Toggle(Rim_On)] _EnableRim("EnableRim?", Float) = 1
		_RimColor("Rim Color (RGB)", Color) = (0.8,0.8,0.8,0.6)
		_RimPower("Rim Power", Range(-2,50)) = 0.5

		[Toggle(Fog_On)] _Enable_Fog("EnableFog?", Float) = 1

		// Blending state
		[HideInInspector] _Mode("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0


			//OUTLINE
			_Outline("Outline Width", Range(0,10)) = 0.05
			_OutlineColor("Outline Color", Color) = (1, 0, 0, 1)
			[Toggle(SMOOTH_Z_ARTEFACTS)] SMOOTH_Z_ARTEFACTS("SMOOTH_Z_ARTEFACTS?", Float) = 0
			[Toggle(OUTLINE_CONST_SIZE)] OUTLINE_CONST_SIZE("OUTLINE_CONST_SIZE?", Float) = 0

	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			Blend[_SrcBlend][_DstBlend]
			ZWrite[_ZWrite]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_fog
			#pragma shader_feature Color_On
			#pragma shader_feature MatCap_On
			#pragma shader_feature Rim_On
			#pragma shader_feature Fog_On
			#pragma shader_feature _ALPHATEST_ON
			#pragma shader_feature _ALPHABLEND_ON
			#pragma shader_feature _ALPHAPREMULTIPLY_ON

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 uv : TEXCOORD0;
#if Fog_On
				UNITY_FOG_COORDS(1)
#endif
#if Rim_On
				fixed3 rimColor : TEXCOORD2;
#endif
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
#if _ALPHATEST_ON
			fixed _Cutoff;
#endif
#if MatCap_On
			sampler2D _MatCap;
			float _Intensity;
#endif

#if Rim_On
			fixed _RimPower;
			fixed4 _RimColor;
#endif

#if Color_On
			fixed4 _Color;
#endif

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
#if MatCap_On
				o.uv.z = dot(normalize(UNITY_MATRIX_IT_MV[0].xyz), normalize(v.normal));
				o.uv.w = dot(normalize(UNITY_MATRIX_IT_MV[1].xyz), normalize(v.normal));
				o.uv.zw = o.uv.zw * 0.5 + 0.5;
#endif

#if Rim_On
				fixed rim = 1.0f - saturate(dot(normalize(ObjSpaceViewDir(v.vertex)), normalize(v.normal)));
				o.rimColor = (_RimColor.rgb * pow(rim, _RimPower));
#endif
#if Fog_On
				UNITY_TRANSFER_FOG(o,o.vertex);
#endif
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv.xy);
#if Color_On
				col *=  _Color;
#endif
#if MatCap_On
				float4 matCapColor = tex2D(_MatCap, i.uv.zw);
				col *= matCapColor * _Intensity;
#endif
#if Rim_On
				col.rgb +=i.rimColor;
#endif
#if _ALPHATEST_ON
				clip(col.a - _Cutoff);
#endif
#if Fog_On
				UNITY_APPLY_FOG(i.fogCoord, col);
#endif
				return col;
			}
			ENDCG
		}
		UsePass "Hidden/OutlinePass-Advanced/OUTLINE-ADVANCED"
	}
	CustomEditor "NormalShaderEditor"
}
