Shader "Hidden/RadiaBlur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};



			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			fixed _CenterX, _CenterY;
			fixed _Strength;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				
				fixed2 center = fixed2(_CenterX, _CenterY);
				fixed strength = -0.05* (abs(_CenterY - i.uv.y) + abs(_CenterX-i.uv.x ));
				i.uv -= center;

				//return fixed4(strength, strength, strength, strength);
				for (int j = 1; j < 3; j++) {
					fixed scale = 1.0 +(strength * j) ;
					col.rgb += tex2D(_MainTex, i.uv * scale + center);
				}
				col.rgb /= 3;
				return col;
			}
			ENDCG
		}
	}
}
