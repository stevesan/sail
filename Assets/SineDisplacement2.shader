Shader "Steve/Sine Displacement 2"
{
    Properties
    {
        MainColor ("Main Color", COLOR)  = ( 0.5, 0.5, 1, 1 )

        Amplitude ("Amplitude", float) = 1
        Frequency ("Frequency", float) = 0.2
        Direction ("Direction", Vector) = (1,0,0,0)
        Origin ("Origin", Vector) = (0,0,0,1)
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
        float Amplitude;
        float Frequency;
        float4 Origin;
        float4 Direction;


        float3x3 axisAngleRotationMatrix( float3 axis, float angleRads )
        {
            float a = angleRads;
            float x = axis.x;
            float y = axis.y;
            float z = axis.z;
            float cosa = cos(a);
            float sina = sin(a);
            return float3x3(
                    (1-cosa)*x*x + cosa,    (1-cosa)*x*y - z*sina,      (1-cosa)*z*x + y*sina,
                    (1-cosa)*x*y + z*sina,  (1-cosa)*y*y + cosa,        (1-cosa)*y*z - x*sina,
                    (1-cosa)*z*x - y*sina,  (1-cosa)*y*z + x*sina,      (1-cosa)*z*z + cosa );
        }

        float unlerp( float a, float b, float x )
        {
            return (x-a) / (b-a);
        }

        void vert(inout appdata_full v)
        {
            v.color = MainColor;

            //----------------------------------------
            // sine-deform
            //----------------------------------------
            const float pi = 3.14159;
            float t = dot(normalize(Direction), v.vertex-Origin);
            float theta = 2*pi * Frequency * t;
            float sinval = sin( theta );
            v.vertex.y += Amplitude * sinval;

            //----------------------------------------
            // rotate normal. this isn't perfect, since it will cause a gradient
            // to show up within a triangle. ideally, the GPU would recalculate our normals
            // post-deformation, per-triangle. But, this gives some reasonable effect.
            //----------------------------------------

            float grad = Amplitude * 2*pi * Frequency * cos( theta );
            float3 xAxis = cross( float3(0,1,0), normalize(Direction) );
            float rads = atan(grad);
            v.normal = mul(
                    axisAngleRotationMatrix( xAxis, rads ),
                    v.normal );
        }

        struct Input
        {
            float4 color : COLOR;
        };

        void surf( Input IN, inout SurfaceOutput o )
        {
            o.Albedo = IN.color;
            o.Alpha = IN.color.a;
        }

        ENDCG
    }

}
