// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Hands_Ghost_NormalMapped"
{
	Properties
	{
		_FillColor("Fill Color", Color) = (0.240833,0.3318721,0.4150943,1)
		_GlowColor("Glow Color", Color) = (0.1367925,0.4676886,1,1)
		_LightSource("Light Source", Vector) = (0,1,0,0)
		_GlowAmount("Glow Amount", Range( 0 , 1)) = 0.4964203
		_Opacity("Opacity", Range( 0 , 1)) = 0.5
		_NormalMap("Normal Map", 2D) = "gray" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		ZWrite Off
		Blend SrcAlpha One
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
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
			o.Normal = float3(0,0,1);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float3 newWorldNormal9 = (WorldNormalVector( i , UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) ) ));
			float fresnelNdotV23 = dot( newWorldNormal9, ase_worldViewDir );
			float fresnelNode23 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV23, ( ( 1.0 - _GlowAmount ) * 4.0 ) ) );
			float dotResult14 = dot( _LightSource , newWorldNormal9 );
			o.Emission = ( ( fresnelNode23 * fresnelNode23 * fresnelNode23 * _GlowColor ) + ( ( dotResult14 + 1.0 ) * 0.5 * _FillColor * _Opacity ) ).rgb;
			float temp_output_29_0 = _Opacity;
			o.Alpha = temp_output_29_0;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17800
0;0;1920;1019;1749.547;887.1288;1.6;True;True
Node;AmplifyShaderEditor.RangedFloatNode;28;-1594.351,-755.1286;Float;False;Property;_GlowAmount;Glow Amount;4;0;Create;True;0;0;False;0;0.4964203;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;36;-1907.947,-128.7284;Inherit;True;Property;_NormalMap;Normal Map;6;0;Create;True;0;0;False;0;-1;None;None;True;0;False;gray;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;9;-1477.336,-111.545;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;11;-1034.934,-226.1445;Float;False;Property;_LightSource;Light Source;3;0;Create;True;0;0;False;0;0,1,0;0,-1,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OneMinusNode;32;-1248.751,-748.7286;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-1247.151,-513.528;Float;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;False;0;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-767.7098,-27.52363;Float;False;Constant;_One;One;5;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;14;-763.9296,-137.7128;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-1000.75,-660.7288;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;10;-669.0994,-526.9858;Float;False;Property;_GlowColor;Glow Color;1;0;Create;True;0;0;False;0;0.1367925,0.4676886,1,1;0.254,0.5150999,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;19;-533.8098,-23.52346;Float;False;Constant;_Half;Half;5;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;15;-521.5986,-131.9693;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-605.8018,225.7373;Float;False;Property;_FillColor;Fill Color;0;0;Create;True;0;0;False;0;0.240833,0.3318721,0.4150943,1;0.9150943,0.7636712,0.608624,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;23;-739.9477,-753.528;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-627.949,427.2716;Float;False;Property;_Opacity;Opacity;5;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-191.4306,-28.857;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-263.1487,-641.5275;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;274.4501,-271.9282;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;593.4995,-317.7998;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Hands_Ghost_NormalMapped;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Custom;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;8;5;False;-1;1;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;2;-1;-1;-1;0;False;0;0;False;-1;0;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;36;0
WireConnection;32;0;28;0
WireConnection;14;0;11;0
WireConnection;14;1;9;0
WireConnection;33;0;32;0
WireConnection;33;1;34;0
WireConnection;15;0;14;0
WireConnection;15;1;17;0
WireConnection;23;0;9;0
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
//CHKSM=7F2D75C9A8FC62FC73C0507F4D06A06A36D1E0E5