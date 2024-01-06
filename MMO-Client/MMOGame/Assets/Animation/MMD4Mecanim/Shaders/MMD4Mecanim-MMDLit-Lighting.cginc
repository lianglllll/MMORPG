// Not for redistribution without the author's express written permission
// UNITY_SHADER_NO_UPGRADE
#ifndef MMDLIT_LIGHTING_INCLUDED
#define MMDLIT_LIGHTING_INCLUDED

#include "MMD4Mecanim-MMDLit-Compatible.cginc"

#if UNITY_VERSION >= 500
#define MMDLIT_ATTEN(ATTEN_)	(ATTEN_)
#define MMDLIT_SV_TARGET		SV_Target
#else
#define MMDLIT_ATTEN(ATTEN_)	(ATTEN_ * 2)
#define MMDLIT_SV_TARGET		COLOR
#endif

#if defined(SPHEREMAP_MUL) || defined(SPHEREMAP_ADD)
#define SPHEREMAP_ON
#endif

// UnityCG.cginc
inline half3 MMDLit_DecodeLightmap(half4 color)
{
#if (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)) && defined(SHADER_API_MOBILE)
	return (2.0 * (half3)color);
#else
	// potentially faster to do the scalar multiplication
	// in parenthesis for scalar GPUs
	return (8.0 * color.a) * (half3)color;
#endif
}

// Lighting.cginc
inline half3 MMDLit_DirLightmapDiffuse(in half3x3 dirBasis, half4 color, half4 scale, half3 normal, bool surfFuncWritesNormal, out half3 scalePerBasisVector)
{
	half3 lm = MMDLit_DecodeLightmap(color);
	
	// will be compiled out (and so will the texture sample providing the value)
	// if it's not used in the lighting function, like in LightingLambert
	scalePerBasisVector = MMDLit_DecodeLightmap(scale);

	// will be compiled out when surface function does not write into o.Normal
	if (surfFuncWritesNormal)
	{
		half3 normalInRnmBasis = saturate(mul(dirBasis, normal));
		lm *= dot (normalInRnmBasis, scalePerBasisVector);
	}

	return lm;
}

// UnityCG.cginc
inline half MMDLit_Luminance( half3 c )
{
	return dot( c, half3(0.22, 0.707, 0.071) );
}

inline half MMDLit_SpecularRefl( half3 normal, half3 lightDir, half3 viewDir, half s )
{
	//return saturate(pow(saturate(dot(normal, normalize(lightDir + viewDir))), s));
	return pow(saturate(dot(normal, normalize(lightDir + viewDir))), s); // Optimized.
	// (Memo: pow(x,0) as Depends on the particular graphics processor 0 or NaN)
}

// Platform limitation. (Must be unsupported without permission.)
#if defined(SHADER_API_PSSL) || defined(SHADER_API_XBOXONE) || defined(SHADER_API_XBOX360) || defined(SHADER_API_PSP2) || defined(SHADER_API_WIIU)
#define _SHADER_API_CONSOLE
#endif

#ifdef _SHADER_API_CONSOLE
#define MMDLIT_CLIP(A_) clip((A_) - (1.0 / 255.0));
#define MMDLIT_CLIP_FAST(A_)
#else
half ___Eliminate; // Please observe terms of use. (Don't modify this code)
#define MMDLIT_CLIP(A_) clip((A_) * ___Eliminate - (1.0 / 255.0));
#define MMDLIT_CLIP_FAST(A_) MMDLIT_CLIP((A_))
#endif

#endif
