// Not for redistribution without the author's express written permission
#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
#ifndef UNITY_PASS_FORWARDADD
#define UNITY_PASS_FORWARDADD
#endif
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "MMD4Mecanim-MMDLit-AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

#include "MMD4Mecanim-MMDLit-Lighting.cginc"
#include "MMD4Mecanim-MMDLit-Surface-Lighting.cginc"
#include "MMD4Mecanim-MMDLit-Compatible.cginc"

struct v2f_surf
{
	float4 pos : SV_POSITION;
	float4 pack0 : TEXCOORD0;
	half3 normal : TEXCOORD1;
	half4 lightDir : TEXCOORD2;
	half4 viewDir : TEXCOORD3;
	LIGHTING_COORDS(4,5)
};

float4 _MainTex_ST;

v2f_surf vert_surf(appdata_full v)
{
	v2f_surf o;
	o.pos = _UnityObjectToClipPos(v.vertex);
	o.pack0.xy = v.texcoord.xy;
	o.normal = mul((float3x3)_UNITY_OBJECT_TO_WORLD, SCALED_NORMAL);
	o.lightDir.xyz = WorldSpaceLightDir(v.vertex);
	o.viewDir.xyz = WorldSpaceViewDir(v.vertex);
	half NdotL = dot(o.normal, (half3)o.lightDir);
	half toonRefl = MMDLit_GetToolRefl(NdotL);
	o.pack0.z = toonRefl;
	o.pack0.w = MMDLit_GetForwardAddStr(toonRefl);
	o.lightDir.w = 0.0;
	o.viewDir.w = MMDLit_GetToonShadow(toonRefl);
	TRANSFER_VERTEX_TO_FRAGMENT(o);
	return o;
}

inline half3 frag_core(in v2f_surf IN, half4 albedo)
{
	half atten = LIGHT_ATTENUATION(IN); // SHADOW_ATTENUATION() might be 1.0 (Can use LIGHT_ATTENUATION() only.)
	#ifndef USING_DIRECTIONAL_LIGHT
	half3 lightDir = normalize((half3)IN.lightDir);
	#else
	half3 lightDir = (half3)IN.lightDir;
	#endif

	half toonRefl = (half)IN.pack0.z;
	half forwardAddStr = (half)IN.pack0.w;
	half toonShadow = IN.viewDir.w;
	half NdotL = IN.lightDir.w;
	half3 c = MMDLit_Lighting_Add(
		(half3)albedo,
		NdotL,
		toonRefl,
		toonShadow,
		IN.normal,
		(half3)lightDir,
		normalize((half3)IN.viewDir),
		atten);

	c *= forwardAddStr;
	return c;
}

fixed4 frag_surf(v2f_surf IN) : MMDLIT_SV_TARGET
{
	half4 albedo = MMDLit_GetAlbedo(IN.pack0.xy);
	albedo.a *= _Color.a; // for Transparency
	MMDLIT_CLIP(albedo.a)
	half3 c = frag_core(IN, albedo);
	c = min(c, 1.0);
	c *= albedo.a;
	return fixed4(c, 0.0);
}

fixed4 frag_fast(v2f_surf IN) : MMDLIT_SV_TARGET
{
	half4 albedo = MMDLit_GetAlbedo(IN.pack0.xy);
	MMDLIT_CLIP_FAST(albedo.a)
	half3 c = frag_core(IN, albedo);
	return fixed4(c, 0.0);
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
