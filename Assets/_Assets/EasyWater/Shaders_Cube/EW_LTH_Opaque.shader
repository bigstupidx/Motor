Shader "EWater/Reflective/LTH-Opaque"
{
	Properties 
	{
		_DistortionMap1("_DistortionMap1", 2D) = "black" {}
		_DistortionMap2("_DistortionMap2", 2D) = "black" {}
		_DistortionPower("_DistortionPower", Range(0,10) ) = 0
		_Reflection("_Reflection", Cube) = "black" {}
		_ReflectPower("_ReflectPower", Range(0,1) ) = 0

			_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
			_Shininess("Shininess", Range(0.01, 1)) = 0.078125
			_FakeLitDir("Lit Dir",vector) = (15,4,-9,1)
			_FakeLitIntensity("Lit Intensity",float) = 1
	}
	
	SubShader 
	{
		Tags {"IgnoreProjector"="True" "RenderType"="Opaque"  "Queue" = "Geometry" }
	LOD 100
		
		Cull Back
		
		CGPROGRAM
		#pragma surface surf FakeBlinnPhong
		
		uniform sampler2D _DistortionMap1;
		uniform sampler2D _DistortionMap2;
		half _DistortionPower;
		samplerCUBE _Reflection;
		half _ReflectPower;

		half _Shininess;
		fixed4 _FakeLitDir;
		fixed4 _FakeLitColor;
		fixed _FakeLitIntensity;



		inline UnityLight FakeLight() {
			UnityLight o = (UnityLight)0;
			o.dir = normalize(_FakeLitDir);
			o.color = _SpecColor;
			return o;
		}

		inline fixed4 LightingFakeBlinnPhong(SurfaceOutput s, half3 viewDir, UnityGI gi)
		{
			fixed4 c;
			//c = UnityBlinnPhongLight(s, viewDir, gi.light);

			fixed intensity = _FakeLitIntensity * pow(gi.indirect.diffuse, 2);
//#ifdef LIGHTMAP_ON
			c = UnityBlinnPhongLight(s, viewDir, FakeLight())*intensity;
//#endif

#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
//			c.rgb += s.Albedo * gi.indirect.diffuse;
#endif
			//	c.rgb = gi.light.dir;
			//c.a=1000*intensity;
			//c.a=0.7f;
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
			float2 uv_DistortionMap1;
			float2 uv_DistortionMap2;
			float3 worldRefl;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			float4 DistortNormal = tex2D(_DistortionMap1, (IN.uv_DistortionMap1.xy+_Time.x*1.5))* _DistortionPower;
			float4 DistortNormal2 = tex2D(_DistortionMap2, (IN.uv_DistortionMap2.xy-_Time.x))* _DistortionPower;
			IN.worldRefl.xz += DistortNormal*10- DistortNormal2*20;
			fixed4 reflcol = texCUBE(_Reflection, IN.worldRefl);
			o.Emission = reflcol.rgb*_ReflectPower;
			o.Gloss = pow(DistortNormal.x+DistortNormal2.z,20);
			o.Albedo = fixed3(1,1,1);
			o.Specular = _Shininess;
			
		}
	ENDCG
	

	}
	Fallback "Diffuse"
}