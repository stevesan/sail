Shader "Steve/Sine Displacement 2"
{
    Properties
    {
        MainColor ("Main Color", COLOR)  = ( 0.5, 0.5, 1, 1 )
        MainTex ("Main Texture", 2D) = "" { }

        EmissionPower ("Emission Power", float) = 1
        EmissiveColor ("Emissive Color", COLOR) = (0.5, 0.5, 0.5, 0.5)
        Amplitude ("Amplitude", float) = 1
        Frequency ("Frequency", float) = 0.2
        Direction ("Direction", Vector) = (1,0,0,0)
        Origin ("Origin", Vector) = (0,0,0,1)
        FadeDistance ("Fade Distance", float) = 1
    }
    Subshader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }

        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha

        ZWrite Off  // very important! otherwise water does not have that shimmering look

        CGPROGRAM

#pragma surface surf Lambert vertex:vert

        float4 MainColor;
        float EmissionPower;
        float4 EmissiveColor;
        float Amplitude;
        float Frequency;
        float4 Origin;
        float4 Direction;
        float FadeDistance;

        void vert(inout appdata_full v)
        {
            float t = dot(normalize(Direction), v.vertex-Origin);
            float sinval = sin( 2*3.14129 * Frequency * t );
            v.vertex.y += Amplitude * sinval;

            /*
               This code provided a white-tint on the apex of the wave; revisit later

               v2f o;

            // fade out the further we get
            float fadeScale = smoothstep( 1, 0, saturate(length(v.vertex)/FadeDistance) );

            // modify position in object space
            float t = dot(normalize(Direction), v.vertex-Origin);
            float sinval = sin( 2*3.14129 * Frequency * t );
            v.vertex.y += fadeScale * Amplitude * sinval;

            o.pos = mul (UNITY_MATRIX_MVP, v.vertex);

            // object space view direction

            o.uv = v.uv;

            o.color = MainColor;
            //o.color.a = fadeScale*(sinval/2+0.5);
            //o.color *= saturate(dot( v.normal, float3(0,1,0) ));

            return o;
             */
        }

        sampler2D MainTex;

        struct Input
        {
            float2 uv_Maintex;
            float3 viewDir;
        };

        void surf( Input IN, inout SurfaceOutput o )
        {
            o.Albedo = MainColor.rgb;
            o.Alpha = MainColor.a;

            o.Emission = EmissiveColor * pow(
                    saturate(dot( normalize(IN.viewDir), o.Normal )),
                    EmissionPower);
        }

        ENDCG
    }

}
