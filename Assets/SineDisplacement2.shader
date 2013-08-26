Shader "Steve/Sine Displacement 2"
{
    Properties
    {
        MainColor ("Main Color", COLOR)  = ( 0.5, 0.5, 1, 1 )
        MainTex ("Main Texture", 2D) = "" { }

        Amplitude ("Amplitude", float) = 1
        Frequency ("Frequency", float) = 0.2
        Direction ("Direction", Vector) = (1,0,0,0)
        Origin ("Origin", Vector) = (0,0,0,1)
        FadeDistance ("Fade Distance", float) = 1
    }

CGINCLUDE
// -----------------------------------------------------------
// This section is included in all program sections below

#include "UnityCG.cginc"

uniform float4 MainColor;

uniform float Amplitude;
uniform float Frequency;
uniform float4 Origin;
uniform float4 Direction;
uniform float FadeDistance;

struct appdata {
	float4 vertex : POSITION;
	float3 normal : NORMAL;
    float2 uv : TEXCOORD0;
};

struct v2f {
	float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    half4 color : COLOR;
};

v2f vert(appdata v)
{
	v2f o;
	float4 s;

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
}

ENDCG
	
// -----------------------------------------------------------
// Fragment program

Subshader {
	//Tags { "RenderType"="Opaque" }

	Tags { "Queue" = "Transparent" }
    Pass {
	    Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
#pragma exclude_renderers gles
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest 

        sampler2D MainTex;

        half4 frag( v2f i ) : COLOR
        {
            // TEMP
            return i.color;

            /*
            half4 water = tex2D( MainTex, i.uv );
            return water;

            half4 col;
            col.rgb = lerp( water.rgb, _horizonColor.rgb, water.a );
            col.a = _horizonColor.a;
            return col;
            */
        }
        ENDCG
    }
}

}
