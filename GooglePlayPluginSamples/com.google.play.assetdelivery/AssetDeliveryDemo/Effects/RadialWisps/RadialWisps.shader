// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

Shader "PlaySamples/RadialWisps"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SecondaryTex ("Texture", 2D) = "white" {}
        _ScrollSpeed ("_ScrollSpeed", float) = 1
        _Color ("Color", Color) = (1,1,1,0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend One One
        ZWrite Off
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
            sampler2D _SecondaryTex;

            float4 _Color;
            float _ScrollSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float2 radialWarp(float2 coord)
            {
                // scale and shift to center
                float2 shiftedUv = coord * 2 - 1;

                // convert to polar coordinates
                float2 warpedUv = float2(atan2(shiftedUv.y, shiftedUv.x), length(shiftedUv));

                // convert from -pi to pi range to 0 to 1 range
                warpedUv.x += 3.141592653589793238462;
                warpedUv.x /= 6.283185307179586476924;
                return warpedUv;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv1 = radialWarp(i.uv);
                float2 uv2 = uv1;

                // scroll textures at different speeds
                uv1.y -= _ScrollSpeed * _Time.x;
                uv2.y -= _ScrollSpeed * _Time.x * 0.5;

                // scale and spin
                uv2.x *= 2;
                uv2.x += _ScrollSpeed;

                // blend textures
                fixed4 col = tex2D(_MainTex, uv1);
                col *= tex2D(_SecondaryTex, uv2);

                // radial mask
                col *= smoothstep(0.0, 0.1, saturate(0.5 - distance(i.uv, float2(0.5,0.5))));
                col *= _Color;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
