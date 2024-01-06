// Not for redistribution without the author's express written permission
Shader "MMD4Mecanim/MMDLit-NEXTEdge-Pass4"
{
	Properties
	{
		_EdgeColor("EdgeColor", Color) = (0.4,1,1,1)
		_EdgeSize("EdgeSize", Float) = 0.005
	
		_PostfixRenderQueue("PostfixRenderQueue",Float) = 0
	}

	SubShader
	{
		Tags { "Queue" = "Geometry" "RenderType" = "Transparent" } // Draw after skybox(+501)
		LOD 200

		Cull Front
		ZWrite Off
		ZTest Less
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB

		Pass {
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma target 2.0
			#pragma exclude_renderers flash
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fwdbase nolightmap nodirlightmap novertexlight
			#define EDGE_SCALE (4.0 / 4.0)
			#define ALPHA_SCALE (1.0 / 4.0)
			#include "MMD4Mecanim-MMDLit-NEXTEdge-ForwardBase.cginc"
			ENDCG
		}
		
		Pass {
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma target 2.0
			#pragma exclude_renderers flash
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fwdbase nolightmap nodirlightmap novertexlight
			#define EDGE_SCALE (3.0 / 4.0)
			#define ALPHA_SCALE (2.0 / 4.0)
			#include "MMD4Mecanim-MMDLit-NEXTEdge-ForwardBase.cginc"
			ENDCG
		}
		
		Pass {
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma target 2.0
			#pragma exclude_renderers flash
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fwdbase nolightmap nodirlightmap novertexlight
			#define EDGE_SCALE (2.0 / 4.0)
			#define ALPHA_SCALE (3.0 / 4.0)
			#include "MMD4Mecanim-MMDLit-NEXTEdge-ForwardBase.cginc"
			ENDCG
		}
		
		Pass {
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma target 2.0
			#pragma exclude_renderers flash
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fwdbase nolightmap nodirlightmap novertexlight
			#define EDGE_SCALE (1.0 / 4.0)
			#define ALPHA_SCALE (4.0 / 4.0)
			#include "MMD4Mecanim-MMDLit-NEXTEdge-ForwardBase.cginc"
			ENDCG
		}
	}

	Fallback Off
}
