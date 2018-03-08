Shader "EWater/EasyWater11_2TBDO"
{
	Properties 
	{
	_Texture1("_Texture1", 2D) = "black" {}
	_BumpMap1("_BumpMap1", 2D) = "black" {}
	_Texture2("_Texture2", 2D) = "black" {}
	_BumpMap2("_BumpMap2", 2D) = "black" {}
	_MainTexSpeed("_MainTexSpeed", Float) = 0
	_Bump1Speed("_Bump1Speed", Float) = 0
	_Texture2Speed("_Texture2Speed", Float) = 0
	_Bump2Speed("_Bump2Speed", Float) = 0
	_DistortionMap("_DistortionMap", 2D) = "black" {}
	_DistortionSpeed("_DistortionSpeed", Float) = 0
	_DistortionPower("_DistortionPower", Range(0,0.02) ) = 0
	_Specular("_Specular", Range(0,7) ) = 1
	_Gloss("_Gloss", Range(0.3,2) ) = 0.3
	_Opacity("_Opacity", Range(-0.2,1) ) = 0

	}
	
	SubShader 
	{
		Tags
		{
		"Queue"="Transparent"
		"IgnoreProjector"="True"
		"RenderType"="Transparent"
		}

		
		Cull Back
		ZWrite On
		ZTest LEqual
		ColorMask RGBA
		Blend SrcAlpha OneMinusSrcAlpha 
		
		
		CGPROGRAM
		#pragma surface surf BlinnPhongEditor    alpha
		#pragma target 3.0
		
		
		uniform sampler2D _Texture1;
		uniform sampler2D _BumpMap1;
		uniform sampler2D _Texture2;
		uniform sampler2D _BumpMap2;
		half _MainTexSpeed;
		half _Bump1Speed;
		half _Texture2Speed;
		half _Bump2Speed;
		uniform sampler2D _DistortionMap;
		half _DistortionSpeed;
		half _DistortionPower;
		fixed _Specular;
		fixed _Gloss;
		float _Opacity;

		
		
			
		inline fixed4 LightingBlinnPhongEditor_PrePass (SurfaceOutput s, fixed4 light)
		{
			fixed3 spec = light.a * s.Gloss;
			fixed4 c;
			c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
			c.a = s.Alpha;
			return c;
		}

		inline fixed4 LightingBlinnPhongEditor (SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
		{
			fixed3 h = normalize (lightDir + viewDir);
			
			fixed diff = max (0, dot ( lightDir, s.Normal ));
			
			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, s.Specular*128.0);
			
			fixed4 res;
			res.rgb = _LightColor0.rgb * diff;
			res.w = spec * Luminance (_LightColor0.rgb);
			res *= atten * 2.0;

			return LightingBlinnPhongEditor_PrePass( s, res );
		}
		
		struct Input {
			
			float3 viewDir;
			float2 uv_DistortionMap;
			float2 uv_Texture1;
			float2 uv_Texture2;
			float2 uv_BumpMap1;
			float2 uv_BumpMap2;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			o.Normal = float3(0.0,0.0,1.0);
			o.Alpha = 1.0;
			o.Albedo = 0.0;
			o.Emission = 0.0;
			o.Gloss = 0.0;
			o.Specular = 0.0;
			
						
			// Animate distortionMap 
			float DistortSpeed=_DistortionSpeed * _Time;
			float2 DistortUV=(IN.uv_DistortionMap.xy) + DistortSpeed;
			// Create Normal for DistorionMap
			float4 DistortNormal =  tex2D(_DistortionMap,DistortUV);
			// Multiply Tex2DNormal effect by DistortionPower
			float FinalDistortion = DistortNormal.r * _DistortionPower;
			
					
			// Animate MainTex
			float Multiply2=_Time * _MainTexSpeed;
			float2 MainTexUV=(IN.uv_Texture1.xy  + FinalDistortion) + Multiply2; 
			
			// Apply Distorion in MainTex
			float4 Tex2D0=tex2D(_Texture1,MainTexUV);
			
			// Animate Texture2
			float Multiply3=_Time * _Texture2Speed;
			float2 Tex2UV=(IN.uv_Texture2.xy  - FinalDistortion) + Multiply3;
			
			// Apply Distorion in Texture2
			// float2 Add1=Tex2UV + FinalDistortion;
			float4 Tex2D1=tex2D(_Texture2,Tex2UV); 
			
			// Merge MainTex and Texture2
			float4 FinalDiffuse =Tex2D0 * Tex2D1;			
			
					
			
			
			// Animate BumpMap1
			float Multiply8=_Time * _Bump1Speed;
			float2 Bump1UV=(IN.uv_BumpMap1.xy  + FinalDistortion) + Multiply8;
			// Apply Distortion to BumpMap
			// float2 Add2=Bump1UV + FinalDistortion;
			half4 Tex2D3=tex2D(_BumpMap1,Bump1UV);
			
			// Animate BumpMap2
			half Multiply9=_Time * _Bump2Speed;
			half2 Bump2UV=(IN.uv_BumpMap2.xy  + FinalDistortion) + Multiply9;
			// Apply Distortion to BumpMap2
			//float2 Add3=Bump2UV + FinalDistortion;
			
			fixed4 Tex2D4=tex2D(_BumpMap2,Bump2UV);
			
			// Get Average from BumpMap1 and BumpMap2
			fixed4 AvgBump= (Tex2D3 + Tex2D4) / 2;
			
			// Unpack Normals
			fixed4 UnpackNormal1=float4(UnpackNormal(AvgBump).xyz, 1.0);
			
		
			float3 alphaByView =  dot(normalize(IN.viewDir),o.Normal) ;
			
			
			o.Albedo = FinalDiffuse;
			o.Normal = UnpackNormal1;
			o.Specular = _Gloss;
			o.Gloss = _Specular;
			o.Alpha = (2 * _Opacity) - alphaByView;

			o.Normal = normalize(o.Normal);
		}
	ENDCG
	}
	Fallback "Diffuse"
}