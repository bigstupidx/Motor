Shader "12Effect/Object/Rim ColorOnly Flow" {
	Properties {
		_Color ("Color",Color) = (1, 1, 1, 1)
		_RimColor ("Rim Color", Color) = (0.8, 0.8, 0.8, 0.6)
		_RimPower ("Rim Power", Range(0, 2)) = 0.5
		_Alpha ("Alpha", Range(0, 1)) = 1

		_FlowTex ("Flow Tex", 2D) = "black" {}
		_FlowTexColor ("Flow Tex Color", Color) = (1, 1, 1, 1)
		_FlowTexMulti ("Flow Tex Multi", Float) = 1
		_FlowSpeedX ("Flow Speed X", Float) = 1
		_FlowSpeedY ("Flow Speed Y", Float) = 0
	}

	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

		Pass {
			ZWrite Off
			Blend SrcAlpha One
			Cull Back
			Lighting Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4  _Color;
			fixed 	_RimPower;
			fixed 	_Alpha;
			fixed4 	_RimColor;
			sampler2D _FlowTex;
			fixed4 	_FlowTex_ST;
			fixed4 	_FlowTexColor;
			fixed 	_FlowTexMulti;
			fixed 	_FlowSpeedX;
			fixed 	_FlowSpeedY;

			struct a2v {
				fixed4 vertex : POSITION;
				fixed3 normal : NORMAL;
				fixed4 texcoord : TEXCOORD0;
				fixed4 color :COLOR;
			};

			struct v2f {
				fixed4 pos : POSITION;
				fixed3 rim : TEXCOORD0;
				fixed2 uvtexmove : TEXCOORD1;
				fixed4 color:COLOR;
			};

			v2f vert (a2v v) {
				v2f o;

				// Rim
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
              	o.rim = saturate(pow(1 - dot(normalize(ObjSpaceViewDir(v.vertex)), normalize(v.normal)), 0.5 / _RimPower) * _RimColor.a) * _RimColor * _RimColor.a * 10;

				// UV Move
				o.uvtexmove = TRANSFORM_TEX (v.texcoord, _FlowTex);
				o.uvtexmove.x += _FlowSpeedX * _Time;
				o.uvtexmove.y += _FlowSpeedY * _Time;
				o.color=v.color;
				return o;
			}

			fixed4 frag(v2f i) : COLOR {
				fixed4 cTexMove = tex2D (_FlowTex, i.uvtexmove);
				fixed4 c = _Color + fixed4(i.rim, 0) + cTexMove * _FlowTexColor * _FlowTexMulti;
				c.a *= _Alpha;
				c*=i.color;
				return c;
			}

			ENDCG
		}
	}
	FallBack "Mobile/Particle/Alpha Blend"
}
