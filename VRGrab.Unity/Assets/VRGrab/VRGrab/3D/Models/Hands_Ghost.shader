// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Hands_Ghost"
{
	Properties
	{
		_FillColor("Fill Color", Color) = (0.240833,0.3318721,0.4150943,1)
		_GlowColor("Glow Color", Color) = (0.1367925,0.4676886,1,1)
		_LightSource("Light Source", Vector) = (0,1,0,0)
		_GlowAmount("Glow Amount", Range( 0 , 1)) = 0.4964203
		_Opacity("Opacity", Range( 0 , 1)) = 0.5
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha One
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float _GlowAmount;
		uniform float4 _GlowColor;
		uniform float3 _LightSource;
		uniform float4 _FillColor;
		uniform float _Opacity;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV23 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode23 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV23, ( ( 1.0 - _GlowAmount ) * 4.0 ) ) );
			float dotResult14 = dot( _LightSource , ase_worldNormal );
			o.Emission = ( ( fresnelNode23 * fresnelNode23 * fresnelNode23 * _GlowColor ) + ( ( dotResult14 + 1.0 ) * 0.5 * _FillColor * _Opacity ) ).rgb;
			float temp_output_29_0 = _Opacity;
			o.Alpha = temp_output_29_0;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16700
-3;11;1918;1017;2261.546;831.9291;1.6;True;False
Node;AmplifyShaderEditor.RangedFloatNode;28;-1594.351,-755.1286;Float;False;Property;_GlowAmount;Glow Amount;4;0;Create;True;0;0;False;0;0.4964203;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;9;-1086.935,18.05496;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;34;-1247.151,-513.528;Float;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;False;0;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;32;-1248.751,-748.7286;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;11;-1086.135,-274.1447;Float;False;Property;_LightSource;Light Source;3;0;Create;True;0;0;False;0;0,1,0;0,-1,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;14;-802.3292,-257.7131;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-1000.75,-660.7288;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-767.7098,-27.52363;Float;False;Constant;_One;One;5;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;15;-559.9982,-251.9696;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-533.8098,-23.52346;Float;False;Constant;_Half;Half;5;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;23;-739.9477,-753.528;Float;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-627.949,427.2716;Float;False;Property;_Opacity;Opacity;5;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;10;-669.0994,-526.9858;Float;False;Property;_GlowColor;Glow Color;1;0;Create;True;0;0;False;0;0.1367925,0.4676886,1,1;0.254,0.5150999,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1;-605.8018,225.7373;Float;False;Property;_FillColor;Fill Color;0;0;Create;True;0;0;False;0;0.240833,0.3318721,0.4150943,1;0.9150943,0.7636712,0.608624,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-191.4306,-28.857;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-263.1487,-641.5275;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;274.4501,-271.9282;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;593.4995,-317.7998;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Hands_Ghost;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;False;Custom;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;8;5;False;-1;1;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;2;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;32;0;28;0
WireConnection;14;0;11;0
WireConnection;14;1;9;0
WireConnection;33;0;32;0
WireConnection;33;1;34;0
WireConnection;15;0;14;0
WireConnection;15;1;17;0
WireConnection;23;3;33;0
WireConnection;22;0;15;0
WireConnection;22;1;19;0
WireConnection;22;2;1;0
WireConnection;22;3;29;0
WireConnection;24;0;23;0
WireConnection;24;1;23;0
WireConnection;24;2;23;0
WireConnection;24;3;10;0
WireConnection;27;0;24;0
WireConnection;27;1;22;0
WireConnection;0;2;27;0
WireConnection;0;9;29;0
ASEEND*/
//CHKSM=A70BE459039FF1699691319D8F4805F4E082C9C0