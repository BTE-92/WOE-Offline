Shader "PlaySomething/GroundFront1" {
Properties {
 _Color ("Color", Color) = (1,1,1,1)
 _Emission ("Emissive Color", Color) = (0,0,0,0)
 _Shininess ("Shininess", Range(0.01,1)) = 1
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "LIGHTMODE"="Vertex" "QUEUE"="Geometry" "RenderType"="Opaque" }
  Lighting On
  Material {
   Ambient [_Color]
   Diffuse [_Color]
   Emission [_Emission]
   Shininess [_Shininess]
  }
  SetTexture [_MainTex] { ConstantColor [_Color] combine texture * primary double, texture alpha * constant alpha }
 }
}
}