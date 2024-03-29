Shader "Portals/Portal"
{
  //  Properties
  //  {
		//_MainTex("Main Texture", 2D) = "white" {}
  //  }
  //  SubShader
  //  {
		//Tags 
		//{ 
		//	"RenderType" = "Opaque"
		//	"Queue" = "Geometry"
		//	"RenderPipeline" = "UniversalPipeline"
		//}

		//HLSLINCLUDE
		//	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		//ENDHLSL

  //      Pass
  //      {
  //          Name "Mask"

  //          HLSLPROGRAM
		//		#pragma vertex vert
		//		#pragma fragment frag

  //          HLSLPROGRAM
  //              struct appdata
  //              {
  //                  float4 vertex : POSITION;
  //              };

  //              struct v2f
  //              {
  //                  float4 vertex : SV_POSITION;
  //                  float4 screenPos : TEXCOORD0;
  //              };

  //              sampler2D _MainTex;
  //              float4 _InactiveColour;
  //              int displayMask; // set to 1 to display texture, otherwise will draw test colour
            

  //              v2f vert (appdata v)
  //              {
  //                  v2f o;
  //                  o.vertex = TransformObjectToHClip(v.vertex.xyz);
  //                  o.screenPos = ComputeScreenPos(o.vertex);
  //                  return o;
  //              }

  //              uniform sampler2D _MainTex;

  //              float4 frag (v2f i) : SV_Target
  //              {
  //                  float2 uv = i.screenPos.xy / i.screenPos.w;
  //                  float4 portalCol = tex2D(_MainTex, uv);
  //                  return portalCol;
  //              }
  //          ENDHLSL
  //      }
  //  }
}
