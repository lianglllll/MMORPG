// Not for redistribution without the author's express written permission
// UNITY_SHADER_NO_UPGRADE
#include "AutoLight.cginc"

// ------------------------------
//  Light helpers (5.0+ version)
// ------------------------------

#if UNITY_VERSION >= 540
#define _UNITY_WORLD_TO_LIGHT unity_WorldToLight
#else
#define _UNITY_WORLD_TO_LIGHT _LightMatrix0
#endif

#if UNITY_VERSION >= 500

#undef UNITY_LIGHT_ATTENUATION

#ifdef POINT
#define UNITY_LIGHT_ATTENUATION(destName, input, worldPos) \
	unityShadowCoord3 lightCoord = mul(_UNITY_WORLD_TO_LIGHT, unityShadowCoord4(worldPos, 1)).xyz; \
	fixed destName = (tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL);
#endif

#ifdef SPOT
#define UNITY_LIGHT_ATTENUATION(destName, input, worldPos) \
	unityShadowCoord4 lightCoord = mul(_UNITY_WORLD_TO_LIGHT, unityShadowCoord4(worldPos, 1)); \
	fixed destName = (lightCoord.z > 0) * UnitySpotCookie(lightCoord) * UnitySpotAttenuate(lightCoord.xyz);
#endif


#ifdef DIRECTIONAL
#define UNITY_LIGHT_ATTENUATION(destName, input, worldPos)	fixed destName = 1.0;
#endif


#ifdef POINT_COOKIE
#define UNITY_LIGHT_ATTENUATION(destName, input, worldPos) \
	unityShadowCoord3 lightCoord = mul(_UNITY_WORLD_TO_LIGHT, unityShadowCoord4(worldPos, 1)).xyz; \
	fixed destName = tex2D(_LightTextureB0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL * texCUBE(_LightTexture0, lightCoord).w;
#endif

#ifdef DIRECTIONAL_COOKIE
#define UNITY_LIGHT_ATTENUATION(destName, input, worldPos) \
	unityShadowCoord2 lightCoord = mul(_UNITY_WORLD_TO_LIGHT, unityShadowCoord4(worldPos, 1)).xy; \
	fixed destName = tex2D(_LightTexture0, lightCoord).w;
#endif

#endif // UNITY_VERSION >= 500

// -----------------------------
//  Light helpers (4.x version)
// -----------------------------

#undef LIGHT_ATTENUATION

#ifdef POINT
#define LIGHT_ATTENUATION(a)	(tex2D(_LightTexture0, dot(a._LightCoord,a._LightCoord).rr).UNITY_ATTEN_CHANNEL)
#endif

#ifdef SPOT
#define LIGHT_ATTENUATION(a)	( (a._LightCoord.z > 0) * UnitySpotCookie(a._LightCoord) * UnitySpotAttenuate(a._LightCoord.xyz) )
#endif


#ifdef DIRECTIONAL
	#define LIGHT_ATTENUATION(a)	1.0
#endif


#ifdef POINT_COOKIE
#define LIGHT_ATTENUATION(a)	(tex2D(_LightTextureB0, dot(a._LightCoord,a._LightCoord).rr).UNITY_ATTEN_CHANNEL * texCUBE(_LightTexture0, a._LightCoord).w)
#endif

#ifdef DIRECTIONAL_COOKIE
#define LIGHT_ATTENUATION(a)	(tex2D(_LightTexture0, a._LightCoord).w)
#endif

// -----------------------------
//  Salvage some macros (2018.1 or later)
// -----------------------------

#if UNITY_VERSION >= 201801
#if !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)

#undef DECLARE_LIGHT_COORDS
#undef COMPUTE_LIGHT_COORDS

#ifdef POINT
#       define DECLARE_LIGHT_COORDS(idx) unityShadowCoord3 _LightCoord : TEXCOORD##idx;
#       define COMPUTE_LIGHT_COORDS(a) a._LightCoord = mul(unity_WorldToLight, mul(unity_ObjectToWorld, v.vertex)).xyz;
#endif

#ifdef SPOT
#       define DECLARE_LIGHT_COORDS(idx) unityShadowCoord4 _LightCoord : TEXCOORD##idx;
#       define COMPUTE_LIGHT_COORDS(a) a._LightCoord = mul(unity_WorldToLight, mul(unity_ObjectToWorld, v.vertex));
#endif

#ifdef DIRECTIONAL
#define DECLARE_LIGHT_COORDS(idx)
#define COMPUTE_LIGHT_COORDS(a)
#endif

#ifdef POINT_COOKIE
#       define DECLARE_LIGHT_COORDS(idx) unityShadowCoord3 _LightCoord : TEXCOORD##idx;
#       define COMPUTE_LIGHT_COORDS(a) a._LightCoord = mul(unity_WorldToLight, mul(unity_ObjectToWorld, v.vertex)).xyz;
#endif

#ifdef DIRECTIONAL_COOKIE
#       define DECLARE_LIGHT_COORDS(idx) unityShadowCoord2 _LightCoord : TEXCOORD##idx;
#       define COMPUTE_LIGHT_COORDS(a) a._LightCoord = mul(unity_WorldToLight, mul(unity_ObjectToWorld, v.vertex)).xy;
#endif

#endif //!defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
#endif // UNITY_VERSION >= 201801
