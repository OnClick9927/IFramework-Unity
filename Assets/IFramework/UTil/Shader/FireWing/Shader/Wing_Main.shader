// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:33156,y:32917,varname:node_4795,prsc:2|emission-2393-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:32043,y:32832,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False|UVIN-4518-UVOUT;n:type:ShaderForge.SFN_Multiply,id:2393,x:32914,y:33017,varname:node_2393,prsc:2|A-4693-OUT,B-797-RGB,C-9248-OUT,D-2053-RGB;n:type:ShaderForge.SFN_VertexColor,id:2053,x:32600,y:32955,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:32600,y:33113,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:9248,x:32600,y:33264,varname:node_9248,prsc:2,v1:2;n:type:ShaderForge.SFN_Tex2d,id:4769,x:32374,y:32363,ptovrint:False,ptlb:SubTex,ptin:_SubTex,varname:_node_4769,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:21672833b40585546bbe38821c1a67de,ntxv:0,isnm:False|UVIN-6021-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:8466,x:32498,y:32783,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:_alpha,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:55dc59ed2ddca9f4f89077b8e7028f3f,ntxv:0,isnm:False|UVIN-2296-UVOUT;n:type:ShaderForge.SFN_Multiply,id:4693,x:32781,y:32726,varname:node_4693,prsc:2|A-402-OUT,B-8466-A;n:type:ShaderForge.SFN_Multiply,id:402,x:32674,y:32475,varname:node_402,prsc:2|A-4769-RGB,B-803-OUT;n:type:ShaderForge.SFN_Slider,id:803,x:32318,y:32640,ptovrint:False,ptlb:Transparency,ptin:_Transparency,varname:_node_803,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3.971232,max:6;n:type:ShaderForge.SFN_TexCoord,id:4972,x:32053,y:32644,varname:node_4972,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:2296,x:32283,y:32802,varname:node_2296,prsc:2,spu:0,spv:0.1|UVIN-4972-UVOUT,DIST-6074-R;n:type:ShaderForge.SFN_Panner,id:4518,x:31851,y:32833,varname:node_4518,prsc:2,spu:-0.3,spv:-0.3|UVIN-9151-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:9151,x:31665,y:32833,varname:node_9151,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:6021,x:32218,y:32375,varname:node_6021,prsc:2,spu:0,spv:0.4|UVIN-7719-UVOUT,DIST-7195-R;n:type:ShaderForge.SFN_TexCoord,id:9405,x:31687,y:32228,varname:node_9405,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:7195,x:32035,y:32417,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:_MainTex_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:5566d690d0bdb454c885259313b76333,ntxv:2,isnm:False|UVIN-1412-UVOUT;n:type:ShaderForge.SFN_Panner,id:1412,x:31871,y:32417,varname:node_1412,prsc:2,spu:-0.8,spv:-0.8|UVIN-1291-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:1291,x:31703,y:32410,varname:node_1291,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:7719,x:31871,y:32228,varname:node_7719,prsc:2,spu:-0.3,spv:-0.3|UVIN-9405-UVOUT;n:type:ShaderForge.SFN_Slider,id:6480,x:33772,y:32944,ptovrint:False,ptlb:Refraction Intensity,ptin:_RefractionIntensity,varname:_RefractionIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:8977,x:34337,y:32916,varname:node_8977,prsc:2|A-7767-OUT,B-2633-OUT;n:type:ShaderForge.SFN_ComponentMask,id:7767,x:34167,y:32843,varname:node_7767,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-3967-RGB;n:type:ShaderForge.SFN_Vector1,id:6486,x:34337,y:32843,varname:node_6486,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Tex2d,id:3967,x:33929,y:32758,ptovrint:False,ptlb:Refraction,ptin:_Refraction,varname:_Refraction,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:bbab0a6f7bae9cf42bf057d8ee2755f6,ntxv:3,isnm:True;n:type:ShaderForge.SFN_TexCoord,id:4509,x:33571,y:32697,varname:node_4509,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:3775,x:33742,y:32758,varname:node_3775,prsc:2|A-4509-UVOUT,B-8217-OUT;n:type:ShaderForge.SFN_Vector1,id:8217,x:33571,y:32854,varname:node_8217,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:1684,x:34337,y:32769,varname:node_1684,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:6005,x:34337,y:32622,varname:node_6005,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:5011,x:34337,y:32678,varname:node_5011,prsc:2,v1:0.8;n:type:ShaderForge.SFN_Lerp,id:6058,x:34167,y:32716,varname:node_6058,prsc:2|A-7186-OUT,B-3967-RGB,T-6480-OUT;n:type:ShaderForge.SFN_Vector3,id:7186,x:33929,y:32637,varname:node_7186,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Multiply,id:2633,x:34167,y:32995,varname:node_2633,prsc:2|A-6480-OUT,B-6919-OUT;n:type:ShaderForge.SFN_Vector1,id:6919,x:33929,y:33023,varname:node_6919,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Vector1,id:8834,x:34337,y:32558,varname:node_8834,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Slider,id:8291,x:33836,y:33008,ptovrint:False,ptlb:Refraction Intensity_copy,ptin:_RefractionIntensity_copy,varname:_RefractionIntensity_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:5309,x:34401,y:32980,varname:node_5309,prsc:2|A-1288-OUT,B-8636-OUT;n:type:ShaderForge.SFN_ComponentMask,id:1288,x:34231,y:32907,varname:node_1288,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-2233-RGB;n:type:ShaderForge.SFN_Vector1,id:9602,x:34401,y:32907,varname:node_9602,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Tex2d,id:2233,x:33993,y:32822,ptovrint:False,ptlb:Refraction_copy,ptin:_Refraction_copy,varname:_Refraction_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:bbab0a6f7bae9cf42bf057d8ee2755f6,ntxv:3,isnm:True;n:type:ShaderForge.SFN_TexCoord,id:2823,x:33635,y:32761,varname:node_2823,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:8482,x:33806,y:32822,varname:node_8482,prsc:2|A-2823-UVOUT,B-3313-OUT;n:type:ShaderForge.SFN_Vector1,id:3313,x:33635,y:32918,varname:node_3313,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:474,x:34401,y:32833,varname:node_474,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:1177,x:34401,y:32686,varname:node_1177,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:4848,x:34401,y:32742,varname:node_4848,prsc:2,v1:0.8;n:type:ShaderForge.SFN_Lerp,id:3295,x:34231,y:32780,varname:node_3295,prsc:2|A-1100-OUT,B-2233-RGB,T-8291-OUT;n:type:ShaderForge.SFN_Vector3,id:1100,x:33993,y:32701,varname:node_1100,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Multiply,id:8636,x:34231,y:33059,varname:node_8636,prsc:2|A-8291-OUT,B-2678-OUT;n:type:ShaderForge.SFN_Vector1,id:2678,x:33993,y:33087,varname:node_2678,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Vector1,id:8860,x:34401,y:32622,varname:node_8860,prsc:2,v1:0.2;proporder:6074-797-4769-8466-803-7195;pass:END;sub:END;*/

