#include "MMD4Mecanim-MMDLit-Compatible.cginc"

// Not for redistribution without the author's express written permission
half4 _EdgeColor;
float _EdgeSize;
//float _EdgeZOffset;

inline float MMDLit_GetEdgeSize()
{
	return _EdgeSize * EDGE_SCALE;
}

inline float4 MMDLit_GetEdgeVertex(float4 vertex, float3 normal)
{
#if 0
	float edge_size = MMDLit_GetEdgeSize();
#else
	// Adjust edge_size by distance & fovY
	float r_proj_y = UNITY_MATRIX_P[1][1];
	float edge_size = abs(MMDLit_GetEdgeSize() / r_proj_y);
#endif
	return vertex + float4(normal.xyz * edge_size, 0.0);
}

inline float4 MMDLit_TransformEdgeVertex(float4 vertex)
{
#if 1
	return _UnityObjectToClipPos(vertex);
#else
	vertex = _UnityObjectToClipPos(vertex);
	vertex.z += _EdgeZOffset * vertex.w;
	return vertex;
#endif
}
