Shader "ScreenshotSeparateAlpha" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Alpha ("Alpha (A)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent-200" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent-200" "RenderType"="Transparent" }
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGB
  SetTexture [_MainTex] { combine texture }
  SetTexture [_Alpha] { combine previous, texture alpha }
 }
}
}