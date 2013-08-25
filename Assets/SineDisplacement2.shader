Shader "Steve/Sine Displacement 2" {
Properties {
	_horizonColor ("Horizon color", COLOR)  = ( .172 , .463 , .435 , 0)
	MainTex ("Main Texture", 2D) = "" { }

	Amplitude ("Amplitude", float) = 1
    Frequency ("Frequency", float) = 0.2
	Direction ("Direction", Vector) = (1,0,0,0)
	Origin ("Origin", Vector) = (0,0,0,1)
}

CGINCLUDE
// -----------------------------------------------------------
// This section is included in all program sections below

#include "UnityCG.cginc"

uniform float4 _horizonColor;

uniform float Amplitude;
uniform float Frequency;
uniform float4 Origin;
uniform float4 Direction;

struct appdata {
	float4 vertex : POSITION;
	float3 normal : NORMAL;
    float2 uv : TEXCOORD0;
};

struct v2f {
	float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
};

v2f vert(appdata v)
{
	v2f o;
	float4 s;

    // modify position in object space
    float t = dot(normalize(Direction), v.vertex-Origin);
    v.vertex.y += Amplitude * sin( 2*3.14129*Frequency*t );

    /*
    if( distance(v.vertex, Origin) < 1.5)
        v.vertex.y += 1;
    else
        v.vertex.y -= 1;
        */

	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);

	// object space view direction

    o.uv = v.uv;

	return o;
}

ENDCG
	
// -----------------------------------------------------------
// Fragment program

Subshader {
	Tags { "RenderType"="Opaque" }
    Pass {

        CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
#pragma exclude_renderers gles
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest 

        sampler2D MainTex;

        half4 frag( v2f i ) : COLOR
        {
            half4 water = tex2D( MainTex, i.uv );
            return water;

            half4 col;
            col.rgb = lerp( water.rgb, _horizonColor.rgb, water.a );
            col.a = _horizonColor.a;
            return col;
        }
        ENDCG
    }
}

}
