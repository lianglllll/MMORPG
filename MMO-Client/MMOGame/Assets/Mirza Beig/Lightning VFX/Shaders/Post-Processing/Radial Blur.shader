Shader "Mirza Beig/Lightning VFX/Post-Processing/RadialBlur"
{
    Properties 
    {
        _MainTex ("Texture", 2D) = "white" {}

        _BlurQuality ("Blur Quality", Range(1.0, 128.0)) = 32.0
        _BlurAmount ("Blur Amount", Range(0.0, 1)) = 0.4

        _BlurPower ("Blur Power", Range(0.0, 32.0)) = 2.0

        _BlurCenterX ("Blur Center X", Range(0.0, 1.0)) = 0.5
        _BlurCenterY ("Blur Center Y", Range(0.0, 1.0)) = 0.5
    }
 
    SubShader
    {
        Tags { "RenderType"="Opaque" }
 
        Pass 
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "CGIncludes\RadialBlur.cginc"
 
            struct appdata 
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };
 
            struct v2f 
            {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
 
            Texture2D _MainTex;
            SamplerState sampler_MainTex;

            float4 _MainTex_TexelSize;

            float _BlurQuality;
            float _BlurAmount;

            float _BlurPower;

            float _BlurCenterX;
            float _BlurCenterY;
 
            v2f vert (appdata v) 
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target 
            {
                float2 blurAmount = float2(1.0, 1.0) * _BlurAmount;
                blurAmount = pow(blurAmount, _BlurPower);

                float2 uv = i.texcoord;

                float2 blurCenter = float2(_BlurCenterX, _BlurCenterY);
    
                float4 colour;
                RadialBlur_float(_MainTex, sampler_MainTex, i.texcoord, _MainTex_TexelSize, _BlurQuality, blurAmount, blurCenter, colour);

                return colour;
            }

            ENDCG
        }
    }
}
