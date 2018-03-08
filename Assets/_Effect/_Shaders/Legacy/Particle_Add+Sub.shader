Shader "LTH/Unity/Particles/Additive+RevSub" {
	Properties {
		_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_LightMulti("LightMulti", float) = 1
		_MaxAlpha("MaxAlpha", int) = 1
	}

	Category {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
		
		SubShader {		
			Pass {		
				Blend SrcAlpha One
				BlendOp RevSub	
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
	                fixed3 normal : NORMAL;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};
				
				float4 _MainTex_ST;
				int _MaxAlpha;

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.color = v.color * _Color;
					o.color.rgb = 1 - o.color.rgb;
					o.color.a *= max(length(v.color.rgb), _MaxAlpha);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{				
					fixed4 col = i.color  * tex2D(_MainTex, i.texcoord) * _LightMulti;
					return col;
				}
				ENDCG 
			}

			Pass {		
				Blend SrcAlpha One
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
	                fixed3 normal : NORMAL;
				};

				struct v2f {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};
				
				float4 _MainTex_ST;
				int _MaxAlpha;

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.color = v.color * _Color;
					o.color.a *= max(length(v.color.rgb), _MaxAlpha);
					o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					return  i.color * tex2D(_MainTex, i.texcoord) * _LightMulti;
				}
				ENDCG 
			}
		}
	}
}