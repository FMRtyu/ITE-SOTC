Shader "Custom/ChromaKey"
{
    Properties
    {
        _MainTex ("Video Texture", 2D) = "white" {}
        _ChromaColor ("Chroma Key Color", Color) = (0,1,0,1)
        _Tolerance ("Color Tolerance", Range(0,1)) = 0.3
        _Smoothness ("Edge Smoothness", Range(0,1)) = 0.1
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ChromaColor;
            float _Tolerance;
            float _Smoothness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Distance between pixel color and chroma color
                float diff = distance(col.rgb, _ChromaColor.rgb);

                // Smooth fade for edges
                float alpha = smoothstep(_Tolerance, _Tolerance + _Smoothness, diff);

                col.a = alpha;
                return col;
            }
            ENDCG
        }
    }
}
