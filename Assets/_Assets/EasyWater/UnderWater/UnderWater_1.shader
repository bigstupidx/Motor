// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "EWater/UnderWater_1"
{
	Properties 
	{
	_Color("_Color", Color) = (1,1,1,1)
	_DistortionMap1("_DistortionMap1", 2D) = "black" {}
	_DistortionMap2("_DistortionMap2", 2D) = "black" {}
	_DistortionPower("_DistortionPower", Range(0,0.02) ) = 0
	_Specular("_Specular", Range(0,7) ) = 1
	_Gloss("_Gloss", Range(0.3,10) ) = 0.3
	_Opacity("_Opacity", Range(-0.1,1.5) ) = 0
	_Reflection("_Reflection", Cube) = "black" {}
	_ReflectPower("_ReflectPower", Range(0,1) ) = 0

	}
	
	SubShader 
	{
		Tags
		{
			"Queue"="Transparent-1"
			"IgnoreProjector"="False"
			"RenderType"="Transparent"
		}

		
		Cull Back
		ZWrite On
		ZTest LEqual
		ColorMask RGBA
		Blend SrcAlpha OneMinusSrcAlpha
		Fog{
		}
		
		
		CGPROGRAM
		#pragma surface surf Underwater
		#pragma target 2.0
		
		
		fixed4 _Color;
		uniform sampler2D _DistortionMap1;
		uniform sampler2D _DistortionMap2;
		half _DistortionPower;
		fixed _Specular;
		fixed _Gloss;
		float _Opacity;
		

		half4 LightingUnderwater (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
		
			lightDir.z = 1-lightDir.z;
          half3 h = normalize (lightDir + viewDir);
          half diff = max (0, dot (s.Normal, lightDir));
          float nh = max (0, dot (s.Normal, h));
          float spec = pow (nh, s.Specular*256.0)* s.Gloss;
          half4 c;
          c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * (atten * 2);
          c.a = s.Alpha;
          return c;
      }
				
		
		struct Input {
			float3 viewDir;
			float2 uv_DistortionMap1;
			float2 uv_DistortionMap2;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			o.Normal = float3(0.0,0.0,1.0);
			
			float2 DistortUV=(IN.uv_DistortionMap1.xy);
			float4 DistortNormal1 = tex2D(_DistortionMap1,DistortUV);
			// Multiply Tex2DNormal effect by DistortionPower
			float2 FinalDistortion = DistortNormal1 * _DistortionPower;
			
			
			
			
			// Merge Textures, Texture and Color
			float4 FinalDiffuse = _Color; 			
			
			
			// Animate DistortionMap1
			float2 Bump1UV=(IN.uv_DistortionMap1.xy) ;
			
			// Apply Distortion to BumpMap 
			half4 DistortedDistortionMap1=tex2D(_DistortionMap1,Bump1UV + FinalDistortion);			
			
			half2 Bump2UV=(IN.uv_DistortionMap2.xy);
			
			// Apply Distortion to DistortionMap2			
			fixed4 DistortedDistortionMap2=tex2D(_DistortionMap2,Bump2UV + FinalDistortion);
			
			// Get Average from DistortionMap1 and DistortionMap2
			fixed4 AvgBump= (DistortedDistortionMap1 + DistortedDistortionMap2) / 2;
			
			// Unpack Normals
			fixed4 UnpackNormal1=float4(UnpackNormal(AvgBump).xyz, 1.0);
			
			// Fresnel
			float fresnel = 1.0 - saturate(dot ( o.Normal, normalize(IN.viewDir))); 
			
			float FinalAlpha = _Opacity  ;		 	
			o.Albedo = FinalDiffuse;
			o.Normal = UnpackNormal1;
			//o.Emission = FinalReflex;
			o.Specular = _Gloss;
			o.Gloss = _Specular;
			o.Alpha = FinalAlpha;

			o.Normal = normalize(o.Normal);
		}
	ENDCG


	Pass{
            Tags {
                "Queue"="Transparent"  "RenderType" = "Transparent" }
Blend  SrcAlpha OneMinusSrcAlpha

CGPROGRAM

        #pragma vertex vert
        #pragma fragment frag alpha
        #include "UnityCG.cginc" 

            samplerCUBE _Reflection;
            float _ReflectPower;
            uniform sampler2D _DistortionMap1;
            uniform sampler2D _DistortionMap2;
            float4 _DistortionMap1_ST;
            float4 _DistortionMap2_ST;
            fixed _DistortionPower;
            
		float _Opacity;


            struct v2f {
                float4 pos : POSITION;
                float2 distort1UV : TEXCOORD2;
                float2 distort2UV : TEXCOORD3;
                float3 I : TEXCOORD1;
                float3 viewDir : TEXCOORD4;
                float3 normal  : TEXCOORD5;
            }; 

            v2f vert( appdata_full v ) {
                v2f o;
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                o.distort1UV = TRANSFORM_TEX (v.texcoord, _DistortionMap1);
                
                o.distort2UV = TRANSFORM_TEX (v.texcoord, _DistortionMap2);
                // calculate object space reflection vector	
			   o.viewDir = ObjSpaceViewDir( v.vertex );   
			   
                float3 I = reflect( float3(-o.viewDir.x,o.viewDir.y,-o.viewDir.z), v.normal );
                // transform to world space reflection vector
				o.I = mul( (float3x3)unity_ObjectToWorld, I ); 
				o.normal = v.normal; 
                return o;
            }

        
        half4 frag( v2f IN ) : COLOR {
        		
        		float4 DistortNormal1 =  tex2D(_DistortionMap1,IN.distort1UV  ); 
        		float4 DistortNormal2 =  tex2D(_DistortionMap2,IN.distort2UV ); 
        		float FinalDistortion = (DistortNormal1.x - DistortNormal2.x) * _DistortionPower * 200;
        		
                
                half4 reflection = texCUBE( _Reflection, IN.I +  FinalDistortion);  
                
                
				float fresnel  = 1 - saturate ( dot ( IN.normal, normalize(IN.viewDir )) ); 
				
				
                reflection.a =  _ReflectPower * _Opacity * reflection; 
                return reflection; 
            }   
        ENDCG
}

	}
	Fallback "Diffuse"
}