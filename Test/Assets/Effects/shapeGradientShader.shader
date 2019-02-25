﻿Shader "Unlit/shapeGradientShader"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
	    _FirstColor ("First Color", Color) = (1,1,1,1)
	    _SecondColor("Second Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

			fixed4 _FirstColor;
		    fixed4 _SecondColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : COLOR
            {
				fixed4 color = lerp(_FirstColor, _SecondColor, i.uv.y) * tex2D(_MainTex, i.uv);
				color.a = 1;
				return color;
			}
            ENDCG
        }
    }
}
