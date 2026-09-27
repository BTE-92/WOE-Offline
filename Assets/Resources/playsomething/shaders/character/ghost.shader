Shader "Character/Ghost Shader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap ("Bumpmap", 2D) = "bump" {}
        _RimColor ("Rim Color", Color) = (0.46,0,1,0)
        _RimPower ("Rim Power", Range(0.2,2)) = 0.5
        _Brightness ("Brightness", Range(0,3)) = 1
    }

    SubShader { 
        Tags { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
        }

        // ============================================================
        // PASS 1: Z-Prepass (Depth-only write to prevent self-overlap)
        // ============================================================
        Pass {
            Tags { 
                "Queue"="Transparent" 
                "IgnoreProjector"="True" 
                "RenderType"="Transparent" 
            }
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
                "Queue"="Transparent" 
                "IgnoreProjector"="True" 
                "RenderType"="Transparent" 
            }
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
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            float4 _RimColor;
            float _RimPower;
            float _Brightness;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;       // xy = _MainTex, zw = _BumpMap
                float3 viewDir : TEXCOORD1;  // Tangent-space view direction
                float3 lightDir : TEXCOORD2; // Tangent-space light direction
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

                // Build tangent space transformation matrix
                TANGENT_SPACE_ROTATION;
                o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));
                o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));

                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 1. Grayscale base diffuse texture
                fixed4 basecol = tex2D(_MainTex, i.uv.xy);
                half gray = dot(basecol.rgb, half3(0.3, 0.59, 0.11));
                half3 diffColor = half3(gray, gray, gray);

                // 2. Unpack normal in tangent space
                half3 normalTangent = UnpackNormal(tex2D(_BumpMap, i.uv.zw));

                // 3. Rim Lighting & Alpha calculation
                float3 viewDir = normalize(i.viewDir);
                float NdotV = saturate(dot(viewDir, normalTangent));
                float rim = 1.0 - NdotV;
                half3 rimColor = _RimColor.rgb * pow(rim, _RimPower) * _Brightness;
                half alpha = (rimColor.r + rimColor.g + rimColor.b) / 3.0;

                // 4. Diffuse Lambert Lighting
                float3 lightDir = normalize(i.lightDir);
                float NdotL = max(0.0, dot(normalTangent, lightDir));
                half3 directLight = diffColor * _LightColor0.rgb * (NdotL * 2.0);

                // 5. Final output
                fixed4 c;
                c.rgb = directLight + rimColor;
                c.a = alpha;
                return c;
            }
            ENDCG
        }

        // ============================================================
        // PASS 3: FORWARD ADD PASS (Point / Spot / Directional Additive)
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { 
                "LightMode"="ForwardAdd" 
                "Queue"="Transparent" 
                "IgnoreProjector"="True" 
                "RenderType"="Transparent" 
            }
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
            float _Brightness;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                float3 lightDir : TEXCOORD2;
                LIGHTING_COORDS(3, 4)
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

                TANGENT_SPACE_ROTATION;
                o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));
                o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 1. Grayscale base diffuse texture
                fixed4 basecol = tex2D(_MainTex, i.uv.xy);
                half gray = dot(basecol.rgb, half3(0.3, 0.59, 0.11));
                half3 diffColor = half3(gray, gray, gray);

                // 2. Unpack normal in tangent space
                half3 normalTangent = UnpackNormal(tex2D(_BumpMap, i.uv.zw));

                // 3. Rim Lighting for alpha modulation
                float3 viewDir = normalize(i.viewDir);
                float NdotV = saturate(dot(viewDir, normalTangent));
                float rim = 1.0 - NdotV;
                half3 rimColor = _RimColor.rgb * pow(rim, _RimPower) * _Brightness;
                half alpha = (rimColor.r + rimColor.g + rimColor.b) / 3.0;

                // 4. Additive Light with Attenuation
                float3 lightDir = normalize(i.lightDir);
                float NdotL = max(0.0, dot(normalTangent, lightDir));
                fixed atten = LIGHT_ATTENUATION(i);

                // In ForwardAdd, rimColor is not added to RGB, only used for alpha
                fixed4 c;
                c.rgb = diffColor * _LightColor0.rgb * (NdotL * atten * 2.0);
                c.a = alpha;
                return c;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}