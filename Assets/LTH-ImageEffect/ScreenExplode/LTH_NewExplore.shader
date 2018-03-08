Shader "LTH/InScreenExplode" {
        Properties {
            _MainTex ("Base (RGB)", 2D) = "white" {}
            _RPQ("R,P,Q",Vector)=(0.5,1,2,0)
            _Center("center(x,y)",Vector)=(0.5,0.5,0,0)
        }
        SubShader {
            Tags {"RenderType"="Opaque"}
            LOD 100

            Pass {
                Cull Back
                Lighting On
                CGPROGRAM
//#pragma exclude_renderers d3d11 xbox360
                #pragma vertex vert
                #pragma fragment frag
     
                #include "UnityCG.cginc"
     
                sampler2D _MainTex;
                fixed4 _MainTex_ST;
                fixed4 _RPQ;
                fixed4 _Center;

                struct a2v
                {
                    fixed4 vertex : POSITION;
                    fixed2 texcoord : TEXCOORD0;

                };
     
                struct v2f
                {
                    fixed4 pos : POSITION;
                    fixed2 uv :TEXCOORD0;
                    fixed3 mc :TEXCOORD1;
                };
     
                v2f vert (a2v v)
                {
                    v2f o;
                    o.pos = mul( UNITY_MATRIX_MVP, v.vertex);
                    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);  
                    o.mc = fixed3( _RPQ.x * ( 2 * o.uv - 1 + _Center), _RPQ.y );
                    return o;
                }
     
                fixed4 frag(v2f i) : COLOR
                {
                    fixed4 c;
					  float off =_RPQ.z* exp( -dot( i.mc.xy, i.mc.xy ));
					  fixed2 newUV= i.uv - off * normalize( i.mc).xy;
                    c = tex2D (_MainTex, newUV);
   
                    return c;
     
                }
     
                ENDCG
            }

}

}
