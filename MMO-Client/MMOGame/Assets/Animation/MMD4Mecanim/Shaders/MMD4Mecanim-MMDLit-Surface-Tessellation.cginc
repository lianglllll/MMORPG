// Not for redistribution without the author's express written permission
// UNITY_SHADER_NO_UPGRADE
#ifndef MMDLIT_SURFACE_TESSELLATION_INCLUDED
#define MMDLIT_SURFACE_TESSELLATION_INCLUDED

#ifdef TESSELLATION_ON
#include "HLSLSupport.cginc" // UNITY_CAN_COMPILE_TESSELLATION
#include "Lighting.cginc" // UnityTessellationFactors
#include "Tessellation.cginc"

#ifdef UNITY_CAN_COMPILE_TESSELLATION

float _TessPhongStrength;
float _TessEdgeLength;
float _TessExtrusionAmount;

#if UNITY_VERSION >= 500
struct InternalTessInterp_appdata_full
{
	float4 vertex : INTERNALTESSPOS;
	float4 tangent : TANGENT;
	float3 normal : NORMAL;
	float4 texcoord : TEXCOORD0;
	float4 texcoord1 : TEXCOORD1;
	float4 texcoord2 : TEXCOORD2;
	float4 texcoord3 : TEXCOORD3;
#if defined(SHADER_API_XBOX360)
	half4 texcoord4 : TEXCOORD4;
	half4 texcoord5 : TEXCOORD5;
#endif // defined(SHADER_API_XBOX360)
	fixed4 color : COLOR;
	//	UNITY_INSTANCE_ID
};
#else // UNITY_VERSION >= 500
struct InternalTessInterp_appdata_full
{
	float4 vertex : INTERNALTESSPOS;
	float4 tangent : TANGENT;
	float3 normal : NORMAL;
	float4 texcoord : TEXCOORD0;
	float4 texcoord1 : TEXCOORD1;
	fixed4 color : COLOR;
#if defined(SHADER_API_XBOX360)
	half4 texcoord2 : TEXCOORD2;
	half4 texcoord3 : TEXCOORD3;
	half4 texcoord4 : TEXCOORD4;
	half4 texcoord5 : TEXCOORD5;
#endif // defined(SHADER_API_XBOX360)
};
#endif // UNITY_VERSION >= 500

InternalTessInterp_appdata_full tess_appdata_full(appdata_full v)
{
	InternalTessInterp_appdata_full o;
	o.vertex = v.vertex;
	o.tangent = v.tangent;
	o.normal = v.normal;
	o.texcoord = v.texcoord;
	o.texcoord1 = v.texcoord1;
	o.color = v.color;
#if UNITY_VERSION >= 500
	o.texcoord2 = v.texcoord2;
	o.texcoord3 = v.texcoord3;
#if defined(SHADER_API_XBOX360)
	o.texcoord4 = v.texcoord4;
	o.texcoord5 = v.texcoord5;
#endif // defined(SHADER_API_XBOX360)
#else // UNITY_VERSION >= 500
#if defined(SHADER_API_XBOX360)
	o.texcoord2 = v.texcoord2;
	o.texcoord3 = v.texcoord3;
	o.texcoord4 = v.texcoord4;
	o.texcoord5 = v.texcoord5;
#endif // defined(SHADER_API_XBOX360)
#endif // UNITY_VERSION >= 500
	return o;
}

// tessellation hull constant shader
UnityTessellationFactors hsconst_appdata_full(InputPatch<InternalTessInterp_appdata_full, 3> v)
{
	float4 tf = UnityEdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, _TessEdgeLength);
	UnityTessellationFactors o;
	o.edge[0] = tf.x;
	o.edge[1] = tf.y;
	o.edge[2] = tf.z;
	o.inside = tf.w;
	return o;
}

// tessellation hull shader
[UNITY_domain("tri")]
[UNITY_partitioning("fractional_odd")]
[UNITY_outputtopology("triangle_cw")]
[UNITY_patchconstantfunc("hsconst_appdata_full")]
[UNITY_outputcontrolpoints(3)]
InternalTessInterp_appdata_full hs_appdata_full(InputPatch<InternalTessInterp_appdata_full, 3> v, uint id : SV_OutputControlPointID)
{
	return v[id];
}

inline appdata_full _ds_appdata_full(UnityTessellationFactors tessFactors, const OutputPatch<InternalTessInterp_appdata_full, 3> vi, float3 bary : SV_DomainLocation)
{
	appdata_full v;
	v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
	float3 pp[3];
	for (int i = 0; i < 3; ++i)
		pp[i] = v.vertex.xyz - vi[i].normal * (dot(v.vertex.xyz, vi[i].normal) - dot(vi[i].vertex.xyz, vi[i].normal));
	v.vertex.xyz = _TessPhongStrength * (pp[0] * bary.x + pp[1] * bary.y + pp[2] * bary.z) + (1.0f - _TessPhongStrength) * v.vertex.xyz;
	v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
	v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
	v.vertex.xyz += v.normal.xyz * _TessExtrusionAmount;
	v.texcoord = vi[0].texcoord*bary.x + vi[1].texcoord*bary.y + vi[2].texcoord*bary.z;
	v.texcoord1 = vi[0].texcoord1*bary.x + vi[1].texcoord1*bary.y + vi[2].texcoord1*bary.z;
	v.color = vi[0].color*bary.x + vi[1].color*bary.y + vi[2].color*bary.z;
#if UNITY_VERSION >= 500
	v.texcoord2 = vi[0].texcoord2*bary.x + vi[1].texcoord2*bary.y + vi[2].texcoord2*bary.z;
	v.texcoord3 = vi[0].texcoord3*bary.x + vi[1].texcoord3*bary.y + vi[2].texcoord3*bary.z;
#if defined(SHADER_API_XBOX360)
	v.texcoord4 = vi[0].texcoord4*bary.x + vi[1].texcoord4*bary.y + vi[2].texcoord4*bary.z;
	v.texcoord5 = vi[0].texcoord5*bary.x + vi[1].texcoord5*bary.y + vi[2].texcoord5*bary.z;
#endif // defined(SHADER_API_XBOX360)
#else // UNITY_VERSION >= 500
#if defined(SHADER_API_XBOX360)
	v.texcoord2 = vi[0].texcoord2*bary.x + vi[1].texcoord2*bary.y + vi[2].texcoord2*bary.z;
	v.texcoord3 = vi[0].texcoord3*bary.x + vi[1].texcoord3*bary.y + vi[2].texcoord3*bary.z;
	v.texcoord4 = vi[0].texcoord4*bary.x + vi[1].texcoord4*bary.y + vi[2].texcoord4*bary.z;
	v.texcoord5 = vi[0].texcoord5*bary.x + vi[1].texcoord5*bary.y + vi[2].texcoord5*bary.z;
#endif // defined(SHADER_API_XBOX360)
#endif // UNITY_VERSION >= 500
	return v;
}

#endif // UNITY_CAN_COMPILE_TESSELLATION
#endif // TESSELLATION_ON

#endif // MMDLIT_SURFACE_TESSELLATION_INCLUDED
