// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Beffio/Car Paint Opaque"
{
	Properties
	{
		_BaseColor("Base Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_DetailColor("Detail Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_DetailMap("Detail Map", 2D) = "white" {}
		_DetailMapDepthBias("Detail Map Depth Bias", Float) = 0.0
		_DiffuseColor("Diffuse Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_DiffuseMap("Diffuse Map", 2D) = "white" {}

		//_ColorR("ColorR", Color) = (1.0, 1.0, 1.0, 1.0)
		//_ColorG("ColorG", Color) = (1.0, 1.0, 1.0, 1.0)
		//_ColorB("ColorB", Color) = (1.0, 1.0, 1.0, 1.0)
		//_ColorMaskMap("Color Mask Map", 2D) = "black" {}


		_MatCapLookup("MatCap Lookup", 2D) = "white" {}
		_Intensity("Intensity",Range(0,10)) = 2
		_ReflectionColor("Reflection Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_ReflectionMap("Reflection Map", Cube) = "" {}
		_ReflectionStrength("Reflection Strength", Range(0.0, 1.0)) = 0.5
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Geometry"
			"RenderType" = "Opaque"
		}

		Pass
		{
			Blend Off
			Cull Back
			ZWrite On

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma fragment FragmentMain
			#pragma vertex VertexMain

			float4 _BaseColor;
			float4 _DetailColor;
			sampler2D _DetailMap;
			float4 _DetailMap_ST;
			float _DetailMapDepthBias;
			float4 _DiffuseColor;
			sampler2D _DiffuseMap;
			float4 _DiffuseMap_ST;


			sampler2D _MatCapLookup;
			float _Intensity;
			float4 _ReflectionColor;
			samplerCUBE _ReflectionMap;
			float _ReflectionStrength;

			//float4 _ColorR ;
			//float4 _ColorG ;
			//float4 _ColorB ;
			//sampler2D _ColorMaskMap;
			//float4 _ColorMaskMap_ST;

			struct VertexInput
			{
				float3 normal : NORMAL;
				float4 position : POSITION;
				float2 UVCoordsChannel1: TEXCOORD0;
			};

			struct VertexToFragment
			{
				float3 detailUVCoordsAndDepth : TEXCOORD0;
				float4 diffuseUVAndMatCapLookupCoords : TEXCOORD1;
				float4 position : SV_POSITION;
				float3 worldSpaceReflectionVector : TEXCOORD2;
			};

			VertexToFragment VertexMain(VertexInput input)
			{
				VertexToFragment output;

				output.diffuseUVAndMatCapLookupCoords.xy = TRANSFORM_TEX(input.UVCoordsChannel1, _DiffuseMap);

				output.diffuseUVAndMatCapLookupCoords.z = dot(normalize(UNITY_MATRIX_IT_MV[0].xyz), normalize(input.normal));
				output.diffuseUVAndMatCapLookupCoords.w = dot(normalize(UNITY_MATRIX_IT_MV[1].xyz), normalize(input.normal));
				output.diffuseUVAndMatCapLookupCoords.zw = output.diffuseUVAndMatCapLookupCoords.zw * 0.5 + 0.5;

				output.position = mul(UNITY_MATRIX_MVP, input.position);

				output.detailUVCoordsAndDepth.xy = TRANSFORM_TEX(input.UVCoordsChannel1, _DetailMap);
				output.detailUVCoordsAndDepth.z = output.position.z;

				float3 worldSpacePosition = mul(unity_ObjectToWorld, input.position).xyz;
				float3 worldSpaceNormal = normalize(mul((float3x3)unity_ObjectToWorld, input.normal));
				output.worldSpaceReflectionVector = reflect(worldSpacePosition - _WorldSpaceCameraPos.xyz, worldSpaceNormal);
				
				return output;
			}

			float4 FragmentMain(VertexToFragment input) : COLOR
			{
				float3 reflectionColor = texCUBE(_ReflectionMap, input.worldSpaceReflectionVector).rgb * _ReflectionColor.rgb;
				float4 diffuseColor = tex2D(_DiffuseMap, input.diffuseUVAndMatCapLookupCoords.xy) * _DiffuseColor;

				//float4 maskColor = tex2D(_ColorMaskMap, input.diffuseUVAndMatCapLookupCoords.xy);
				//if (maskColor.r > 0) {
				//	diffuseColor.rgb = diffuseColor *_ColorR;
				//	//diffuseColor.r *=_ColorR;
				//}
				//if (maskColor.g > 0) {
				//	diffuseColor.rgb = diffuseColor *_ColorG;
				//}
				//if (maskColor.b > 0) {
				//	diffuseColor.rgb = diffuseColor *_ColorB;
				//}

				float3 finalColor = lerp(lerp(_BaseColor.rgb, diffuseColor.rgb, diffuseColor.a), reflectionColor, _ReflectionStrength);

				float3 detailMask = tex2D(_DetailMap, input.detailUVCoordsAndDepth.xy).rgb;
				float3 detailColor = lerp(_DetailColor.rgb, finalColor, detailMask);
				finalColor = lerp(detailColor, finalColor, saturate(input.detailUVCoordsAndDepth.z * _DetailMapDepthBias));

				float3 matCapColor = tex2D(_MatCapLookup, input.diffuseUVAndMatCapLookupCoords.zw).rgb;
				return float4(finalColor * matCapColor * _Intensity, _BaseColor.a);
			}


			ENDCG
		}
	}

	Fallback "VertexLit"
}
