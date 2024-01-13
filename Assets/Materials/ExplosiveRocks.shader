Shader "Unlit/ExplosiveRocks"
{
    Properties
    {
        _ColorCenter("ColorCenter", Color) = (1,1,1,1)
        _ColoExtremity("ColoExtremity", Color) = (1,1,1,1)
        _IntIntensity("Interior Intensity", Float) = 1
        _ExtIntensity("Exterior Intensity", Float) = 1
        _SinSpeed("SinSpeed", Float) = 1       
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

            struct appdata
            {
                float4 vertex : POSITION;
                float4 vertexpos : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float dist : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                //o.vertexPos = v.vertex;
                o.dist = length(v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.dist = length(o.vertex); //Cool DepthFade
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float _IntIntensity;
            float _ExtIntensity;
            float _SinSpeed;
            float4 _ColorCenter;
            float4 _ColoExtremity;              
                    
            fixed4 frag(v2f i) : SV_Target
            {
                float CoolSin = abs(sin(_Time.z * _SinSpeed)) + 0.5f;

                float mask = pow(i.dist, 100.5) * 0.25f;
                mask = saturate(mask);

                float4 col = lerp(_ColorCenter * CoolSin * _IntIntensity, _ColoExtremity * _ExtIntensity,mask);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