Shader "FireWing/Wing_Main" {
    Properties {
        _MainTex ("MainTex", 2D) = "black" {}
        _TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _SubTex ("SubTex", 2D) = "white" {}
        _Mask ("Mask", 2D) = "white" {}
        _Transparency ("Transparency", Range(0, 6)) = 3.971232
        _Noise ("Noise", 2D) = "black" {}
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles n3ds wiiu 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform sampler2D _SubTex; uniform float4 _SubTex_ST;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _Transparency;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 node_1814 = _Time + _TimeEditor;
                float2 node_1412 = (i.uv0+node_1814.g*float2(-0.8,-0.8));
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_1412, _Noise));
                float2 node_6021 = ((i.uv0+node_1814.g*float2(-0.3,-0.3))+_Noise_var.r*float2(0,0.4));
                float4 _SubTex_var = tex2D(_SubTex,TRANSFORM_TEX(node_6021, _SubTex));
                float2 node_4518 = (i.uv0+node_1814.g*float2(-0.3,-0.3));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4518, _MainTex));
                float2 node_2296 = (i.uv0+_MainTex_var.r*float2(0,0.1));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(node_2296, _Mask));
                float3 emissive = (((_SubTex_var.rgb*_Transparency)*_Mask_var.a)*_TintColor.rgb*2.0*i.vertexColor.rgb);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0,0,0,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
