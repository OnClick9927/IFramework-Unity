// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/NewUnlitShader"
{
	Properties
	{
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			void Func(out float4 color)
			{
				color = float4(0,1,1,0);
			}
			float4 vert (in float3 p:POSITION,out float4 pp:POSITION) :COLOR
			{
				pp = UnityObjectToClipPos(  p ) ;
				return pp;
			}
			
			fixed4 frag (in fixed4 color:COLOR):COLOR
			{
				Func(color);
				return color;

			}
			ENDCG
		}
	}
}
