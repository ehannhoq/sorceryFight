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

float4 ImpactFrame(float2 coords : TEXCOORD0) : COLOR0
{
    float4 pixelColor = tex2D(uImage0, coords);

    float4 finalColor = uProgress == 1 ? float4(uColor, pixelColor.a) : float4(1, 1, 1, pixelColor.a);

    float3 luminosityVector = float3(0.213, 0.715, 0.072);
    float luminosity = dot(pixelColor.rgb, luminosityVector);

    return luminosity > 0.5 ? finalColor : float4(0, 0, 0, pixelColor.a); 
}

technique Technique1
{
    pass ImpactFrame
    {
        PixelShader = compile ps_2_0 ImpactFrame();
    }
}