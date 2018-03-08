// Use MaskTex's RED as mask
Shader "12Effect/Object/Rim Flow Mask" {
    Properties {
        _MainTex ("Main Tex", 2D) = "white" {}
        _FlowTex ("Flow Tex", 2D) = "black" {}
        _MaskTex ("Mask Tex", 2D) = "black" {}
        _FlowTexColor ("Flow Tex Color", Color) = (1, 1, 1, 1)
        _FlowTexMulti ("Flow Tex Multi", Float) = 1
        _FlowSpeedX ("Flow Speed X", Float) = 1
        _FlowSpeedY ("Flow Speed Y", Float) = 0
		_RimColor ("Rim Color", Color) = (0.8, 0.8, 0.8, 0.4)
		_RimPower ("Rim Power", Range(0, 2)) = 0.5
    }

    SubShader {
		Tags { "RenderType"="Opaque" }

        Pass {
			ZWrite On
            Cull Back
            Lighting Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _MainTex_ST;            
            sampler2D _FlowTex;
            fixed4 _FlowTex_ST;
            sampler2D _MaskTex;
            fixed4 _MaskTex_ST;
            fixed4 _FlowTexColor;
            fixed _FlowTexMulti;
       		fixed _FlowSpeedX;
       		fixed _FlowSpeedY;
			fixed4 _RimColor;
      		fixed _RimPower;

            struct a2v {
                fixed4 vertex : POSITION;
                fixed3 normal : NORMAL;
                fixed4 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f {
                fixed4 pos : POSITION;
                fixed2 uvmain : TEXCOORD0;
                fixed3 rim : TEXCOORD1;
                fixed2 uvmove : TEXCOORD2;
                fixed2 uvmask : TEXCOORD3;
                fixed4 color : COLOR;
            };

            v2f vert (a2v v) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uvmain = TRANSFORM_TEX (v.texcoord, _MainTex);
                o.uvmove = TRANSFORM_TEX (v.texcoord, _FlowTex);
                o.uvmask = TRANSFORM_TEX (v.texcoord, _MaskTex);

				o.uvmove.x += _FlowSpeedX * _Time;
				o.uvmove.y += _FlowSpeedY * _Time;

              	o.rim = v.color * saturate(pow(1 - dot(normalize(ObjSpaceViewDir(v.vertex)), normalize(v.normal)), 0.5 / _RimPower) * _RimColor.a) * _RimColor * _RimColor.a * 10;

                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : COLOR {
                fixed4 cMain = tex2D (_MainTex, i.uvmain);
                fixed4 cTexMove = tex2D (_FlowTex, i.uvmove);
                fixed cMask = tex2D (_MaskTex, i.uvmask).r;

                cMain.rgb += i.rim;
                cMain.rgb += cTexMove * _FlowTexColor * _FlowTexMulti * cMask;

                return cMain ;
            }
            ENDCG
        }
	}

    FallBack "Mobile/Diffuse"
}