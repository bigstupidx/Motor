Shader "SkillEff_UI/NGUI_Transparent"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}

		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
		
		_FlowlightTex ("Flowlight Texture", 2D) = "white" {}
		_Color ("Flowlight Color", color) = (0, 0, 0, 1)
		_Power ("Power", float) = 1
		_SpeedX ("SpeedX", float) = 1
		_SpeedY ("SpeedY", float) = 0
	}
	
	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType"="Plane"
		}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
				};
	
				struct v2f
				{
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
					half2 texflow : TEXCOORD1;
				};
	
				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _FlowlightTex;
				fixed4 _FlowlightTex_ST;
				fixed _SpeedX;
				fixed _SpeedY;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.color = v.color;
#ifdef UNITY_HALF_TEXEL_OFFSET
					o.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif
					o.texflow = TRANSFORM_TEX(v.texcoord, _FlowlightTex);
					o.texflow.x += _Time * _SpeedX;
					o.texflow.y += _Time * _SpeedY;
					o.color = v.color;

					return o;
				}	

				float _Power;
				fixed4 _Color;
				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
					clip (col.a - 0.01);

					fixed4 cadd = tex2D(_FlowlightTex, i.texflow) * _Power;
					cadd.rgb *= col.rgb * _Color;
					col.rgb += cadd.rgb;
					col.rgb *= col.a;
					
					return col;
				}
			ENDCG
		}
	}
}
