// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Guerre/Airplane"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
		_TurnAngle("TurnAngle", float) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

			Pass
			{
			CGPROGRAM

	#pragma vertex AirplaneSpriteVert
	#pragma fragment AirplaneSpriteFrag
	#pragma target 2.0
	#pragma multi_compile_instancing
	#pragma multi_compile _ PIXELSNAP_ON
	#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
	#include "UnitySprites.cginc"

			uniform float _TurnAngle;

		v2f AirplaneSpriteVert(appdata_t IN)
		{
			v2f OUT;

			UNITY_SETUP_INSTANCE_ID(IN);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

#ifdef UNITY_INSTANCING_ENABLED
			IN.vertex.xy *= _Flip.xy;
#endif

			// OUT.vertex = UnityObjectToClipPos(IN.vertex);
			float4 pos = float4(IN.vertex.xyz, 1.0);
			OUT.vertex = mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, pos));
			OUT.texcoord = IN.texcoord;
			OUT.color = IN.color * _Color * _RendererColor;

#ifdef PIXELSNAP_ON
			OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif

			return OUT;
		}

		fixed4 AirplaneSpriteFrag(v2f IN) : SV_Target
		{
			float2 uv = IN.texcoord;
		
			/*float tens = 1;
			float dx = uv.x - 0.5;
			uv.x = 0.5 + dx * (1 + abs(_TurnAngle) * tens / 2);
			float dy = uv.y - 0.5;
			if (_TurnAngle < 0)
				uv.y = 0.5f + dy * (1 - _TurnAngle * uv.x * tens);
			else
				uv.y = 0.5f + dy * (1 - -_TurnAngle * (1 - uv.x) * tens);*/

			fixed4 c = SampleSpriteTexture(uv) * IN.color;
			c.rgb *= c.a;
			return c;
		}

		ENDCG
		}
	}
}
