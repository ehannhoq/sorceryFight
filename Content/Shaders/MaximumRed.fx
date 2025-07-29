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

float4 Desaturate(float2 coords : TEXCOORD0) : COLOR0
{
    float4 pixelColor = tex2D(uImage0, coords);
    
    if (pixelColor.r > pixelColor.g * 2 && pixelColor.r > pixelColor.b * 2)
    {
        return pixelColor;
    }
    
    float radius = uProgress;
    float2 pos = (uTargetPosition - uScreenPosition) / uScreenResolution;

    float2 difference = coords - pos;
    difference.x *= uScreenResolution.x / uScreenResolution.y;
    
    float dist = length(difference);

    if (dist <= radius)
    {
        float gray = dot(pixelColor.rgb, float3(0.3, 0.59, 0.11));
        return float4(gray, gray, gray, pixelColor.a);
    }
    else if (dist <= radius + 0.001f)
    {
        return float4(uColor.rgb, pixelColor.a);
    }

    return pixelColor;
}


technique Technique1
{
    pass Desaturate
    {
        PixelShader = compile ps_2_0 Desaturate();
    }
}

