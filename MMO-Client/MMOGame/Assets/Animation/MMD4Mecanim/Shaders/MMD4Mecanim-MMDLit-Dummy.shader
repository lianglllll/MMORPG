// Not for redistribution without the author's express written permission
Shader "MMD4Mecanim/MMDLit-Dummy"
{
	SubShader {
		Tags { "Queue" = "Background-1000" "RenderType" = "Opaque" "ForceNoShadowCasting" = "True" }
		LOD 200

		Pass { // No effeects.(Removed by ForceNoShadowCasting = True)
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			Fog { Mode Off }

			ZTest Equal
			ZWrite Off
			Cull Off
			Lighting Off
			Blend Off
			ColorMask 0

			CGPROGRAM
			#pragma target 2.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "MMD4Mecanim-MMDLit-Lighting.cginc"

			struct v2f {
			    float4 pos : SV_POSITION;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = half4(0,0,-2,1);
				return o;
			}

			half4 frag (v2f i) : MMDLIT_SV_TARGET
			{
				return half4(0,0,0,0);
			}
			ENDCG
	    }
	}

	Fallback Off
}
