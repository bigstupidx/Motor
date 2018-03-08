Shader "Unlit/Glass"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
		_Cube ("Reflection Cubemap", Cube) = "_Skybox" {}
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha 
		ZWrite Off
		
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
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float3 worldSpaceReflectionVector : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			samplerCUBE _Cube;

			fixed4 _Color;
			fixed4 _ReflectColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);

				float3 worldSpacePosition = mul(unity_ObjectToWorld, v.vertex).xyz;
				float3 worldSpaceNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
				o.worldSpaceReflectionVector = reflect(worldSpacePosition - _WorldSpaceCameraPos.xyz, worldSpaceNormal);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv)*_Color;

				float3 reflectionColor = texCUBE(_Cube, i.worldSpaceReflectionVector).rgb * _ReflectColor.rgb;
				col.rgb+=reflectionColor;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
