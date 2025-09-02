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
    float2 center = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float radius = uProgress;

    float2 difference = coords - center;
    difference.x *= uScreenResolution.x / uScreenResolution.y;

    float distance = length(difference)

    if (distance < radius)
    {
        return float4(0, 0, 0, 1);
    }

    float distFromEdge = distance - radius + EPSILON;

    float angle = (90 * (0.01 / distFromEdge)) * (PI / 180);
    float s = sin(angle);
    float c = cos(angle);
    float2x2 rotationMatrix = float2x2(
        c, -s,
        s, c,
    );

    float2 rotatedCoords = mul(difference, rotationMatrix);
    float4 targetColor = tex2D(uImage0, rotatedCoords);

    return targetColor;

}

technique Technique1
{
    pass Blackhole
    {
        PixelShader = compile ps_2_0 Blackhole();
    }
}