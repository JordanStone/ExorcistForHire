Shader "Hidden/TextShader_NoZTest"
{
	Properties 
	{
		_MainTex ("Font Texture", 2D) = "white" {}
		_Color ("Text Color", Color) = (1,1,1,1)
	}
	
	SubShader 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Lighting On Cull Back ZWrite On Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		
		//#pragma surface
		
		Pass 
		{
			Color [_Color]
			SetTexture [_MainTex] 
			{
				combine primary, texture * primary
			}
		}
	}
}
