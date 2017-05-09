// This filter takes a collection of weights and offsets
// and uses them to blur along a single dimension
// This is usually called twice, once for each axis.

// This sampler is set automatically based on the render texture source
sampler TextureSampler : register(s0);

// The blur radius
#define RADIUS 7

// the total kernal size
#define KERNAL_SIZE (RADIUS * 2 + 1)

// offsets, the x or y values should generally be zero, depending on which axis is being blurred
float2 Offsets[KERNAL_SIZE];

// usually a one set of gaussian weight calculations
float Weights[KERNAL_SIZE];

// blur filter function, applies a collection of samples to the pixel based on the sample weight
float4 OneDimensionBlurFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
	float4 c = 0;

	// combine weighted filter taps
	for (int i = 0; i < KERNAL_SIZE; i++)
	{
		c += tex2D(TextureSampler, texCoord + Offsets[i]) * Weights[i];
	}

	return c;
}

technique OneDimensionalBlur
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 OneDimensionBlurFunction();
	}
}