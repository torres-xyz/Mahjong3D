Shader "Skybox Gradient"
{
	Properties
	{
		_Top("Top", Color) = (1,1,1,0)
		_Bottom("Bottom", Color) = (0,0,0,0)
		_mult("mult", Float) = 1
		_pwer("pwer", Float) = 1
		[Toggle(_SCREENSPACE_ON)] _Screenspace("Screen space", Float) = 0
	}
	
	SubShader
	{		
		Tags { "RenderType"="Opaque" }
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#pragma shader_feature_local _SCREENSPACE_ON

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
			};

			uniform float4 _Bottom;
			uniform float4 _Top;
			uniform float _mult;
			uniform float _pwer;
			
			v2f vert ( appdata v )
			{
				v2f o;

				float4 clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(clipPos);
				o.texcoord2 = screenPos;
				
				o.texcoord1 = v.vertex;
				float3 vertexValue = float3(0, 0, 0);
				
				v.vertex.xyz += vertexValue;
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				fixed4 finalColor;
				float4 screenPos = i.texcoord2;
				float4 screenPosNorm = screenPos / screenPos.w;
				screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? screenPosNorm.z : screenPosNorm.z * 0.5 + 0.5;

				#ifdef _SCREENSPACE_ON
					float direction = screenPosNorm.y;
				#else
					float direction = i.texcoord1.xyz.y;
				#endif

				finalColor = lerp( _Bottom , _Top , pow( saturate( ( direction * _mult ) ) , _pwer ));
				return finalColor;
			}
			ENDCG
		}
	}	
}