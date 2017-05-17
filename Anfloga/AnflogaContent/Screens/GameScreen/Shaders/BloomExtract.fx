// Extracts the brightest areas of an image
// based on a threshold.
// This is step1 in applying a bloom effect.

sampler TextureSampler : register(s0);

float BloomThreshold;

float4 BloomExtractFunction(float2 texCoord: TEXCOORD0) : COLOR0
{
	// get pixel color
	float4 c = tex2D(TextureSampler, texCoord);

	// adjust it to keep only values brighter than threshold
	return saturate((c - BloomThreshold) / (1 - BloomThreshold));
}

technique BloomExtract
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 BloomExtractFunction();
	}
}