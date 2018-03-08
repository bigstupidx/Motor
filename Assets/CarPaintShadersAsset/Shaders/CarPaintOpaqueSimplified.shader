Shader "Beffio/Car Paint Opaque (Simplified)"
{
	Properties
	{
		_BaseColor("Base Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_DetailColor("Detail Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_DetailMap("Detail Map", 2D) = "white" {}
		_DetailMapDepthBias("Detail Map Depth Bias", Float) = 1.0
		_DiffuseColor("Diffuse Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_DiffuseMap("Diffuse Map", 2D) = "white" {}
		_NormalMap("Normal Map", 2D) = "bump" {}
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

		Blend Off
		Cull Back
		ZWrite On

		CGPROGRAM
		#pragma target 3.0
		#pragma surface SurfaceMain Lambert vertex:VertexMain

		float4 _BaseColor;
		float4 _DetailColor;
		sampler2D _DetailMap;
		float _DetailMapDepthBias;
		float4 _DiffuseColor;
		sampler2D _DiffuseMap;
		sampler2D _NormalMap;
		float4 _ReflectionColor;
		samplerCUBE _ReflectionMap;
		float _ReflectionStrength;

		struct Input
		{
			float depth;
			float2 uv_DetailMap;
			float2 uv_DiffuseMap;
			float2 uv_NormalMap;
			float3 worldRefl;
			INTERNAL_DATA
		};

		void SurfaceMain(Input input, inout SurfaceOutput output)
		{
			output.Normal = UnpackNormal(tex2D(_NormalMap, input.uv_NormalMap));

			float3 reflectionColor = texCUBE(_ReflectionMap, WorldReflectionVector(input, output.Normal)).rgb * _ReflectionColor.rgb;
			float4 diffuseColor = tex2D(_DiffuseMap, input.uv_DiffuseMap) * _DiffuseColor;
			float3 finalColor = lerp(lerp(_BaseColor.rgb, diffuseColor.rgb, diffuseColor.a), reflectionColor, _ReflectionStrength);

			float3 detailMask = tex2D(_DetailMap, input.uv_DetailMap).rgb;
			float3 detailColor = lerp(_DetailColor.rgb, finalColor, detailMask);
			finalColor = lerp(detailColor, finalColor, saturate(input.depth * _DetailMapDepthBias));

			output.Albedo = finalColor;
			output.Alpha = _BaseColor.a;
		}

		void VertexMain(inout appdata_full input, out Input output)
		{
			UNITY_INITIALIZE_OUTPUT(Input, output);
			output.depth = mul(UNITY_MATRIX_MVP, input.vertex).z;
		}
		ENDCG
	}

	FallBack "VertexLit"
} 
