Shader "LoadingScreenFade" {
Properties {
 _Color ("Color", Color) = (1,1,1,1)
 _MainTex ("Texture", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent+200" "IGNOREPROJECTOR"="True" }
 Pass {
  Tags { "QUEUE"="Transparent+200" "IGNOREPROJECTOR"="True" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  Blend SrcAlpha OneMinusSrcAlpha
  SetTexture [_MainTex] { combine texture * primary, texture alpha * primary alpha }
 }
}
}