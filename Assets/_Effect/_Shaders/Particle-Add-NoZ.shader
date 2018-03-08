Shader "LTH/Unity/Particles/Additive-Color-NoZ" {
Properties {
_Color("Color",Color)=(1,1,1,1)
	_MainTex ("Particle Texture", 2D) = "white" {}
_LightMulti("LightMulti",float)=1
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	// Blend SrcAlpha One
	Blend SrcAlpha One
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) } ZTest Always
	
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_particles

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _Color;
			fixed _LightMulti;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color*_Color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				return  i.color  * tex2D(_MainTex, i.texcoord)*_LightMulti;
			}
			ENDCG 
		}
	}	
}
}
