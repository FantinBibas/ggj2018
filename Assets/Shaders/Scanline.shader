// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Scanlines" {
Properties {
    _MainTex("Texture", 2D) = "white" {}
    _Color("Color", Color) = (0,0,0,1)
    _LinesSize("LinesSize", Range(1,10)) = 1
}
SubShader {
    Tags {"IgnoreProjector" = "True" "Queue" = "Overlay"}
    Fog { Mode Off }
    Pass {
        ZTest Always
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
 
CGPROGRAM
 
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
 
        fixed4 _Color;
        half _LinesSize;
        sampler2D _MainTex;
 
        struct v2f {
            half4 pos:POSITION;
            fixed4 sPos:TEXCOORD;
        };
 
        v2f vert(appdata_base v) {
            v2f o; 
            o.pos = UnityObjectToClipPos(v.vertex);
            o.sPos = ComputeScreenPos(o.pos);
            return o;
        }
 
        fixed4 frag(v2f i) : COLOR {
            fixed4 pos = i.sPos;
            pos.y += (cos((pos.x + 0.5) * 3.14) + 1) / 50.0 * sin((pos.y - 0.5) * 3.14);
            if (pos.y < 0 || pos.y > 1) return _Color;
            fixed fact = (pos.y + _Time.x) * 30 % 1;
            fact = sin(fact * 3.14) / 6;
            fixed4 color = _Color;
            fixed4 texColor = tex2D(_MainTex, pos);
            pos.xy -= 0.5;
            fact += dot(pos.xy, pos.xy) * 2;
            return  texColor * (1 - fact) + color * fact;
         }
 
ENDCG
       }
 
}
}