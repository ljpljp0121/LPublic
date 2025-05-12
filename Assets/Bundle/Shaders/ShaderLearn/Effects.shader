Shader "Unlit/Effects"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Toggle] _A("A",int) = 0
    }
    SubShader
    {
        Blend One One
        Tags
        {
            "Queue" = "Transparent"
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _A_ON _B 

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                #if _A_ON
                o.uv.y = 1;
                #elif B
                o.uv.x = 1;
                #endif
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}