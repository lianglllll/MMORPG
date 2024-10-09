// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Mirza Beig/Lightning VFX/Post-Processing/Grayscale"
{
	Properties
	{
		_MainTex ( "Screen", 2D ) = "black" {}
		_RemapMin("Remap Min", Range( 0 , 1)) = 0
		_RemapMax("Remap Max", Range( 0 , 1)) = 1
		_ScreenColor("Screen Color", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		
		
		ZTest Always
		Cull Off
		ZWrite Off

		
		Pass
		{ 
			CGPROGRAM 

			

			#pragma vertex vert_img_custom 
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
			

			struct appdata_img_custom
			{
				float4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
				
			};

			struct v2f_img_custom
			{
				float4 pos : SV_POSITION;
				half2 uv   : TEXCOORD0;
				half2 stereoUV : TEXCOORD2;
		#if UNITY_UV_STARTS_AT_TOP
				half4 uv2 : TEXCOORD1;
				half4 stereoUV2 : TEXCOORD3;
		#endif
				
			};

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_TexelSize;
			uniform half4 _MainTex_ST;
			
			uniform float _RemapMin;
			uniform float _RemapMax;
			uniform float _ScreenColor;


			v2f_img_custom vert_img_custom ( appdata_img_custom v  )
			{
				v2f_img_custom o;
				
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = float4( v.texcoord.xy, 1, 1 );

				#if UNITY_UV_STARTS_AT_TOP
					o.uv2 = float4( v.texcoord.xy, 1, 1 );
					o.stereoUV2 = UnityStereoScreenSpaceUVAdjust ( o.uv2, _MainTex_ST );

					if ( _MainTex_TexelSize.y < 0.0 )
						o.uv.y = 1.0 - o.uv.y;
				#endif
				o.stereoUV = UnityStereoScreenSpaceUVAdjust ( o.uv, _MainTex_ST );
				return o;
			}

			half4 frag ( v2f_img_custom i ) : SV_Target
			{
				#ifdef UNITY_UV_STARTS_AT_TOP
					half2 uv = i.uv2;
					half2 stereoUV = i.stereoUV2;
				#else
					half2 uv = i.uv;
					half2 stereoUV = i.stereoUV;
				#endif	
				
				half4 finalColor;

				// ase common template code
				float3 temp_cast_0 = (_RemapMin).xxx;
				float3 temp_cast_1 = (_RemapMax).xxx;
				float2 uv_MainTex = i.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 ScreenColour20 = tex2D( _MainTex, uv_MainTex );
				float3 desaturateInitialColor14 = ScreenColour20.rgb;
				float desaturateDot14 = dot( desaturateInitialColor14, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar14 = lerp( desaturateInitialColor14, desaturateDot14.xxx, 1.0 );
				float3 smoothstepResult18 = smoothstep( temp_cast_0 , temp_cast_1 , desaturateVar14);
				float4 lerpResult21 = lerp( float4( smoothstepResult18 , 0.0 ) , ScreenColour20 , _ScreenColor);
				

				finalColor = lerpResult21;

				return finalColor;
			} 
			ENDCG 
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19108
Node;AmplifyShaderEditor.DesaturateOpNode;14;-587.961,-269.006;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-653.0654,17.64146;Inherit;False;Property;_RemapMax;Remap Max;1;0;Create;True;0;0;0;False;0;False;1;0.506;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-669.5403,-98.30032;Inherit;False;Property;_RemapMin;Remap Min;0;0;Create;True;0;0;0;False;0;False;0;0.973;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;18;-159.4052,-184.4234;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;1,1,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;2;-1301.129,-242.0744;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;1;-1543.374,-243.3927;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;20;-927.2366,-239.0457;Inherit;False;ScreenColour;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;21;289.0944,-141.8018;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;680.9846,-164.0037;Float;False;True;-1;2;ASEMaterialInspector;0;9;Mirza Beig/Lightning VFX/Post-Processing/Grayscale;c71b220b631b6344493ea3cf87110c93;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;True;7;False;;False;True;0;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.GetLocalVarNode;22;-175.6017,-3.038978;Inherit;False;20;ScreenColour;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-203.558,114.9942;Inherit;False;Property;_ScreenColor;Screen Color;2;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
WireConnection;14;0;20;0
WireConnection;18;0;14;0
WireConnection;18;1;4;0
WireConnection;18;2;19;0
WireConnection;2;0;1;0
WireConnection;20;0;2;0
WireConnection;21;0;18;0
WireConnection;21;1;22;0
WireConnection;21;2;23;0
WireConnection;0;0;21;0
ASEEND*/
//CHKSM=28E29530F55695ED61FFA1DA30DD271A168E93DC