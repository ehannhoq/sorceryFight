sampler uImage0 : register(s0);
float uTime;
float2 uSize;
float2 uOffset;
float2 uPieceSize;
float4 ResonantTooltip(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 mask = tex2D(uImage0, coords);
    float2 uv = (uOffset + coords * uPieceSize) / uSize;
    float aspect = uSize.x / uSize.y;

    float3 colorA = float3(0.32, 0.14, 0.04);
    float3 colorB = float3(0.09, 0.04, 0.01);

    float2 b1 = float2(0.30 + 0.12 * sin(uTime * 0.7),       0.30 + 0.21 * cos(uTime * 0.9));
    float2 b2 = float2(0.30 + 0.31 * sin(uTime * 0.5 + 2.0), 0.60 + 0.52 * cos(uTime * 1.1 + 1.0));
    float2 b3 = float2(0.50 + 0.25 * sin(uTime * 0.3 + 4.0), 0.70 + 0.15 * cos(uTime * 0.6 + 3.0));
    float2 b4 = float2(0.40 + 0.43 * sin(uTime * 0.3 + 1.5), 0.20 + 0.22 * cos(uTime * 0.6 + 5.0));
    float2 b5 = float2(0.20 + 0.32 * sin(uTime * 0.3 + 3.0), 0.50 + 0.60 * cos(uTime * 0.3 + 7.0));
    float2 b6 = float2(0.40 + 0.40 * sin(uTime * 0.9 + 3.0), 0.31 + 0.40 * cos(uTime * 0.3 + 3.0));
    float2 b7 = float2(0.25 + 1.02 * sin(uTime * 0.2 + 3.0), 0.82 + 0.74 * cos(uTime * 0.7 + 6.0));

    float field = 0.0;
    float2 d1 = uv - b1;
    d1.x *= aspect;
    field += 0.02 / dot(d1, d1);
    float2 d2 = uv - b2;
    d2.x *= aspect;
    field += 0.02 / dot(d2, d2);
    float2 d3 = uv - b3;
    d3.x *= aspect;
    field += 0.02 / dot(d3, d3);
    float2 d4 = uv - b4;
    d4.x *= aspect;
    field += 0.02 / dot(d4, d4);
    float2 d5 = uv - b5;
    d5.x *= aspect;
    field += 0.02 / dot(d5, d5);
    float2 d6 = uv - b6;
    d6.x *= aspect;
    field += 0.02 / dot(d6, d6);
    float2 d7 = uv - b7;
    d7.x *= aspect;
    field += 0.02 / dot(d7, d7);

    float lineWidth = 0.015;
    float contourSpacing = 0.25;
    float contourPhase = frac(field / contourSpacing) * contourSpacing;
    float distToLine = min(contourPhase, contourSpacing - contourPhase);
    float edge = 1.0 - smoothstep(0.0, lineWidth, distToLine);

    float3 col = lerp(colorB, colorA, edge);

    return float4(col * mask.a, mask.a) * sampleColor;
}
technique Technique1
{
    pass ResonantTooltip
    {
        PixelShader = compile ps_2_0 ResonantTooltip();
    }
}
