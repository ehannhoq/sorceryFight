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

float4 LimitlessRCTFilter(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float4 originalColor = color;

    color.r = min(color.r * 1.2, 1.0);
    color.g = min(color.g * 1.0, 1.0);
    color.b = color.b * 0.5;

    color.rgb = min(color.rgb * 1.1, 1.0);
    color *= uOpacity;

    float3 result = originalColor.rgb * (1 - uProgress) + color * uProgress;
    return float4(result.rgb, color.a);
}

technique Technique1
{
    pass LimitlessRCTFilter
    {
        PixelShader = compile ps_2_0 LimitlessRCTFilter();
    }
}