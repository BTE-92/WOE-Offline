Shader "Character/CharShader-lambertwrap" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("MaskTex (RGBA)", 2D) = "black" {}
        _EyeColor ("Eye Color", Color) = (0.5,0.5,0.5,1)
        _SkinColor ("Skin Color", Color) = (0.5,0.5,0.5,1)
        _HairColor ("Hair Color", Color) = (0.5,0.5,0.5,1)
    }

    SubShader { 
        Tags { "RenderType"="Opaque" }
        LOD 200

        // ============================================================
        // FORWARD BASE PASS (Direct Light, SH Ambient, Shadows)
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" "RenderType"="Opaque" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MaskTex;
            float4 _MaskTex_ST;
            fixed4 _EyeColor;
            fixed4 _SkinColor;
            fixed4 _HairColor;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                #ifndef LIGHTMAP_ON
                fixed3 shLight : COLOR0;
                #endif
                SHADOW_COORDS(2)
            };

            v2f vert (appdata v) {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldNormal = worldNormal;

                #ifndef LIGHTMAP_ON
                o.shLight = ShadeSH9(float4(worldNormal, 1.0));
                #endif

                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 1. Samples base and mask colors
                half4 basecol = tex2D(_MainTex, i.uv);
                half4 maskcol = tex2D(_MaskTex, i.uv);

                // 2. Eyes Mask Blend (Red channel)
                if (maskcol.r > 0.0) {
                    half luminance = dot(basecol.rgb, half3(0.3, 0.59, 0.11));
                    half3 eyeBlend = luminance * 2.0 * _EyeColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, eyeBlend, maskcol.r);
                }

                // 3. Skin Mask Blend (Green channel)
                if (maskcol.g > 0.0) {
                    half3 skinBlend = basecol.rgb * 2.0 * _SkinColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, skinBlend, maskcol.g);
                }

                // 4. Hair Mask Blend (Blue channel)
                if (maskcol.b > 0.0) {
                    half3 hairBlend = basecol.rgb * 2.0 * _HairColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, hairBlend, maskcol.b);
                }

                half3 albedo = basecol.rgb;

                // 5. Lighting Math (Wrapped Half-Lambert with Shadows & SH Ambient)
                float3 normal = normalize(i.worldNormal);
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                fixed shadow = SHADOW_ATTENUATION(i);

                // Standard Half-lambert wrapping formulation: (N·L * 0.5 + 0.5)
                half NdotLWrap = dot(normal, lightDir) * 0.5 + 0.5;

                fixed4 c;
                c.rgb = (albedo * _LightColor0.rgb) * (NdotLWrap * shadow);

                #ifndef LIGHTMAP_ON
                c.rgb += albedo * i.shLight;
                #endif

                c.a = 0.0;
                return c;
            }
            ENDCG
        }

        // ============================================================
        // FORWARD ADD PASS (Additive Lights)
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LightMode"="ForwardAdd" "RenderType"="Opaque" }
            ZWrite Off
            Fog { Color (0,0,0,0) }
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd_fullshadows

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MaskTex;
            float4 _MaskTex_ST;
            fixed4 _EyeColor;
            fixed4 _SkinColor;
            fixed4 _HairColor;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 lightDir : TEXCOORD2;
                LIGHTING_COORDS(3, 4)
            };

            v2f vert (appdata v) {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.lightDir = _WorldSpaceLightPos0.xyz - worldPos * _WorldSpaceLightPos0.w;

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half4 basecol = tex2D(_MainTex, i.uv);
                half4 maskcol = tex2D(_MaskTex, i.uv);

                if (maskcol.r > 0.0) {
                    half luminance = dot(basecol.rgb, half3(0.3, 0.59, 0.11));
                    half3 eyeBlend = luminance * 2.0 * _EyeColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, eyeBlend, maskcol.r);
                }
                if (maskcol.g > 0.0) {
                    half3 skinBlend = basecol.rgb * 2.0 * _SkinColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, skinBlend, maskcol.g);
                }
                if (maskcol.b > 0.0) {
                    half3 hairBlend = basecol.rgb * 2.0 * _HairColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, hairBlend, maskcol.b);
                }

                half3 albedo = basecol.rgb;

                float3 normal = normalize(i.worldNormal);
                float3 lightDir = normalize(i.lightDir);
                fixed atten = LIGHT_ATTENUATION(i);

                half NdotLWrap = dot(normal, lightDir) * 0.5 + 0.5;

                fixed4 c;
                c.rgb = (albedo * _LightColor0.rgb) * (NdotLWrap * atten);
                c.a = 0.0;
                return c;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}