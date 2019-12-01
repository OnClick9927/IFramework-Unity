// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:33191,y:32704,varname:node_9361,prsc:2|custl-5513-OUT,alpha-745-A;n:type:ShaderForge.SFN_Tex2d,id:745,x:32710,y:32988,ptovrint:False,ptlb:MainTexture,ptin:_MainTexture,varname:node_745,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Time,id:2219,x:30687,y:32827,varname:node_2219,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6429,x:30907,y:32791,varname:node_6429,prsc:2|A-6744-OUT,B-2219-T;n:type:ShaderForge.SFN_TexCoord,id:8474,x:30687,y:32358,varname:node_8474,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:6115,x:32738,y:32825,varname:node_6115,prsc:2|A-8809-OUT,B-6160-RGB;n:type:ShaderForge.SFN_Color,id:6160,x:32477,y:32916,ptovrint:False,ptlb:FlowColor,ptin:_FlowColor,varname:node_6160,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Add,id:5513,x:32936,y:32903,varname:node_5513,prsc:2|A-6115-OUT,B-745-RGB;n:type:ShaderForge.SFN_ValueProperty,id:6744,x:30687,y:32744,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_6744,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Subtract,id:7761,x:31817,y:32643,varname:node_7761,prsc:2|A-3973-R,B-5658-OUT;n:type:ShaderForge.SFN_Vector1,id:5658,x:31552,y:32789,varname:node_5658,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Abs,id:7538,x:32024,y:32643,varname:node_7538,prsc:2|IN-7761-OUT;n:type:ShaderForge.SFN_OneMinus,id:5549,x:32204,y:32643,varname:node_5549,prsc:2|IN-7538-OUT;n:type:ShaderForge.SFN_Power,id:8809,x:32455,y:32716,varname:node_8809,prsc:2|VAL-5549-OUT,EXP-2376-OUT;n:type:ShaderForge.SFN_Panner,id:6654,x:31133,y:32588,varname:node_6654,prsc:2,spu:1,spv:0|UVIN-1934-UVOUT,DIST-6429-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2376,x:32204,y:32814,ptovrint:False,ptlb:Range,ptin:_Range,varname:node_2376,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_ComponentMask,id:3973,x:31524,y:32588,varname:node_3973,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-1567-OUT;n:type:ShaderForge.SFN_Frac,id:1567,x:31335,y:32588,varname:node_1567,prsc:2|IN-6654-UVOUT;n:type:ShaderForge.SFN_Rotator,id:1934,x:30923,y:32426,varname:node_1934,prsc:2|UVIN-8474-UVOUT,ANG-7541-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8445,x:30287,y:32424,ptovrint:False,ptlb:Angle,ptin:_Angle,varname:node_8445,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Divide,id:4530,x:30479,y:32478,varname:node_4530,prsc:2|A-8445-OUT,B-2035-OUT;n:type:ShaderForge.SFN_Vector1,id:2035,x:30287,y:32558,varname:node_2035,prsc:2,v1:180;n:type:ShaderForge.SFN_Multiply,id:7541,x:30687,y:32554,varname:node_7541,prsc:2|A-4530-OUT,B-2359-OUT;n:type:ShaderForge.SFN_NormalVector,id:3028,x:32458,y:33234,prsc:2,pt:False;n:type:ShaderForge.SFN_Multiply,id:2963,x:32790,y:33201,varname:node_2963,prsc:2|A-8809-OUT,B-3028-OUT,C-2567-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2567,x:32498,y:33464,ptovrint:False,ptlb:VertexOffsetIntensity,ptin:_VertexOffsetIntensity,varname:node_2567,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Pi,id:2359,x:30479,y:32627,varname:node_2359,prsc:2;proporder:745-6160-2376-6744-8445-2567;pass:END;sub:END;*/

Shader "Customed/FlowColor" {
    Properties {
        _MainTexture ("MainTexture", 2D) = "white" {}
        _FlowColor ("FlowColor", Color) = (0.5,0.5,0.5,1)
        _Range ("Range", Float ) = 10
        _Speed ("Speed", Float ) = 1
        _Angle ("Angle", Float ) = 0
        _VertexOffsetIntensity ("VertexOffsetIntensity", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
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
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTexture; uniform float4 _MainTexture_ST;
            uniform float4 _FlowColor;
            uniform float _Speed;
            uniform float _Range;
            uniform float _Angle;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
                float4 node_2219 = _Time + _TimeEditor;
                float node_1934_ang = ((_Angle/180.0)*3.141592654);
                float node_1934_spd = 1.0;
                float node_1934_cos = cos(node_1934_spd*node_1934_ang);
                float node_1934_sin = sin(node_1934_spd*node_1934_ang);
                float2 node_1934_piv = float2(0.5,0.5);
                float2 node_1934 = (mul(i.uv0-node_1934_piv,float2x2( node_1934_cos, -node_1934_sin, node_1934_sin, node_1934_cos))+node_1934_piv);
                float node_8809 = pow((1.0 - abs((frac((node_1934+(_Speed*node_2219.g)*float2(1,0))).rg.r-0.5))),_Range);
                float4 _MainTexture_var = tex2D(_MainTexture,TRANSFORM_TEX(i.uv0, _MainTexture));
                float3 finalColor = ((node_8809*_FlowColor.rgb)+_MainTexture_var.rgb);
                fixed4 finalRGBA = fixed4(finalColor,_MainTexture_var.a);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
