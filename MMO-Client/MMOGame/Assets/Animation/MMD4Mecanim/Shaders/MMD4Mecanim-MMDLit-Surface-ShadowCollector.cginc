// Not for redistribution without the author's express written permission
#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
#define UNITY_PASS_SHADOWCOLLECTOR
#define SHADOW_COLLECTOR_PASS
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "MMD4Mecanim-MMDLit-Lighting.cginc"
#include "MMD4Mecanim-MMDLit-Surface-Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

//half4 _Color;
//sampler2D _MainTex;

struct v2f_surf {
  V2F_SHADOW_COLLECTOR;
  float2 pack0 : TEXCOORD5;
};
v2f_surf vert_surf (appdata_full v) {
  v2f_surf o;
  o.pack0.xy = v.texcoord;
  TRANSFER_SHADOW_COLLECTOR(o)
  return o;
}
fixed4 frag_surf (v2f_surf IN) : MMDLIT_SV_TARGET{
  half4 albedo = (half4)tex2D(_MainTex, IN.pack0.xy);
  albedo.a *= _Color.a; // for Transparency
  MMDLIT_CLIP(albedo.a)

  SHADOW_COLLECTOR_FRAGMENT(IN)
}

fixed4 frag_fast (v2f_surf IN) : MMDLIT_SV_TARGET{
  MMDLIT_CLIP_FAST(1.0)
  SHADOW_COLLECTOR_FRAGMENT(IN)
}

#ifdef TESSELLATION_ON
#ifdef UNITY_CAN_COMPILE_TESSELLATION

// tessellation domain shader
[UNITY_domain("tri")]
v2f_surf ds_surf(UnityTessellationFactors tessFactors, const OutputPatch<InternalTessInterp_appdata_full, 3> vi, float3 bary : SV_DomainLocation)
{
	appdata_full v = _ds_appdata_full(tessFactors, vi, bary);
	v2f_surf o = vert_surf(v);
	return o;
}

#endif // UNITY_CAN_COMPILE_TESSELLATION
#endif // TESSELLATION_ON
