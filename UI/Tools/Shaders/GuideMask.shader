Shader "Hidden/GuideMask"
{
    Properties
    {
        _Rect("Rect",vector)=(0,0,0,0)
        _BackgroundColor("_BackgroundColor",Color)=(0,0,0,0.3)
        _FrontColor("_FrontColor",Color)=(0,0,0,0.2)
        _Radian("_Radian",Range(0,1))=0.5
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            Tags{"Queue"="Transparent"}
            Blend SrcAlpha OneMinusSrcAlpha
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
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 _Rect;
            fixed4 _BackgroundColor;
            fixed4 _FrontColor;
            float _Radian;
            fixed InCircle(float2 center,float radius,float2 uv)
            {
                return step(radius,distance(center,uv));
            }

            float Fit(float2 pos)
            {
                pos=pos*_ScreenParams.xy;
                float2 halfSize= _Rect.zw/2;
                float halfWidth=halfSize.x;
                float halfHeight=halfSize.y;
                float _min= min(halfWidth,halfHeight);
                float radius = _min*_Radian;

                float2 horizontalsize=halfSize-float2(radius,0);
                float2 verticalsize=halfSize-float2(0,radius);
                fixed2 h=step(_Rect.xy-horizontalsize,pos)*step(pos,_Rect.xy+horizontalsize);
                fixed2 v=step(_Rect.xy-verticalsize,pos)*step(pos,_Rect.xy+verticalsize);
                float _r=max(h.x*h.y,v.x*v.y);

                float2 center=_Rect.xy;
                float witdhBigger=step(halfHeight,halfWidth);
                float heightBigger=1-witdhBigger;
                float _add=_min*(1-_Radian);
                float gap= max(halfWidth,halfHeight)-_min;

                float2 lu=center+float2(-gap*witdhBigger,gap*heightBigger)+float2(-_add,_add);
                float2 ld=center+float2(-gap*witdhBigger,-gap*heightBigger)+float2(-_add,-_add);
                float2 ru=center+float2(gap*witdhBigger,gap*heightBigger)+float2(_add,_add);
                float2 rd=center+float2(gap*witdhBigger,-gap*heightBigger)+float2(_add,-_add);

                float _c= InCircle(lu,radius,pos)*InCircle(ru,radius,pos)*InCircle(ld,radius,pos)*InCircle(rd,radius,pos);
                return max(_r,1-_c);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return lerp(_BackgroundColor,_FrontColor,Fit(i.uv));
            }
            ENDCG
        }
    }
}
