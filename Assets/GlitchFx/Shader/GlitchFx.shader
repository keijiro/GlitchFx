//
// Glitch Fx
//
// Copyright (C) 2014 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

Shader "Hidden/GlitchFx"
{
    Properties
    {
        _MainTex   ("Base",      2D)    = ""{}
        _GlitchTex ("Glitch",    2D)    = ""{}
        _BufferTex ("Buffer",    2D)    = ""{}
        _Intensity ("Intensity", Float) = 1
    }
    
    CGINCLUDE

    #include "UnityCG.cginc"
    
    sampler2D _MainTex;
    sampler2D _GlitchTex;
    sampler2D _BufferTex;
    float _Intensity;

    float4 frag(v2f_img i) : SV_Target 
    {
        float4 glitch = tex2D(_GlitchTex, i.uv);

        float w1 = step(1 - _Intensity, pow(glitch.w, 3));
        float w2 = step(1 - _Intensity, pow(glitch.w, 6));

        float2 uv = i.uv + glitch.xy * w1;
        float4 source = tex2D(_MainTex, uv);
        float4 buffer = tex2D(_BufferTex, uv);

        //float3 rgb = lerp(source.rgb, buffer.rgb + lerp(float3(0, 0.0, 0), float3(0, 0.5, 0.5), glitch.z), w2);
        float3 rgb = lerp(source.rgb, buffer.rgb - source.gbb + source.rrg, w2);

        return float4(rgb, source.a);
    }

    ENDCG 
    
    Subshader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            Fog { Mode off }      
            CGPROGRAM
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
    }
}
