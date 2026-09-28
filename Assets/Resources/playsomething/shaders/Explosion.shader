Shader "Custom/Explosion" {
    Properties {
        _RampTex ("Color Ramp", 2D) = "white" {}
        _DispTex ("Displacement Texture", 2D) = "gray" {}
        _Displacement ("Displacement", Range(0,1)) = 0.1
        _ChannelFactor ("ChannelFactor (r,g,b)", Vector) = (1,0,0,1)
        _Range ("Range (min,max)", Vector) = (0,0.5,0,1)
        _ClipRange ("ClipRange [0,1]", Float) = 0.8
    }

    SubShader { 
        LOD 300
        Tags { "RenderType"="Opaque" }

        // ============================================================
        // PASS 1: FORWARD BASE
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" "RenderType"="Opaque" }
            Cull Off

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_base
            #pragma fragment frag_base
            #pragma multi_compile_fwdbase
            #pragma fragmentoption ARB_precision_hint_fastest

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _RampTex;
            sampler2D _DispTex;
            float4 _DispTex_ST;
            float _Displacement;
            float4 _ChannelFactor;
            float4 _Range;
            float _ClipRange;

            struct appdata_exp {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f_base {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                fixed3 shLight : COLOR0;
                SHADOW_COORDS(2)
            };

            v2f_base vert_base (appdata_exp v) {
                v2f_base o;
                UNITY_INITIALIZE_OUTPUT(v2f_base, o);

                float3 norm = normalize(v.normal);

                // Vertex displacement along normal using displacement texture LOD 0
                float3 dcolor = tex2Dlod(_DispTex, float4(v.texcoord.xy, 0.0, 0.0)).rgb;
                float dispAmount = dot(dcolor, _ChannelFactor.xyz) * _Displacement;
                float4 displacedVertex = v.vertex;
                displacedVertex.xyz += norm * dispAmount;

                o.pos = UnityObjectToClipPos(displacedVertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _DispTex);

                // World normal for lighting (original normal transformed to world space)
                float3 worldNormal = UnityObjectToWorldNormal(norm);
                o.worldNormal = worldNormal;
                o.shLight = ShadeSH9(float4(worldNormal, 1.0));

                // Shadow coords from DISPLACED world position
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag_base (v2f_base i) : SV_Target {
                // Re-sample displacement in fragment for discard/ramp
                float3 dcolor = tex2D(_DispTex, i.uv).rgb;
                float dispVal = dot(dcolor, _ChannelFactor.xyz) * (_Range.y - _Range.x) + _Range.x;

                // Clip discard
                if (_ClipRange - dispVal < 0.0) {
                    discard;
                }

                // Ramp lookup: x = displacement value, y = 0.5
                float2 rampUV = float2(dispVal, 0.5);
                float4 rampColor = tex2D(_RampTex, rampUV);

                float3 diffColor = rampColor.rgb;
                float3 emissionColor = rampColor.rgb * rampColor.a;

                // Lighting
                float NdotL = max(0.0, dot(i.worldNormal, _WorldSpaceLightPos0.xyz));
                fixed shadow = SHADOW_ATTENUATION(i);

                fixed4 c;
                c.rgb = (diffColor * _LightColor0.rgb) * (NdotL * shadow)
                      + (diffColor * i.shLight)
                      + emissionColor;
                c.a = 0.0;
                return c;
            }
            ENDCG
        }

        // ============================================================
        // PASS 2: FORWARD ADD
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LightMode"="ForwardAdd" "RenderType"="Opaque" }
            ZWrite Off
            Cull Off
            Fog { Color (0,0,0,0) }
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_add
            #pragma fragment frag_add
            #pragma multi_compile_fwdadd_fullshadows
            #pragma fragmentoption ARB_precision_hint_fastest

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _RampTex;
            sampler2D _DispTex;
            float4 _DispTex_ST;
            float _Displacement;
            float4 _ChannelFactor;
            float4 _Range;
            float _ClipRange;

            struct appdata_exp {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f_add {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 lightDir : TEXCOORD2;
                LIGHTING_COORDS(3, 4)
            };

            v2f_add vert_add (appdata_exp v) {
                v2f_add o;
                UNITY_INITIALIZE_OUTPUT(v2f_add, o);

                float3 norm = normalize(v.normal);

                float3 dcolor = tex2Dlod(_DispTex, float4(v.texcoord.xy, 0.0, 0.0)).rgb;
                float dispAmount = dot(dcolor, _ChannelFactor.xyz) * _Displacement;
                float4 displacedVertex = v.vertex;
                displacedVertex.xyz += norm * dispAmount;

                o.pos = UnityObjectToClipPos(displacedVertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _DispTex);

                float3 worldNormal = UnityObjectToWorldNormal(norm);
                o.worldNormal = worldNormal;

                float3 worldPos = mul(unity_ObjectToWorld, displacedVertex).xyz;
                o.lightDir = _WorldSpaceLightPos0.xyz - worldPos * _WorldSpaceLightPos0.w;

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag_add (v2f_add i) : SV_Target {
                float3 dcolor = tex2D(_DispTex, i.uv).rgb;
                float dispVal = dot(dcolor, _ChannelFactor.xyz) * (_Range.y - _Range.x) + _Range.x;

                if (_ClipRange - dispVal < 0.0) {
                    discard;
                }

                float2 rampUV = float2(dispVal, 0.5);
                float4 rampColor = tex2D(_RampTex, rampUV);
                float3 diffColor = rampColor.rgb;

                float3 lightDir = normalize(i.lightDir);
                float NdotL = max(0.0, dot(i.worldNormal, lightDir));
                fixed atten = LIGHT_ATTENUATION(i);

                fixed4 c;
                c.rgb = (diffColor * _LightColor0.rgb) * (NdotL * atten);
                c.a = 0.0;
                return c;
            }
            ENDCG
        }

        // ============================================================
        // PASS 3: PREPASS BASE (Deferred Normals)
        // ============================================================
        Pass {
            Name "PREPASS"
            Tags { "LightMode"="PrePassBase" "RenderType"="Opaque" }
            Cull Off
            Fog { Mode Off }

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_prepass_base
            #pragma fragment frag_prepass_base

            #include "UnityCG.cginc"

            sampler2D _DispTex;
            float4 _DispTex_ST;
            float _Displacement;
            float4 _ChannelFactor;
            float4 _Range;
            float _ClipRange;

            struct appdata_exp {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f_prepass_base {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            v2f_prepass_base vert_prepass_base (appdata_exp v) {
                v2f_prepass_base o;

                float3 norm = normalize(v.normal);

                float3 dcolor = tex2Dlod(_DispTex, float4(v.texcoord.xy, 0.0, 0.0)).rgb;
                float dispAmount = dot(dcolor, _ChannelFactor.xyz) * _Displacement;
                float4 displacedVertex = v.vertex;
                displacedVertex.xyz += norm * dispAmount;

                o.pos = UnityObjectToClipPos(displacedVertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _DispTex);
                o.worldNormal = UnityObjectToWorldNormal(norm);

                return o;
            }

            fixed4 frag_prepass_base (v2f_prepass_base i) : SV_Target {
                float3 dcolor = tex2D(_DispTex, i.uv).rgb;
                float dispVal = dot(dcolor, _ChannelFactor.xyz) * (_Range.y - _Range.x) + _Range.x;

                if (_ClipRange - dispVal < 0.0) {
                    discard;
                }

                fixed4 res;
                res.xyz = i.worldNormal * 0.5 + 0.5;
                res.w = 0.0;
                return res;
            }
            ENDCG
        }

        // ============================================================
        // PASS 4: PREPASS FINAL (Deferred Combine)
        // ============================================================
        Pass {
            Name "PREPASS"
            Tags { "LightMode"="PrePassFinal" "RenderType"="Opaque" }
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_prepass_final
            #pragma fragment frag_prepass_final
            #pragma multi_compile HDR_LIGHT_PREPASS_OFF HDR_LIGHT_PREPASS_ON

            #include "UnityCG.cginc"

            sampler2D _LightBuffer;
            sampler2D _RampTex;
            sampler2D _DispTex;
            float4 _DispTex_ST;
            float _Displacement;
            float4 _ChannelFactor;
            float4 _Range;
            float _ClipRange;

            struct appdata_exp {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f_prepass_final {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                fixed3 shLight : COLOR0;
            };

            v2f_prepass_final vert_prepass_final (appdata_exp v) {
                v2f_prepass_final o;

                float3 norm = normalize(v.normal);

                float3 dcolor = tex2Dlod(_DispTex, float4(v.texcoord.xy, 0.0, 0.0)).rgb;
                float dispAmount = dot(dcolor, _ChannelFactor.xyz) * _Displacement;
                float4 displacedVertex = v.vertex;
                displacedVertex.xyz += norm * dispAmount;

                float4 clipPos = UnityObjectToClipPos(displacedVertex);
                o.pos = clipPos;
                o.uv = TRANSFORM_TEX(v.texcoord, _DispTex);
                o.screenPos = ComputeScreenPos(clipPos);

                float3 worldNormal = UnityObjectToWorldNormal(norm);
                o.shLight = ShadeSH9(float4(worldNormal, 1.0));

                return o;
            }

            fixed4 frag_prepass_final (v2f_prepass_final i) : SV_Target {
                float3 dcolor = tex2D(_DispTex, i.uv).rgb;
                float dispVal = dot(dcolor, _ChannelFactor.xyz) * (_Range.y - _Range.x) + _Range.x;

                if (_ClipRange - dispVal < 0.0) {
                    discard;
                }

                float2 rampUV = float2(dispVal, 0.5);
                float4 rampColor = tex2D(_RampTex, rampUV);
                float3 diffColor = rampColor.rgb;
                float3 emissionColor = rampColor.rgb * rampColor.a;

                half4 light = tex2Dproj(_LightBuffer, UNITY_PROJ_COORD(i.screenPos));

                #ifdef HDR_LIGHT_PREPASS_ON
                    light = max(light, half4(0.001, 0.001, 0.001, 0.001));
                #else
                    light = -log2(max(light, half4(0.001, 0.001, 0.001, 0.001)));
                #endif

                light.rgb += i.shLight;

                fixed4 c;
                c.rgb = diffColor * light.rgb + emissionColor;
                c.a = 0.0;
                return c;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}