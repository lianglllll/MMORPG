Shader "TrailsFX/Effect/Outline" {
Properties {
    _MainTex ("Texture", 2D) = "white" {}
    _Color ("Color", Color) = (1,1,1,1)
    _NormalThreshold("Normal Threshold", Float) = 0.1
    _RimPower("Rim Power", Float) = 1.0
    _Cull ("Cull", Int) = 2
    _ZTest ("ZTest", Int) = 4
    _ZOffset("ZOffset", Float) = 0
}
    SubShader
    {
        Tags { "Queue"="Transparent+101" "RenderType"="Transparent" }

        Pass
        {
			Stencil {
                Ref 2
                ReadMask 2
                Comp NotEqual
                Pass replace
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull [_Cull]
            ZTest [_ZTest]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing assumeuniformscaling nolightprobe nolodfade nolightmap
            #pragma multi_compile_local _ TRAIL_INTERPOLATE
            #pragma multi_compile_local _ TRAIL_MASK
            #pragma multi_compile_local _ TRAIL_LOCAL
            #pragma multi_compile_local _ TRAIL_RIM
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 norm   : NORMAL;
                #if TRAIL_MASK
                    float2 uv : TEXCOORD0;
                #endif
                #if TRAIL_INTERPOLATE
                    float4 prevVertex : TEXCOORD1;
                #endif
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
            	UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 pos    : SV_POSITION;
                #if TRAIL_MASK
                    float2 uv : TEXCOORD0;
                #endif
                float3 wsNorm : TEXCOORD1;
                #if TRAIL_RIM
                    float3 viewDir : TEXCOORD2;
                #endif
				UNITY_VERTEX_OUTPUT_STEREO
            };

UNITY_INSTANCING_BUFFER_START(Props)
	UNITY_DEFINE_INSTANCED_PROP(fixed4, _Colors)
    #if TRAIL_INTERPOLATE
        UNITY_DEFINE_INSTANCED_PROP(half, _SubFrameKeys)
    #endif
    #if TRAIL_LOCAL
        UNITY_DEFINE_INSTANCED_PROP(float4x4, _ParentMatrices)
    #endif
UNITY_INSTANCING_BUFFER_END(Props)

            sampler2D _MaskTex;
            float4 _MaskTex_ST;
            float4x4 _PivotMatrix;
            float _NormalThreshold;
            float _RimPower;
            float _ZOffset;

            v2f vert (appdata v)
            {
                v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                float4 vertex = v.vertex;

                #if TRAIL_INTERPOLATE
                    half key = UNITY_ACCESS_INSTANCED_PROP(Props, _SubFrameKeys);
                    vertex.xyz = lerp(v.prevVertex.xyz, vertex.xyz, key);
                #endif

                #if TRAIL_LOCAL
                    vertex = mul(unity_ObjectToWorld, vertex);
                    float4x4 parentMatrix = UNITY_ACCESS_INSTANCED_PROP(Props, _ParentMatrices);
                    vertex = mul(parentMatrix, vertex);
                    vertex = mul(_PivotMatrix, vertex);
                    o.pos = mul(UNITY_MATRIX_VP, vertex);
                #else
                    o.pos = UnityObjectToClipPos(vertex);
                #endif

                o.wsNorm = UnityObjectToWorldNormal(v.norm);
                #if TRAIL_RIM
                    o.viewDir = WorldSpaceViewDir(vertex);
                #endif

                #if TRAIL_MASK
                    o.uv = TRANSFORM_TEX(v.uv, _MaskTex);
                #endif

                #if UNITY_REVERSED_Z // prevent flickering when render order is BeforeObject
                    o.pos.z += _ZOffset;
                #else
                    o.pos.z -= _ZOffset;
                #endif

                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            {
            	UNITY_SETUP_INSTANCE_ID(i);
              	fixed4 col = UNITY_ACCESS_INSTANCED_PROP(Props, _Colors);
                #if TRAIL_RIM
                    fixed dx = 1.0 - dot(normalize(i.wsNorm), normalize(i.viewDir));
                    dx = pow(dx, _RimPower);
                #else
              	    fixed dx = saturate(dot(fwidth(i.wsNorm), 1.0.xxx) / _NormalThreshold);
                #endif

              	col *= dx;

                #if TRAIL_MASK
                    fixed4 mask = tex2D(_MaskTex, i.uv);
                    col.a *= mask.r;
                #endif

				return col;
            }
            ENDCG
        }

    }
}