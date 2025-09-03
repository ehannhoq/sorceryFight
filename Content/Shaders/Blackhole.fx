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

#define PI 3.14159265358979323846
#define EPSILON 0.0001

float4 Blackhole(float2 coords : TEXCOORD0) : COLOR0
{
    float aspectRatio = uScreenResolution.x / uScreenResolution.y;
    float2 center = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float radius = uProgress;

    float2 d = coords - center;
    float2 dAR = float2(d.x * aspectRatio, d.y);

    float dist = length(dAR);
    if (dist <= radius) return float4(0, 0, 0, 1);

    float t = (dist - radius) / dist;

    float angle = 0.5 * (radius / dist) * PI;
    float s = sin(angle);
    float c = cos(angle);
    float rot = float2x2(c, -s, s, c);

    float2 v = mul(dAR, rot);
    v.x /= aspectRatio;
    
    float2 uv = center + lerp(v, d, t);
    uv = saturate(uv);

    return tex2D(uImage0, uv);
    
}

technique Technique1
{
    pass Blackhole
    {
        PixelShader = compile ps_2_0 Blackhole();
    }
}