// Not for redistribution without the author's express written permission
#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
#ifndef UNITY_PASS_FORWARDBASE
#define UNITY_PASS_FORWARDBASE
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

#ifdef TESSELLATION_ON
#include "Tessellation.cginc"
#endif

struct v2f_surf {
	float4 pos : SV_POSITION;
	float2 pack0 : TEXCOORD0;
	half3 normal : TEXCOORD1;
	half3 vlight : TEXCOORD2;
	half3 viewDir : TEXCOORD3;
	LIGHTING_COORDS(4,5)
	half3 mmd_globalAmbient : TEXCOORD6;
	#ifdef SPHEREMAP_ON
	half3 mmd_uvwSphere : TEXCOORD7;
	#endif
};

float4 _MainTex_ST;

v2f_surf vert_surf(appdata_full v)
{
	v2f_surf o;
	o.pos = _UnityObjectToClipPos(v.vertex);
	o.pack0.xy = v.texcoord.xy;
	float3 worldN = mul((float3x3)_UNITY_OBJECT_TO_WORLD, SCALED_NORMAL);
	o.normal = worldN;

	#ifdef SPHEREMAP_ON
	float4x4 matMV = MMDLit_GetMatrixMV();
	half3 norm = normalize(mul((float3x3)matMV, v.normal));
	half3 eye = normalize(mul(matMV, v.vertex).xyz);
	o.mmd_uvwSphere = reflect(eye, norm);
	#endif
	
	o.viewDir = (half3)WorldSpaceViewDir(v.vertex);
	
	o.vlight = ShadeSH9(float4(worldN, 1.0));
	o.mmd_globalAmbient = o.vlight;
	#ifdef VERTEXLIGHT_ON
	float3 worldPos = mul(_UNITY_OBJECT_TO_WORLD, v.vertex).xyz;
	o.vlight += Shade4PointLights(
		unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
		unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
		unity_4LightAtten0, worldPos, worldN );
	#endif // VERTEXLIGHT_ON
	// Feedback Ambient.
	o.vlight *= MMDLit_GetAmbientRate();

	TRANSFER_VERTEX_TO_FRAGMENT(o);
	return o;
}

inline half4 frag_core(in v2f_surf IN, half4 albedo)
{
	half atten = LIGHT_ATTENUATION(IN);
	half shadowAtten = SHADOW_ATTENUATION(IN);
	half3 c;

	half3 baseC;
	half NdotL = dot(IN.normal, _WorldSpaceLightPos0.xyz);
	c = MMDLit_Lighting(
		(half3)albedo,
		#ifdef SPHEREMAP_ON
		IN.mmd_uvwSphere,
		#else
		half3( 0.0, 0.0, 0.0 ),
		#endif
		NdotL,
		IN.normal,
		_WorldSpaceLightPos0.xyz,
		normalize(IN.viewDir),
		atten,
		shadowAtten,
		baseC,
		IN.mmd_globalAmbient);

	c += baseC * IN.vlight;

	return half4(c, albedo.a);
}

// for Transparency
half4 frag_surf(v2f_surf IN) : MMDLIT_SV_TARGET
{
	half4 albedo = MMDLit_GetAlbedo(IN.pack0.xy);
	albedo.a *= _Color.a; // for Transparency
	MMDLIT_CLIP(albedo.a)
	return frag_core(IN, albedo);
}

// for Opaque
half4 frag_fast(v2f_surf IN) : MMDLIT_SV_TARGET
{
	half4 albedo = MMDLit_GetAlbedo(IN.pack0.xy);
	MMDLIT_CLIP_FAST(albedo.a)
	return frag_core(IN, albedo);
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
