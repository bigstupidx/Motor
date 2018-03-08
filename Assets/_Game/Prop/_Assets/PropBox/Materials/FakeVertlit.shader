Shader "Unlit/FakeVertlit"
{
	Properties
	{
		_FakeDirLitColor("Fake Dir Lit Color", Color) = (0.5, 0.5, 0.5, 1)
		_FakeAmbientLitColor("Fake Ambient Lit Color", Color) = (0.5, 0.5, 0.5, 1)
		_FakeLitDir("Fake Lit Dir",vector) = (15,4,-9,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				fixed3 normal : NORMAL;
			};

			struct v2f
			{
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				fixed3 color : COLOR;
			};

			fixed4 _FakeLitDir;
			fixed4 _FakeAmbientLitColor;
			fixed4 _FakeDirLitColor;

			float3 FakeShadeVertexLights(float4 vertex, float3 normal)
			{
				float3 viewpos = mul (UNITY_MATRIX_MV, vertex).xyz;
				float3 viewN = normalize (mul ((float3x3)UNITY_MATRIX_IT_MV, normal));
				float diff = max (0, dot (viewN,normalize(_FakeLitDir.xyz)));
				return _FakeAmbientLitColor + diff * _FakeDirLitColor;
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex =mul( UNITY_MATRIX_MVP, v.vertex);
				o.color = FakeShadeVertexLights(v.vertex, v.normal);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = fixed4(0,0,0,1);
				col.rgb=2*i.color;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
