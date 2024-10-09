// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Mirza Beig/Lightning VFX//Lightning"
{
	Properties
	{
		[Toggle]_ParticleMode("Particle Mode", Float) = 1
		[HDR]_Colour("Colour", Color) = (1,1,1,1)
		_AlphaErosion("Alpha Erosion", Range( 0 , 1)) = 0
		_RemapMin("Remap Min", Range( 0 , 1)) = 0
		_RemapMax("Remap Max", Range( 0 , 1)) = 0
		_AlbedoTexture("Albedo Texture", 2D) = "white" {}
		_Duplicate("Duplicate", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _AlbedoTexture;
		uniform float _Duplicate;
		uniform float4 _Colour;
		uniform float _ParticleMode;
		uniform float _AlphaErosion;
		uniform float _RemapMin;
		uniform float _RemapMax;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float ParticleStableRandom42 = i.uv_texcoord.w;
			float2 appendResult40 = (float2(0.0 , ParticleStableRandom42));
			float ifLocalVar63 = 0;
			if( ParticleStableRandom42 <= 0.5 )
				ifLocalVar63 = 1.0;
			else
				ifLocalVar63 = -1.0;
			float2 appendResult54 = (float2(0.0 , ( ParticleStableRandom42 - 0.5 )));
			float4 AlbedoTextureSample48 = saturate( ( tex2D( _AlbedoTexture, ( i.uv_texcoord.xy + appendResult40 ) ) + ( tex2D( _AlbedoTexture, ( ( i.uv_texcoord.xy * ifLocalVar63 ) + appendResult54 ) ) * _Duplicate ) ) );
			float4 temp_output_5_0 = ( AlbedoTextureSample48 * i.vertexColor * _Colour );
			float3 ColourRGB4 = (temp_output_5_0).rgb;
			o.Emission = ColourRGB4;
			float ColourA10 = (temp_output_5_0).a;
			float ParticleErosionOverLifetime30 = i.uv_texcoord.z;
			float temp_output_1_0_g2 = _RemapMin;
			float Opacity23 = saturate( ( ( saturate( ( ColourA10 - (( _ParticleMode )?( ParticleErosionOverLifetime30 ):( _AlphaErosion )) ) ) - temp_output_1_0_g2 ) / ( _RemapMax - temp_output_1_0_g2 ) ) );
			o.Alpha = Opacity23;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19108
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Mirza Beig/Lightning VFX//Lightning;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;30;-4532.04,894.6838;Inherit;False;ParticleErosionOverLifetime;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;42;-4529.63,1039.647;Inherit;False;ParticleStableRandom;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;29;-4875.187,892.0133;Inherit;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;45;-2834.576,-1077.884;Inherit;True;Property;_AlbedoTexture;Albedo Texture;5;0;Create;True;0;0;0;False;0;False;None;0ad415cb683dc65438c8fd22c34a1500;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RegisterLocalVarNode;46;-2546.912,-1080.454;Inherit;False;AlbedoTexture;-1;True;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-2410.602,843.3228;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;17;-2867.359,1045.198;Inherit;False;Property;_Colour;Colour;1;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;8,8,8,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;7;-2852.733,852.0948;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;2;-2125.706,785.3329;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;9;-2123.373,920.113;Inherit;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;4;-1814.993,781.9593;Inherit;False;ColourRGB;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-1814.059,918.3372;Inherit;False;ColourA;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;49;-2897.91,714.2134;Inherit;False;48;AlbedoTextureSample;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-2546.521,-433.6447;Inherit;True;Property;_Albedo;Albedo;2;0;Create;True;0;0;0;False;0;False;-1;0ad415cb683dc65438c8fd22c34a1500;0ad415cb683dc65438c8fd22c34a1500;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;50;-2541.193,-168.293;Inherit;True;Property;_Albedo1;Albedo;2;0;Create;True;0;0;0;False;0;False;-1;0ad415cb683dc65438c8fd22c34a1500;0ad415cb683dc65438c8fd22c34a1500;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;47;-2828.424,-556.1292;Inherit;False;46;AlbedoTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;51;-2803.25,-170.4918;Inherit;False;46;AlbedoTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;34;-3118.899,-463.8468;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;35;-2770.899,-408.8468;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;40;-3031.5,-289.6468;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;53;-2784.015,-36.08008;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;54;-3044.616,83.11993;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;43;-3454.442,-269.197;Inherit;False;42;ParticleStableRandom;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-3083.363,-61.91193;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;52;-3505.815,-122.2801;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;58;-3427.291,214.0071;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-3803.417,177.1858;Inherit;False;Constant;_Float0;Float 0;6;0;Create;True;0;0;0;False;0;False;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-3804.417,269.1858;Inherit;False;Constant;_Float1;Float 0;6;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;63;-3464.289,22.70003;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;55;-3887.397,4.424606;Inherit;False;42;ParticleStableRandom;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-3866.417,94.18584;Inherit;False;Constant;_Float2;Float 2;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-2552.866,71.40936;Inherit;False;Property;_Duplicate;Duplicate;6;0;Create;True;0;0;0;False;0;False;0;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-2079.845,-160.9764;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;56;-1810.38,-351.0695;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;57;-1582.219,-369.7292;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;48;-1321.7,-371.4484;Inherit;False;AlbedoTextureSample;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;20;-2173.595,2325.791;Inherit;False;Inverse Lerp;-1;;2;09cbe79402f023141a4dc1fddd4c9511;0;3;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-2658.175,2289.733;Inherit;False;Property;_RemapMin;Remap Min;3;0;Create;True;0;0;0;False;0;False;0;0.065;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-2649.944,2398.179;Inherit;False;Property;_RemapMax;Remap Max;4;0;Create;True;0;0;0;False;0;False;0;0.4;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;14;-2687.626,2566.406;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;15;-2481.626,2566.406;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;11;-2958.83,2508.922;Inherit;False;10;ColourA;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;21;-1934.944,2325.179;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;23;-1736.821,2317.068;Inherit;False;Opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;33;-3033.589,2664.47;Inherit;False;Property;_ParticleMode;Particle Mode;0;0;Create;True;0;0;0;False;0;False;1;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;31;-3473.074,2765.492;Inherit;False;30;ParticleErosionOverLifetime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-3464.127,2639.93;Inherit;False;Property;_AlphaErosion;Alpha Erosion;2;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;72;-2836.96,1698.101;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-2514.049,1571.927;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;69;-2862.788,1511.097;Inherit;False;Constant;_Emission;Emission;7;0;Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;73;-2284.102,1569.408;Inherit;False;Emission_TEST_ONLY;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;8;-358.2879,-52.5931;Inherit;False;4;ColourRGB;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;24;-370.0051,267.8712;Inherit;False;23;Opacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;74;-394.8003,69.01295;Inherit;False;73;Emission_TEST_ONLY;1;0;OBJECT;;False;1;COLOR;0
WireConnection;0;2;8;0
WireConnection;0;9;24;0
WireConnection;30;0;29;3
WireConnection;42;0;29;4
WireConnection;46;0;45;0
WireConnection;5;0;49;0
WireConnection;5;1;7;0
WireConnection;5;2;17;0
WireConnection;2;0;5;0
WireConnection;9;0;5;0
WireConnection;4;0;2;0
WireConnection;10;0;9;0
WireConnection;1;0;47;0
WireConnection;1;1;35;0
WireConnection;50;0;51;0
WireConnection;50;1;53;0
WireConnection;35;0;34;0
WireConnection;35;1;40;0
WireConnection;40;1;43;0
WireConnection;53;0;62;0
WireConnection;53;1;54;0
WireConnection;54;1;58;0
WireConnection;62;0;52;0
WireConnection;62;1;63;0
WireConnection;58;0;55;0
WireConnection;63;0;55;0
WireConnection;63;2;64;0
WireConnection;63;3;65;0
WireConnection;63;4;65;0
WireConnection;67;0;50;0
WireConnection;67;1;68;0
WireConnection;56;0;1;0
WireConnection;56;1;67;0
WireConnection;57;0;56;0
WireConnection;48;0;57;0
WireConnection;20;1;19;0
WireConnection;20;2;22;0
WireConnection;20;3;15;0
WireConnection;14;0;11;0
WireConnection;14;1;33;0
WireConnection;15;0;14;0
WireConnection;21;0;20;0
WireConnection;23;0;21;0
WireConnection;33;0;13;0
WireConnection;33;1;31;0
WireConnection;71;0;69;0
WireConnection;71;1;72;0
WireConnection;73;0;71;0
ASEEND*/
//CHKSM=17613DEA8859CF7456F971F28083279A7BD24E31