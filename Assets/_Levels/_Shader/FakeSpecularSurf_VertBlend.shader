Shader "Unlit/FakeSpecularSurf-VertBlend" {
	Properties{
		_Color("Color",Color) = (1,1,1,1)
		_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess("Shininess", Range(0.01, 1)) = 0.078125
		_MainTex("Base (RGB) Gloss (A)", 2D) = "white" {}
	_MainTex2("Base (RGB) Gloss (A)", 2D) = "white" {}
	_FakeLitDir("Lit Dir",vector) = (15,4,-9,1)
		_FakeLitIntensity("Lit Intensity",float) = 1
	}

		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 1000

		CGPROGRAM
#pragma surface surf FakeBlinnPhong

		sampler2D _MainTex;
	sampler2D _MainTex2;
	half _Shininess;
	fixed4 _FakeLitDir;
	fixed4 _FakeLitColor;
	fixed _FakeLitIntensity;
	fixed4 _Color;

	inline UnityLight FakeLight() {
		UnityLight o = (UnityLight)0;
		o.dir = normalize(_FakeLitDir);
		o.color = _SpecColor;
		return o;
	}

	inline fixed4 LightingFakeBlinnPhong(SurfaceOutput s, half3 viewDir, UnityGI gi)
	{
		fixed4 c;
		c = UnityBlinnPhongLight(s, viewDir, gi.light);

		fixed intensity = _FakeLitIntensity * pow(gi.indirect.diffuse,2);
#ifdef LIGHTMAP_ON
		c += UnityBlinnPhongLight(s, viewDir, FakeLight())*intensity;
#endif

#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
		c.rgb += s.Albedo * gi.indirect.diffuse;
#endif
		//	c.rgb = gi.light.dir;
		return c;
	}

	inline void LightingFakeBlinnPhong_GI(
		SurfaceOutput s,
		UnityGIInput data,
		inout UnityGI gi)
	{
		gi = UnityGlobalIllumination(data, 1.0, s.Normal);
	}



	struct Input {
		float2 uv_MainTex;
		float2 uv_MainTex2;
		float4 color:Color;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
		fixed4 tex2 = tex2D(_MainTex2, IN.uv_MainTex2);
		o.Albedo = (tex.rgb*IN.color.r + tex2.rgb*IN.color.g) * _Color;
		o.Gloss = tex.a*IN.color.r + tex2.a*IN.color.g;
		//o.Alpha = tex.a;
		o.Specular = _Shininess;
	}
	ENDCG
	}

		Fallback "Legacy Shaders/VertexLit"
}
