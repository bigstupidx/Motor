Shader "SkillEff/ParticalAlphablendMono" {
Properties {
	_Color ("Tint Color", Color) = (1, 1, 1, 1)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_Power ("Power", float) = 1
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	
	SubShader {
		Pass {		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_particles

			#include "UnityCG.cginc"

			sampler2D _MainTex;
            fixed4 _MainTex_ST;
			fixed _Power;
			fixed4 _Color;
			
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

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color * _Color;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{				
				fixed4 c = fixed4(1, 1, 1, 1);
				fixed4 ct = tex2D(_MainTex, i.texcoord);
				c.a *= length(ct.rgb) * _Power;
				c *= i.color;
				
				return c;
			}
			ENDCG 
		}
	}	
}
}
