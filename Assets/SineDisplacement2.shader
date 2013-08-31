Shader "Steve/Sine Displacement 2"
{
    Properties
    {
        MainColor ("Main Color", COLOR)  = ( 0.5, 0.5, 1, 1 )
        CapColor ("Cap Color", COLOR)  = ( 0.5, 0.5, 1, 1 )
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
            //"Queue" = "Transparent"
            //"RenderType" = "Transparent"
            //"IgnoreProjector" = "True"
        }

        LOD 200

        //Blend SrcAlpha OneMinusSrcAlpha

        //ZWrite Off  // very important! otherwise water does not have that shimmering look

        CGPROGRAM

#pragma surface surf Lambert vertex:vert

        float4 MainColor;
        float4 CapColor;
        float Amplitude;
        float Frequency;
        float4 Origin;
        float4 Direction;
        float FadeDistance;

        float EmissionPower;
        float4 EmissiveColor;

        void vert(inout appdata_full v)
        {
            float t = dot(normalize(Direction), v.vertex-Origin);
            float theta = 2*3.14129 * Frequency * t;
            float sinval = sin( theta );
            v.vertex.y += Amplitude * sinval;
            v.color = MainColor + saturate(sinval) * CapColor;

            // TODO rotate normal wrt current t value
            float grad = 2*3.14129 * Frequency * cos( theta );
            float3 xAxis = cross( float3(0,1,0), normalize(Direction) );
            float rads = atan(grad);
            /*
            float x = v.normal.x;
            float y = v.normal.y
            v.normal.y = saturate(sinval);
            v.normal = normalize(v.normal);
            */
        }

        sampler2D MainTex;

        struct Input
        {
            float2 uv_Maintex;
            float3 viewDir;
            float4 vertColor;
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
