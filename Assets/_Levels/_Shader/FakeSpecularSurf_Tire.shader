Shader "Unlit/FakeSpecularSurf_Tire" {
	Properties{
		_Color("Color",Color)=(1,1,1,1)
		_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess("Shininess", Range(0.01, 1)) = 0.078125
		_MainTex("Base (RGB) Gloss (Length(RGB))", 2D) = "white" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		_FakeLitDir("Lit Dir",vector) = (15,4,-9,1)
		_FakeLitIntensity("Lit Intensity",float) = 1
			_Ambient("Ambient",Color) = (1,1,1,1)
	}

		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 300

		CGPROGRAM
#pragma surface surf FakeBlinnPhong

		sampler2D _MainTex;
		sampler2D _BumpMap;
	half _Shininess;
	fixed4 _FakeLitDir;
	fixed4 _FakeLitColor;
	fixed _FakeLitIntensity;
	fixed4 _Color;
	fixed4 _Ambient;

	inline UnityLight FakeLight() {
		UnityLight o = (UnityLight)0;
		o.dir = normalize(_FakeLitDir);
		o.color = _SpecColor;
		return o;
	}

	inline fixed4 LightingFakeBlinnPhong(SurfaceOutput s, half3 viewDir, UnityGI gi)
	{
		fixed4 c;
		c = UnityBlinnPhongLight(s, viewDir, FakeLight())*_FakeLitIntensity;

		c.rgb += s.Albedo *_Ambient;// gi.indirect.diffuse;
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
		float2 uv_BumpMap;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = tex.rgb*_Color;
		o.Gloss = length(tex.rgb);
		o.Alpha = tex.a;
		o.Specular = _Shininess;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	}
	ENDCG
	}

		Fallback "Legacy Shaders/VertexLit"
}
