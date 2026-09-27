// Disintegrate Bumped Diffuse
//
// Cg reconstruction of the decompiled GLES shader dump.
//
// WHY THIS IS A SURFACE SHADER, NOT HAND-WRITTEN VERTEX/FRAGMENT CODE:
// The decompiled GLSL contains six passes - ForwardBase, ForwardAdd, PrePassBase,
// PrePassFinal, ShadowCaster, ShadowCollector - and every one of them matches,
// line for line, the code Unity's surface shader compiler auto-generates from
// UnityCG.cginc / Lighting.cginc / AutoLight.cginc for a "surf Lambert" shader:
//   - the per-vertex ambient term is exactly ShadeSH9()
//   - the VERTEXLIGHT_ON block is exactly Shade4PointLights(unity_4LightPosX0, ...)
//   - the lit color is exactly LightingLambert() (Albedo * _LightColor0 * diff*atten*2),
//     with "+= Albedo*vlight" (ambient) and "+= Emission" appended only in ForwardBase
//   - PrePassFinal is exactly LightingLambert_PrePass() plus the HDR light-buffer decode
//   - shadow coords/attenuation match TRANSFER_SHADOW / LIGHT_ATTENUATION / SHADOW_ATTENUATION
//   - the ShadowCaster/ShadowCollector bodies match V2F_SHADOW_CASTER/COLLECTOR verbatim
//
// So instead of re-deriving all six passes by hand (and risking a mismatch with the
// macros), the correct + accurate conversion is the compact surface shader below.
// Unity's compiler expands it into the same six passes, using the same macros,
// automatically - which is exactly what "use Unity macros where things were inlined"
// means for a shader that was a surface shader to begin with.
//
// The only genuinely custom logic - the noise-driven dissolve cutout, edge color and
// edge emission - lives in surf(), matched 1:1 against the decompiled fragment math:
//   float edge = noise.r - _DisintegrateAmount;   // (tmpvar_5.xyz - _DisintegrateAmount).x
//   clip(edge);                                    // if (edge < 0) discard;
//   if (edge < _DissolveEdge && _DisintegrateAmount > 0) -> edge color + edge emission
//   else                                            -> normal _MainTex sampling

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

SubShader {
	Tags { "RenderType"="Opaque" }

	CGPROGRAM
	// "addshadow" forces Unity to route the surf() clip() into the auto-generated
	// ShadowCaster pass, which is exactly what the decompiled SHADOWCASTER pass does
	// (it repeats the same noise-threshold discard before writing depth).
	#pragma surface surf Lambert addshadow

	sampler2D _MainTex;
	sampler2D _BumpMap;
	sampler2D _NoiseTex;

	float _DisintegrateAmount;
	float4 _DissolveColor;
	float4 _EdgeEmission;
	float _DissolveEdge;
	float _TileFactor;

	struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMap;
	};

	void surf (Input IN, inout SurfaceOutput o) {
		// _NoiseTex reuses the _MainTex UV set, tiled by _TileFactor -
		// matches: P = xlv_TEXCOORD0.xy * _TileFactor; texture2D(_NoiseTex, P)
		fixed4 noise = tex2D(_NoiseTex, IN.uv_MainTex * _TileFactor);

		// matches: tmpvar_7 = (noise.xyz - _DisintegrateAmount).x
		float edge = noise.r - _DisintegrateAmount;

		// matches: if (tmpvar_7 < 0.0) discard;
		clip(edge);

		fixed3 albedo;
		fixed3 emission = fixed3(0,0,0);

		// matches: if (tmpvar_7 < _DissolveEdge && _DisintegrateAmount > 0.0) { edge color/emission } else { main tex }
		if (edge < _DissolveEdge && _DisintegrateAmount > 0.0) {
			albedo = _DissolveColor.rgb;
			emission = _EdgeEmission.rgb;
		} else {
			albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
		}

		o.Albedo = albedo;
		o.Emission = emission;

		// matches: (packednormal.xyz * 2.0) - 1.0 (the plain, non-DXT5nm unpack the
		// GLES compile shows) - UnpackNormal() picks that branch automatically on GLES.
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

		o.Alpha = 1;
	}
	ENDCG
}

Fallback "Disintegrate"
}
