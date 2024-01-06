// UNITY_SHADER_NO_UPGRADE
#ifndef MMD4MECANIM_MMDLIT_COMPATIBLE_INCLUDED
#define MMD4MECANIM_MMDLIT_COMPATIBLE_INCLUDED

#if UNITY_VERSION >= 540
#define _UNITY_OBJECT_TO_WORLD unity_ObjectToWorld
#else
#define _UNITY_OBJECT_TO_WORLD _Object2World
#endif

#if UNITY_VERSION >= 560
#define _UNITY_SHADOW_ATTENUATION UNITY_SHADOW_ATTENUATION
#else
#define _UNITY_SHADOW_ATTENUATION(a, worldPos) SHADOW_ATTENUATION(a)
#endif

#if UNITY_VERSION >= 550
#define _UNITY_VERTEX_INPUT_INSTANCE_ID UNITY_VERTEX_INPUT_INSTANCE_ID
#else
#define _UNITY_VERTEX_INPUT_INSTANCE_ID UNITY_INSTANCE_ID
#endif

#if UNITY_VERSION >= 540
#define _UnityObjectToClipPos(V_) UnityObjectToClipPos(V_)
#define _UnityObjectToViewPos(V_) UnityObjectToViewPos(V_)
#else
#define _UnityObjectToClipPos(V_) mul(UNITY_MATRIX_MVP, V_)
#define _UnityObjectToViewPos(V_) mul(UNITY_MATRIX_MV, float4((float3)V_, 1.0)).xyz
#endif

#if UNITY_VERSION >= 540
#ifdef UNITY_USE_PREMULTIPLIED_MATRICES
inline float4x4 MMDLit_GetMatrixMV() { return UNITY_MATRIX_MV; }
#else
inline float4x4 MMDLit_GetMatrixMV() { return mul(UNITY_MATRIX_V, _UNITY_OBJECT_TO_WORLD); }
#endif
#else
inline float4x4 MMDLit_GetMatrixMV() { return UNITY_MATRIX_MV; }
#endif

#endif
