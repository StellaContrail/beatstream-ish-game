Shader "GUI/Outline Text Shader" {
	Properties {
		_MainTex ("Font Texture", 2D) = "white" {}
		_Color ("Text Color", Color) = (1,1,1,1)
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_OuterThickness ("Outer Thickness", Float) = 0.01
		_InnerThickness ("Inner Thickness", Vector) = (0,0,0,0)
	}

	SubShader {

		Tags {
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
		}
		Lighting Off Cull Off ZTest Always ZWrite Off Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert_o
			#pragma fragment frag_o
			#define OUTLINE_THICKNESS_X -1
			#define OUTLINE_THICKNESS_Y -1
			#include "OutlineFont.cginc"
			ENDCG 
		}

		Pass {
			CGPROGRAM
			#pragma vertex vert_o
			#pragma fragment frag_o
			#define OUTLINE_THICKNESS_X 0
			#define OUTLINE_THICKNESS_Y -1
			#include "OutlineFont.cginc"
			ENDCG 
		}

		Pass {
			CGPROGRAM
			#pragma vertex vert_o
			#pragma fragment frag_o
			#define OUTLINE_THICKNESS_X 1
			#define OUTLINE_THICKNESS_Y -1
			#include "OutlineFont.cginc"
			ENDCG 
		}

		Pass {
			CGPROGRAM
			#pragma vertex vert_o
			#pragma fragment frag_o
			#define OUTLINE_THICKNESS_X -1
			#define OUTLINE_THICKNESS_Y 0
			#include "OutlineFont.cginc"
			ENDCG 
		}

		Pass {
			CGPROGRAM
			#pragma vertex vert_o
			#pragma fragment frag_o
			#define OUTLINE_THICKNESS_X 1
			#define OUTLINE_THICKNESS_Y 0
			#include "OutlineFont.cginc"
			ENDCG 
		}

		Pass {
			CGPROGRAM
			#pragma vertex vert_o
			#pragma fragment frag_o
			#define OUTLINE_THICKNESS_X -1
			#define OUTLINE_THICKNESS_Y 1
			#include "OutlineFont.cginc"
			ENDCG 
		}

		Pass {
			CGPROGRAM
			#pragma vertex vert_o
			#pragma fragment frag_o
			#define OUTLINE_THICKNESS_X 0
			#define OUTLINE_THICKNESS_Y 1
			#include "OutlineFont.cginc"
			ENDCG 
		}

		Pass {
			CGPROGRAM
			#pragma vertex vert_o
			#pragma fragment frag_o
			#define OUTLINE_THICKNESS_X 1
			#define OUTLINE_THICKNESS_Y 1
			#include "OutlineFont.cginc"
			ENDCG 
		}

		Pass {	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define OUTLINE_THICKNESS_X 0
			#define OUTLINE_THICKNESS_Y 0
			#include "OutlineFont.cginc"
			ENDCG 
		}
	}
}
