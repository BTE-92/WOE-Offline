Shader "Character/Hologram" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap ("Bumpmap", 2D) = "bump" {}
        _RimColor ("Rim Color", Color) = (0.26,0.7,1,0)
        _RimPower ("Rim Power", Range(0.1,8)) = 3
        _ClipPower ("Clip Power", Range(0,301)) = 200
        _Brightness ("Brightness", Range(0,3)) = 1.5
        _DiffuseAmount ("Diffuse Amount", Range(0,1)) = 0
    }

    SubShader { 
        Tags { 
            "Queue"="Transparent+1000" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
        }

        // ============================================================
        // PASS 1: Depth Pass (Z-Test Always / ColorMask 0)
        // ============================================================
        Pass {
            Tags { 
                "Queue"="Transparent+1000" 
                "IgnoreProjector"="True" 
                "RenderType"="Transparent" 
            }
            ZTest Always
            ZWrite On
            ColorMask 0
        }

        // ============================================================
        // PASS 2: FORWARD BASE PASS
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { 
                "LightMode"="ForwardBase" 
                "Queue"="Transparent+1000" 
                "IgnoreProjector"="True" 
                "RenderType"="Transparent" 
            }
            ZTest Always
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            AlphaTest Greater 0
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase nodirlightmap nolightmap

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            float4 _RimColor;
            float _RimPower;
            float _ClipPower;
            float _Brightness;
            float _DiffuseAmount;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;          // xy = _MainTex, zw = _BumpMap
                float4 screenPos : TEXCOORD1;   // For screen-space clipping
                float3 viewDir : TEXCOORD2;     // Tangent-space view direction
                float3 lightDir : TEXCOORD3;    // Tangent-space light direction
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

                o.screenPos = ComputeScreenPos(o.pos);

                // Build tangent space transformation matrix
                TANGENT_SPACE_ROTATION;
                o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));
                o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));

                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 1. Hologram scanline clipping
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                if (_ClipPower <= 300.0) {
                    float scanline = frac(screenUV.y * _ClipPower) - 0.5;
                    if (scanline < 0.0) {
                        discard;
                    }
                }

                // 2. Texture & Normal setup
                fixed4 basecol = tex2D(_MainTex, i.uv.xy);
                half3 normalTangent = UnpackNormal(tex2D(_BumpMap, i.uv.zw));

                // 3. Rim Lighting & Blending
                float3 viewDir = normalize(i.viewDir);
                float rim = 1.0 - saturate(dot(viewDir, normalTangent));
                half3 rimColor = _RimColor.rgb * pow(rim, _RimPower) * _Brightness;
                half3 mixedColor = lerp(rimColor, basecol.rgb, _DiffuseAmount);

                // 4. Lambertian Diffuse Lighting
                float3 lightDir = normalize(i.lightDir);
                float NdotL = max(0.0, dot(normalTangent, lightDir));
                half3 directLight = basecol.rgb * _LightColor0.rgb * NdotL;

                // 5. Final Composition
                fixed4 c;
                c.rgb = directLight + mixedColor;
                c.a = mixedColor.r; // Red channel of blended color drives the alpha channel
                return c;
            }
            ENDCG
        }

        // ============================================================
        // PASS 3: FORWARD ADD PASS (Additive Lights)
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { 
                "LightMode"="ForwardAdd" 
                "Queue"="Transparent+1000" 
                "IgnoreProjector"="True" 
                "RenderType"="Transparent" 
            }
            ZTest Always
            ZWrite Off
            Fog { Color (0,0,0,0) }
            Blend SrcAlpha One
            AlphaTest Greater 0
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            float4 _RimColor;
            float _RimPower;
            float _ClipPower;
            float _Brightness;
            float _DiffuseAmount;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float3 lightDir : TEXCOORD3;
                LIGHTING_COORDS(4, 5)
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

                o.screenPos = ComputeScreenPos(o.pos);

                TANGENT_SPACE_ROTATION;
                
                // View and Light direction transformed to tangent space using standard helpers
                o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));
                o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 1. Hologram scanline clipping
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                if (_ClipPower <= 300.0) {
                    float scanline = frac(screenUV.y * _ClipPower) - 0.5;
                    if (scanline < 0.0) {
                        discard;
                    }
                }

                // 2. Texture & Normal setup
                fixed4 basecol = tex2D(_MainTex, i.uv.xy);
                half3 normalTangent = UnpackNormal(tex2D(_BumpMap, i.uv.zw));

                // 3. Rim Lighting (Used solely to evaluate Alpha composition, not added to RGB in ForwardAdd)
                float3 viewDir = normalize(i.viewDir);
                float rim = 1.0 - saturate(dot(viewDir, normalTangent));
                half3 rimColor = _RimColor.rgb * pow(rim, _RimPower) * _Brightness;
                half3 mixedColor = lerp(rimColor, basecol.rgb, _DiffuseAmount);

                // 4. Additive Light with Attenuation
                float3 lightDir = normalize(i.lightDir);
                float NdotL = max(0.0, dot(normalTangent, lightDir));
                fixed atten = LIGHT_ATTENUATION(i);

                // 5. Output composition (diffuse lighting only, rim color is already added in ForwardBase)
                fixed4 c;
                c.rgb = basecol.rgb * _LightColor0.rgb * (NdotL * atten);
                c.a = mixedColor.r;
                return c;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}