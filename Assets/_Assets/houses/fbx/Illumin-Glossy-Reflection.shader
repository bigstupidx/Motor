Shader "Illumin-Specular-Reflection" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_Gloss("Gloss", Range(0.01, 1)) = 0.078125
	_MainTex ("Base (RGB) Reflection (A)", 2D) = "white" {}
		_ReflectColor("Reflection Color", Color) = (1,1,1,0.5)
		_Cube("Reflection Cubemap", Cube) = "_Skybox" {}
	_Illum("Illumin (A)", 2D) = "white" {}
	_Emission ("Emission (Lightmapper)", Range (0.0, 8.0)) = 1.0

}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 300
	
CGPROGRAM
#pragma surface surf BlinnPhong

sampler2D _MainTex;
sampler2D _Illum;
samplerCUBE _Cube;
fixed4 _ReflectColor;
fixed4 _Color;
half _Shininess;
half _Gloss;
fixed _Emission;


struct Input {
	float2 uv_MainTex;
	float2 uv_Illum;
	float3 worldRefl;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 c = tex * _Color;
	o.Albedo = c.rgb;
	fixed4 reflcol = texCUBE(_Cube, IN.worldRefl);
	reflcol *= c.a;
	o.Emission = //
		reflcol.rgb*_ReflectColor.rgb+ c.rgb * tex2D(_Illum, IN.uv_Illum).a;
#if defined (UNITY_PASS_META)
	o.Emission *= _Emission.rrr;
#endif



	o.Gloss = _Gloss;
	o.Alpha = c.a;
	o.Specular = _Shininess;
}
ENDCG
}
FallBack "Legacy Shaders/Self-Illumin/Diffuse"
CustomEditor "LegacyIlluminShaderGUI"
}
