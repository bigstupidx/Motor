Shader "Hidden/LTH/ImageEffects/RadialBlur" {
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
			uniform fixed4 _MainTex_TexelSize;
			uniform fixed _CenterX, _CenterY;
			uniform fixed _Strength;

			v2f_img vert(appdata_img v) {
				v2f_img o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				return o;
			}

			fixed4 frag (v2f_img i) : COLOR {
				fixed4 main = tex2D(_MainTex, i.uv);
				fixed2 center = fixed2(_CenterX, _CenterY);
				i.uv -= center;
				for(int j = 1; j < 2; j++) {
					fixed scale = 1.0 + (-_Strength * j);
					main.rgb += tex2D(_MainTex, i.uv * scale + center);
				}
				main.rgb /= 1.5;
				return main;
			}
			ENDCG
		}
	}

	Fallback off
}