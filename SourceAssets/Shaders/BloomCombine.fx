// The final step in the Bloom effect.
// Combines the base texture with the
// extracted and blurred bloom image.

sampler BloomSampler : register(s0);
sampler BaseSampler : register(s1);

float BloomIntensity;
float BaseIntensity;
float BloomSaturation;
float BaseSaturation;

// helper for modifying color saturation
float4 AdjustSaturation(float4 color, float saturation)
{
	// use constants 0.3, 0.59 and 0.11 because the human
	// eye is more sensitive to green and less to blue
	float grey = dot(color, float3(0.3, 0.59, 0.11));
	return lerp(grey, color, saturation);
}

float4 BloomCombineFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
	// look up bloom and original colors
	float4 bloomColor = tex2D(BloomSampler, texCoord);
	float4 baseColor = tex2D(BaseSampler, texCoord);

	// Adjust saturation and intensity
	bloomColor = AdjustSaturation(bloomColor, BloomSaturation) * BloomIntensity;
	baseColor = AdjustSaturation(baseColor, BaseSaturation) * BaseIntensity;

	// Darken down a bit to prevent excessive burnout look
	baseColor *= (1 - saturate(bloomColor));

	// combine
	return baseColor + bloomColor;
}

technique BloomCombine
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 BloomCombineFunction();
	}
}