Shader "Unlit/Effects"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"


            struct v2f
            {
                float2 uv : TEXCOORD0;
            };


            v2f vert(float4 vertex: POSITION,
                     out float4 pos:SV_POSITION)
            {
                v2f o;
                pos = UnityObjectToClipPos(vertex);
                return o;
            }

            fixed4 frag(v2f i,UNITY_VPOS_TYPE screenPos: VPOS) : SV_Target
            {
                return 1;
            }
            ENDCG
        }
    }
}