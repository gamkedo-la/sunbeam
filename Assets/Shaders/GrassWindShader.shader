Shader "Custom/GrassWindShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		[NoScaleOffset] _MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
		[HideInInspector] _dir("direction", Color) = (0, 0, 0, 0)
	}
	SubShader
	{
		Tags{ "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" "IgnoreProjector" = "True" }
		LOD 200
		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows alphatest:_Cutoff addshadow vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		float4 _Color;
		float _Glossiness;
		float _Metallic;
		float4 _dir;
	
		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};
	
		void vert(inout appdata_full v)
		{
			v.vertex.xyz += _dir.xyz * v.texcoord.y;
		}
	
		void surf(Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	
			o.Albedo = c.rgb;
			o.Alpha = c.a;
	
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}

		ENDCG
		UsePass "Standard/SHADOWCASTER"
	}
	Fallback "VertexLit"
}
