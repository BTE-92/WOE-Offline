Shader "Character/Ghost Shader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
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
        // PASS 1: Z-Prime (Depth-Only, prevents self-transparency sort issues)
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
        // PASS 2: FORWARD BASE
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
            #pragma multi_compile_fwdbase
            #pragma fragmentoption ARB_precision_hint_fastest

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Brightness;
            float _RimPower;
            float4 _RimColor;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 tex0 : TEXCOORD0;
                float3 viewVec : TEXCOORD1;      // Camera - WorldVertex
                float3 worldNormal : TEXCOORD2;
            };

            v2f vert (appdata v) {
                v2f o;
                
                o.pos = UnityObjectToClipPos(v.vertex);
                o.tex0 = TRANSFORM_TEX(v.texcoord, _MainTex);
                
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewVec = _WorldSpaceCameraPos - worldPos;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                
                return o;
            }

            float4 frag (v2f i) : SV_Target {
                // 1. Sample base texture and convert to grayscale luminance
                float4 basecol = tex2D(_MainTex, i.tex0);
                float lum = dot(basecol.xyz, float3(0.3, 0.59, 0.11));
                float3 diffColor = float3(lum, lum, lum);
                
                float3 worldNormal = normalize(i.worldNormal);

                // 2. Rim lighting calculation (inverse Fresnel using world normal vs view vector)
                float rim = 1.0 - saturate(dot(normalize(i.viewVec), worldNormal));
                float3 rimColor = (_RimColor.xyz * pow(rim, _RimPower)) * _Brightness;
                
                // 3. Alpha derived from average of rim RGB
                float alpha = (rimColor.x + rimColor.y + rimColor.z) / 3.0;
                
                // 4. Direct diffuse light (Lambertian)
                float NdotL = max(0.0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                
                float4 c;
                c.xyz = (diffColor * _LightColor0.rgb) * NdotL;
                
                // Add rim to RGB
                c.xyz = c.xyz + rimColor;
                c.w = alpha;
                
                return c;
            }
            ENDCG
        }

        // ============================================================
        // PASS 3: FORWARD ADD (Point / Spot / Directional / Cookies)
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
            #pragma fragmentoption ARB_precision_hint_fastest

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Brightness;
            float _RimPower;
            float4 _RimColor;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 tex0 : TEXCOORD0;
                float3 viewVec : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 lightDir : TEXCOORD3;     // Vector to light (unnormalized for point/spot)
                LIGHTING_COORDS(4, 5)
            };

            v2f vert (appdata v) {
                v2f o;
                
                float3 worldVertex = mul(unity_ObjectToWorld, v.vertex).xyz;
                
                // Light vector: point/spot = pos - worldVert; directional = pos.xyz
                float3 lightDir = _WorldSpaceLightPos0.xyz - worldVertex * _WorldSpaceLightPos0.w;
                
                o.pos = UnityObjectToClipPos(v.vertex);
                o.tex0 = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.viewVec = _WorldSpaceCameraPos - worldVertex;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.lightDir = lightDir;
                
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            float4 frag (v2f i) : SV_Target {
                // 1. Grayscale base
                float4 basecol = tex2D(_MainTex, i.tex0);
                float lum = dot(basecol.xyz, float3(0.3, 0.59, 0.11));
                float3 diffColor = float3(lum, lum, lum);
                
                float3 worldNormal = normalize(i.worldNormal);

                // 2. Rim
                float rim = 1.0 - saturate(dot(normalize(i.viewVec), worldNormal));
                float3 rimColor = (_RimColor.xyz * pow(rim, _RimPower)) * _Brightness;
                float alpha = (rimColor.x + rimColor.y + rimColor.z) / 3.0;
                
                // 3. Additive lighting
                float3 lightDir = normalize(i.lightDir);
                float atten = LIGHT_ATTENUATION(i);
                float NdotL = max(0.0, dot(worldNormal, lightDir));
                
                float4 c;
                c.xyz = (diffColor * _LightColor0.rgb) * (NdotL * atten);
                c.w = alpha;
                
                return c;
            }
            ENDCG
        }
    }
    
    Fallback "Diffuse"
}