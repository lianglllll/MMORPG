// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Mirza Beig/Lightning VFX/Ground Crack"
{
	Properties
	{
		[HDR]_Colour("Colour", Color) = (1,1,1,1)
		_Albedo("Albedo", 2D) = "white" {}
		_AlphaErosionOffset("Alpha Erosion Offset", Range( 0 , 1)) = 0
		_RadialMaskPower("Radial Mask Power", Float) = 1
		[Toggle(_SUBTRACTIVERADIALMASK_ON)] _SubtractiveRadialMask("Subtractive Radial Mask", Float) = 0
		_Noise("Noise", Range( 0 , 1)) = 1
		_NoiseScale("Noise Scale", Float) = 0
		_NoiseTiling("Noise Tiling", Vector) = (1,1,0,0)
		_NoiseRemapMin("Noise Remap Min", Range( 0 , 1)) = 0
		_NoiseRemapMax("Noise Remap Max", Range( 0 , 1)) = 1
		_NoisePower("Noise Power", Range( 0.1 , 10)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma shader_feature_local _SUBTRACTIVERADIALMASK_ON
		#pragma surface surf Unlit alpha:fade keepalpha noshadow exclude_path:deferred 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 vertexColor : COLOR;
			float4 uv_texcoord;
		};

		uniform float4 _Colour;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float _RadialMaskPower;
		uniform float _AlphaErosionOffset;
		uniform float _NoiseRemapMin;
		uniform float _NoiseRemapMax;
		uniform float2 _NoiseTiling;
		uniform float _NoiseScale;
		uniform float _NoisePower;
		uniform float _Noise;


		inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }

		inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }

		inline float valueNoise (float2 uv)
		{
			float2 i = floor(uv);
			float2 f = frac( uv );
			f = f* f * (3.0 - 2.0 * f);
			uv = abs( frac(uv) - 0.5);
			float2 c0 = i + float2( 0.0, 0.0 );
			float2 c1 = i + float2( 1.0, 0.0 );
			float2 c2 = i + float2( 0.0, 1.0 );
			float2 c3 = i + float2( 1.0, 1.0 );
			float r0 = noise_randomValue( c0 );
			float r1 = noise_randomValue( c1 );
			float r2 = noise_randomValue( c2 );
			float r3 = noise_randomValue( c3 );
			float bottomOfGrid = noise_interpolate( r0, r1, f.x );
			float topOfGrid = noise_interpolate( r2, r3, f.x );
			float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
			return t;
		}


		float SimpleNoise(float2 UV)
		{
			float t = 0.0;
			float freq = pow( 2.0, float( 0 ) );
			float amp = pow( 0.5, float( 3 - 0 ) );
			t += valueNoise( UV/freq )*amp;
			freq = pow(2.0, float(1));
			amp = pow(0.5, float(3-1));
			t += valueNoise( UV/freq )*amp;
			freq = pow(2.0, float(2));
			amp = pow(0.5, float(3-2));
			t += valueNoise( UV/freq )*amp;
			return t;
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 Albedo18 = ( _Colour * i.vertexColor );
			o.Emission = Albedo18.rgb;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode1 = tex2D( _Albedo, uv_Albedo );
			float2 uvs_TexCoord25 = i.uv_texcoord;
			uvs_TexCoord25.xy = i.uv_texcoord.xy * float2( 2,2 ) + float2( -1,-1 );
			float RadialMask27 = saturate( ( 1.0 - length( uvs_TexCoord25.xy ) ) );
			float temp_output_37_0 = pow( RadialMask27 , _RadialMaskPower );
			#ifdef _SUBTRACTIVERADIALMASK_ON
				float staticSwitch42 = saturate( ( tex2DNode1.a - ( 1.0 - temp_output_37_0 ) ) );
			#else
				float staticSwitch42 = saturate( ( tex2DNode1.a * temp_output_37_0 ) );
			#endif
			float ParticleAlphaErosionOverLifetime11 = i.uv_texcoord.z;
			float Alpha8 = saturate( ( staticSwitch42 - ( _AlphaErosionOffset + ParticleAlphaErosionOverLifetime11 ) ) );
			float ParticleStableRandomX58 = i.uv_texcoord.w;
			float2 temp_cast_1 = (( ParticleStableRandomX58 * 10.0 )).xx;
			float2 uvs_TexCoord44 = i.uv_texcoord;
			uvs_TexCoord44.xy = i.uv_texcoord.xy * _NoiseTiling + temp_cast_1;
			float simpleNoise43 = SimpleNoise( uvs_TexCoord44.xy*_NoiseScale );
			float smoothstepResult46 = smoothstep( _NoiseRemapMin , _NoiseRemapMax , simpleNoise43);
			float lerpResult56 = lerp( 1.0 , pow( smoothstepResult46 , _NoisePower ) , _Noise);
			float Noise49 = lerpResult56;
			o.Alpha = ( (Albedo18).a * Alpha8 * Noise49 );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19108
Node;AmplifyShaderEditor.TexCoordVertexDataNode;10;-4172.486,-639.7768;Inherit;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;25;-4433.455,-9.522856;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;2,2;False;1;FLOAT2;-1,-1;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LengthOpNode;24;-4156.455,-6.522856;Inherit;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;26;-3914.455,-7.522856;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;27;-3499.518,-14.64007;Inherit;False;RadialMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;21;-2420.253,-599.6035;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-2086.252,-723.6035;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;16;-2436.596,-835.4899;Inherit;False;Property;_Colour;Colour;0;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;32,32,32,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;18;-1815.409,-729.6035;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;22;-182.7518,-17.2118;Inherit;False;18;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;14;-220.2437,367.4062;Inherit;False;8;Alpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;31;-188.2797,164.7966;Inherit;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;463.9981,19.10884;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Mirza Beig/Lightning VFX/Ground Crack;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;160.7081,246.2605;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;30;-485.5742,164.3909;Inherit;False;18;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;35;-3711.121,-5.758148;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-3173.391,708.1844;Inherit;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;0;False;0;False;-1;None;cdcd600ebdb6d774f8138d3b36beb7c8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;39;-2518.11,996.29;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-2464.715,764.4012;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;36;-2145.688,807.2727;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;41;-2152.655,939.1084;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;42;-1890.203,898.0017;Inherit;False;Property;_SubtractiveRadialMask;Subtractive Radial Mask;4;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;3;-1405.072,1028.566;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;38;-1119.434,1025.959;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;37;-3350.552,1022.693;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-3736.283,1164.288;Inherit;False;Property;_RadialMaskPower;Radial Mask Power;3;0;Create;True;0;0;0;False;0;False;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;29;-3703.408,1035.167;Inherit;False;27;RadialMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;40;-2921.492,1085.722;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-2173.032,1111.33;Inherit;False;Property;_AlphaErosionOffset;Alpha Erosion Offset;2;0;Create;True;0;0;0;False;0;False;0;0.1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;12;-2194.762,1235.562;Inherit;False;11;ParticleAlphaErosionOverLifetime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;13;-1762.59,1150.617;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-3514.806,1941.067;Inherit;False;Property;_NoiseScale;Noise Scale;6;0;Create;True;0;0;0;False;0;False;0;50;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;43;-3200.69,1796.978;Inherit;True;Simple;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-3203.806,2053.066;Inherit;False;Property;_NoiseRemapMin;Noise Remap Min;8;0;Create;True;0;0;0;False;0;False;0;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;46;-2737.58,1962;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;50;-2416.469,2082.987;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;-894.2993,1020.137;Inherit;False;Alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;55;-193.7667,482.8648;Inherit;False;49;Noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;56;-1969.56,2050.194;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-2402.739,2222.372;Inherit;False;Property;_Noise;Noise;5;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;49;-1721.288,2050.938;Inherit;False;Noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-3205.806,2143.067;Inherit;False;Property;_NoiseRemapMax;Noise Remap Max;9;0;Create;True;0;0;0;False;0;False;1;0.6;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-2855.469,2237.987;Inherit;False;Property;_NoisePower;Noise Power;10;0;Create;True;0;0;0;False;0;False;1;0.4;0.1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;11;-3886.409,-571.9268;Inherit;False;ParticleAlphaErosionOverLifetime;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;58;-3882.709,-473.7922;Inherit;False;ParticleStableRandomX;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;44;-3530.806,1757.067;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;60;-4195.958,1860.984;Inherit;False;58;ParticleStableRandomX;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;52;-4119.986,1701.464;Inherit;False;Property;_NoiseTiling;Noise Tiling;7;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-3814.958,1899.984;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-4092.958,1975.984;Inherit;False;Constant;_Float0;Float 0;11;0;Create;True;0;0;0;False;0;False;10;100;0;0;0;1;FLOAT;0
WireConnection;24;0;25;0
WireConnection;26;0;24;0
WireConnection;27;0;35;0
WireConnection;19;0;16;0
WireConnection;19;1;21;0
WireConnection;18;0;19;0
WireConnection;31;0;30;0
WireConnection;0;2;22;0
WireConnection;0;9;32;0
WireConnection;32;0;31;0
WireConnection;32;1;14;0
WireConnection;32;2;55;0
WireConnection;35;0;26;0
WireConnection;39;0;1;4
WireConnection;39;1;40;0
WireConnection;28;0;1;4
WireConnection;28;1;37;0
WireConnection;36;0;28;0
WireConnection;41;0;39;0
WireConnection;42;1;36;0
WireConnection;42;0;41;0
WireConnection;3;0;42;0
WireConnection;3;1;13;0
WireConnection;38;0;3;0
WireConnection;37;0;29;0
WireConnection;37;1;34;0
WireConnection;40;0;37;0
WireConnection;13;0;7;0
WireConnection;13;1;12;0
WireConnection;43;0;44;0
WireConnection;43;1;45;0
WireConnection;46;0;43;0
WireConnection;46;1;47;0
WireConnection;46;2;48;0
WireConnection;50;0;46;0
WireConnection;50;1;51;0
WireConnection;8;0;38;0
WireConnection;56;1;50;0
WireConnection;56;2;57;0
WireConnection;49;0;56;0
WireConnection;11;0;10;3
WireConnection;58;0;10;4
WireConnection;44;0;52;0
WireConnection;44;1;59;0
WireConnection;59;0;60;0
WireConnection;59;1;62;0
ASEEND*/
//CHKSM=B566B3570432BF80D62CD24F758521C12D7435EF