sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect; 
float2 uZoom;

float4 GaussianBlur(float2 coords : TEXCOORD0) : COLOR0
{
    float2 texel = 1.0 / uScreenResolution;

    bool isEdge =
        uv.x < texel.x ||
        uv.y < texel.y ||
        uv.x > 1.0 - texel.x ||
        uv.y > 1.0 - texel.y;

    if (isEdge) return;

    float4 ul = tex2D(sampler, coords + float2(-texel.x, texel.y));
    float4 u = tex2D(sampler, coords + float2(0, -texel.));
    float4 ur = tex2D(sampler, coords + float2(texel.x, texel.y));

    float4 cl = tex2D(sampler, coords + float2(-texel.x, 0));
    float4 c = tex2D(sampler, coords);
    float4 cr = tex2D(sampler, coords + float2(texel.x, 0));

    float4 bl = tex2D(sampler, coords + float2(-texel.x, -texel.y));
    float4 b = tex2D(sampler, coords + float2(0, -texel.y));
    float4 br = tex2D(sampler, coords + float2(texel.x, -texel.y));

    float4 result = 
        (ul + ur + bl + br) * 1.0 +
        (u + cl + cr + b) * 2.0 + 
        (c) * 4.0;

    result /= 16;

    return result;
}

technique Technique1
{
    pass GaussianBlur
    {
        PixelShader = compile ps_2_0 GaussianBlur();
    }
}