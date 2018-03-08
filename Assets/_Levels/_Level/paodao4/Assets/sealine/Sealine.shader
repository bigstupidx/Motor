Shader "Unlit/Sealine"
{
	Properties
	{
		_Color ("Color", Color)=(1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_MainTex2 ("Texture", 2D) = "white" {}
		_Noise ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 

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
				float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				UNITY_FOG_COORDS(2)
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _MainTex2;
			float4 _MainTex2_ST;
			sampler2D _Noise;
			float4 _Noise_ST;
			
			fixed4 _Color;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				o.color=v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col ;
				
				fixed4 noiseTex=tex2D(_Noise,i.uv+_Time.x);
				noiseTex*=0.2f;
				
				fixed2 uv1=i.uv+fixed2(0,-_Time.y*0.4f)+noiseTex.rg;
				fixed4 tex1= tex2D(_MainTex, uv1);
				
				fixed2 uv2=i.uv+fixed2(0,-_Time.y*0.4f+0.5f)+noiseTex.gr;
				fixed4 tex2= tex2D(_MainTex2, uv2);
				
				col=tex1+tex2;
				
				
				UNITY_APPLY_FOG(i.fogCoord, col);
				col.a*=i.color.a;
				col*=_Color;
				return col;
			}
			ENDCG
		}
	}
}
