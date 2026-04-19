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


float4 WorldCuttingSlash(float2 coords : TEXCOORD0) : COLOR0
{
    float2 dirScreen = normalize(uDirection);
    float2 dir = normalize(float2(
        dirScreen.x / uScreenResolution.x,
        dirScreen.y / uScreenResolution.y
    ));

    float2 linePos = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float2 toPixel = coords - linePos;
    float2 perp = float2(-dir.y, dir.x);
    float dist = dot(toPixel, perp);

    float radius = 0.005;
    float influence = smoothstep(0.0, radius, abs(dist));
    
    float2 offset = dir * sign(dist) * 0.03 * uProgress * influence;
    float4 pixel = tex2D(uImage0, coords + offset);

    float width = 0.001;
    if (abs(dist) < width) {
        return float4(pixel.rgb * (1 - uProgress) + float3(1.0, 1.0, 1.0) * uProgress, pixel.a);
    }

    return pixel;
}


technique Technique1
{
    pass WorldCuttingSlash
    {
        PixelShader = compile ps_2_0 WorldCuttingSlash();
    }
}