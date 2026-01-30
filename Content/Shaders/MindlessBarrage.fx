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

float4 MindlessBarrage(float2 coords : TEXCOORD0) : COLOR0
{
    float2 texel = 1.0 / uScreenResolution;
    float border = 2.0;

    float2 borderWidth = border * texel;

    bool isEdge =
        coords.x < borderWidth.x ||
        coords.y < borderWidth.y ||
        coords.x > 1.0 - borderWidth.x ||
        coords.y > 1.0 - borderWidth.y;

    if (isEdge)
        return float4(0.0, 0.0, 0.0, 1.0);

    float4 ul = tex2D(uImage0, coords + float2(-texel.x, texel.y));
    float4 u = tex2D(uImage0, coords + float2(0.0, texel.y));
    float4 ur = tex2D(uImage0, coords + float2(texel.x, texel.y));

    float4 cl = tex2D(uImage0, coords + float2(-texel.x, 0.0));
    float4 c = tex2D(uImage0, coords);
    float4 cr = tex2D(uImage0, coords + float2(texel.x, 0.0));

    float4 bl = tex2D(uImage0, coords + float2(-texel.x, -texel.y));
    float4 b = tex2D(uImage0, coords + float2(0.0, -texel.y));
    float4 br = tex2D(uImage0, coords + float2(texel.x, -texel.y));

    float4 blur = 
        (ul + u + ur + cl + c + cr + bl + b + br);

    blur /= 9;

    float2 center = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float2 d = coords - center;
    float dist = length(d);
    float vignette = smoothstep(0.0, 0.5, dist);

    float gray = dot(blur.rgb, float3(0.2126, 0.7152, 0.0722));

    float redStrength = blur.r - max(blur.g, blur.b);
    float redMask = saturate(redStrength * 5.0);

    float3 colorNoGrayOnRed = lerp(
        float3(gray, gray, gray),
        blur.rgb,
        redMask
    );

    colorNoGrayOnRed *= (1 - vignette);

    return float4(c.rgb * (1 - uOpacity) + colorNoGrayOnRed * uOpacity, blur.a);
}

technique Technique1
{
    pass MindlessBarrage
    {
        PixelShader = compile ps_2_0 MindlessBarrage();
    }
}