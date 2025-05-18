Shader "Unlit/UIShader1"
{
    Properties
    {
        [PerRendererData]_MainTex("Tex",2D) = "white"{}
        _Ref("Stencil Ref",int) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)]_StencilComp("Stencil Comp",int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilOp("Stencil Op",int) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        
        ColorMask RGBA
        Stencil
        {
            //            Ref [_Stencil]
            //            ReadMask [_StencilReadMask]
            //            WriteMask [_StencilWriteMask]
            //            Comp [_StencilComp] ((UnityEngine.Rendering.CompareFunction))
            //            Pass [_StencilOp] (UnityEngine.Rendering.StencilOp)
            //            Fail [_Fail]
            //            ZFail [_ZFail]
            Ref [_Ref]
            ReadMask 3
            Comp [_StencilComp]
            Pass [_StencilOp]
            WriteMask 5
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c;
                fixed4 mainTex = tex2D(_MainTex, i.uv);
                c = mainTex;
                c *= i.color;
                return c;
            }
            ENDCG
        }
    }
}