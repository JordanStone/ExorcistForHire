Shader "Custom/BlendShader" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_Opacity("Blend", Range(0,100)) = 0
		
		_TextOne("TextureOne (RGB)", 2D) = "white" {}
		_TextTwo("TextureTwo (RGB)", 2D) = "white" {}
		
		_NormalOne("NormalOne", 2D) = "bump" {}
		_NormalTwo("NormalTwo", 2D) = "bump" {}
		
		
		
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 300
		
		CGPROGRAM
		

		#include "UnityCG.cginc"
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		

		sampler2D _TextOne;
		sampler2D _TextTwo;

		sampler2D _NormalOne;
		sampler2D _NormalTwo;
		
		struct Input 
		{
			float2 uv_TextOne;
			float2 uv_TextTwo;
			float2 uv_NormalOne;
			float2 uv_NormalTwo;
			float3 WorldSpaceViewDir;
		};
		
	
		
		
		half _Opacity;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			//Weighted Average for Albedo
			// Albedo comes from a texture tinted by color
			fixed4 b = tex2D (_TextOne, IN.uv_TextOne) * _Color;
			fixed4 c = tex2D (_TextTwo, IN.uv_TextTwo) * _Color;
			fixed4 d = ( (b *( abs(_Opacity-100)/100) )   +  (c * (_Opacity/100) ) );
			//d = d * WorldSpaceViewDir;
		
			fixed3 NormalA = UnpackNormal (tex2D (_NormalOne, IN.uv_NormalOne));
			fixed3 NormalB = UnpackNormal (tex2D (_NormalTwo, IN.uv_NormalTwo));
			fixed3 NormalC = ( (NormalA *( abs(_Opacity-100)/100) )   +  (NormalB * (_Opacity/100) ) );
			
			o.Albedo = d.rgb;
			o.Normal = NormalC;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = d.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
