Shader "Hidden/SimpleLuminance" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader {
		Pass {
			Tags { "LightMode" = "Always" }
			ZTest Always Cull Off ZWrite Off Fog { Mode off }

			CGPROGRAM
			#pragma exclude_renderers xbox360 ps3 flash
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_TexelSize;
			fixed _Saturation;
			fixed _R;
			fixed _G;
			fixed _B;

			v2f_img vert(appdata_img v) {
				v2f_img o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
#if SHADER_API_D3D9
					if (_MainTex_TexelSize.y < 0)
						v.texcoord.y = 1.0 - v.texcoord.y;
#endif
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				return o;
			}

			fixed4 frag (v2f_img i) : COLOR {
				fixed4 col =tex2D(_MainTex,i.uv);
				fixed lum = dot(col, fixed3(_R, _G, _B) );;
				col.rgb = lerp(fixed3(lum,lum,lum), col.rgb,_Saturation);
				return col;
			}
			ENDCG
		}
	}

	Fallback off
}
