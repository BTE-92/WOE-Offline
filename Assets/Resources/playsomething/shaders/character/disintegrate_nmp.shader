Shader "Character/Disintegrate Bumped Diffuse" {
Properties {
 _MainTex ("Texture (RGB)", 2D) = "white" {}
 _BumpMap ("Texture (RGB)", 2D) = "bump" {}
 _NoiseTex ("Effect Map (RGB)", 2D) = "white" {}
 _DisintegrateAmount ("Effect Amount", Range(0,1.01)) = 0
 _DissolveColor ("Edge Color", Color) = (1,0.5,0.2,0)
 _EdgeEmission ("Edge Emission", Color) = (0,0,0,0)
 _DissolveEdge ("Edge Range", Range(0,0.1)) = 0.01
 _TileFactor ("Tile Factor", Range(0,4)) = 1
}
	//DummyShaderTextExporter
	
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Lambert
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutput o)
		{
			float4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
}