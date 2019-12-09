Shader "IFramework/SimpleShader01"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
      //  Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

         
            struct v2f
            {
                float4 position : POSITION;
				float4 pos : TEXCOORD0; 
            };

            v2f vert (appdata_base v)
            {
                v2f o;
				v.vertex.y *=sin(_Time.w * 2)/2+0.5 ;
				v.vertex.z *= cos(_Time.x * 20) / 2 + 0.5;
                o.position = UnityObjectToClipPos(v.vertex);
				o.pos = v.vertex;
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag (v2f o) :COLOR
            {
			   return fixed4(o.pos.xy,o.pos.z*_SinTime.w,1);
            }
            ENDCG
        }
    }
}
