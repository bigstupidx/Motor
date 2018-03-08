Shader "Hidden/LTH/ScreenExplode" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "" {}
        center("center(x,y)/amplitude(z)",Vector)=(0,0,0,0)
        power("power",float)=10
    }
        
        
        
    CGINCLUDE
//#pragma exclude_renderers d3d11 xbox360
        
    struct v2in {
        float4 vertex : POSITION;
        float2 texcoord  : TEXCOORD0;
    };
        
    struct v2f {
        float4 pos : POSITION;
        float2 uv  : TEXCOORD0;
    };
        
    sampler2D _MainTex;
    half4 center;
    float power;
    
        
    v2f vert( v2in v )
    {
        v2f o;
        float distance1;//顶点距离中心距离
        float distance2;//offset
        float2 tempUV;
        o.pos = mul(UNITY_MATRIX_MVP, v.vertex);



        tempUV=v.texcoord-center;
        distance1 = tempUV.x*tempUV.x + tempUV.y*tempUV.y;
//        distance1 = (center.z - distance1);
//        distance2 = saturate(0.2 - distance1);
        if(distance1<center.w/8){
        	o.uv=v.texcoord;
        }else
        if(distance1<center.w/4){
        	o.uv=v.texcoord-tempUV*power;
        }else if(distance1<center.w/2){
			o.uv=v.texcoord+tempUV*power;
		}else if(distance1<center.w){
			o.uv=v.texcoord-tempUV*power;
        }else{
        	o.uv=v.texcoord;
        }

//        o.uv*=distance1/10;
//        o.uv+=distance2;
//        o.uv=o.uv*2;
            
        return o;


    return o;
            
    }
        
    half4 frag(v2f i) : COLOR
    {
        float4 color = tex2D(_MainTex, i.uv);
        return color;
    }
    
    ENDCG
        
Subshader {
     
    Tags { "Queue" = "Transparent" }
    
 Pass {
      //ZWrite Off
      //Cull Off
      Fog { Mode off }     
      CGPROGRAM
      //#pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment frag
      ENDCG
  }
      
}
    
Fallback off
        
}