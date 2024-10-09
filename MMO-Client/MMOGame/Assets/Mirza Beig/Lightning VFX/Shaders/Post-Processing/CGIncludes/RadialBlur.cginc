void RadialBlur_float(Texture2D tex, SamplerState samplerState, float2 uv, float2 texelSize, int blurQuality, float2 blurAmountXY, float2 blurCenter, out float4 output)
{
    // Pre-calculations.

    float4 colour = 0.0;
    float kernelWeightSum = 0;
    
    float blurQualityOneMinus = float(blurQuality - 1.0);
    
    // Iterate through the surrounding pixels based on the blur quality.

    for (int i = 0; i < blurQuality; i++)
    {
        // Calculate the offset in the radial direction based on the iteration and blur amount.

        float offset = i / blurQualityOneMinus;
        float2 offsetUV = blurCenter + (uv - blurCenter) * (1.0 - offset * blurAmountXY);

        colour += tex.Sample(samplerState, float2(offsetUV));
        kernelWeightSum++;
    }
    
    // Normalize accumulated blur color by iterations.
    // Return the final blurred color.

    output = colour / kernelWeightSum;
}