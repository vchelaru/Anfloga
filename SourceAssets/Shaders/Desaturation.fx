// Desaturates an image by the provided strenght (0-1)

sampler TextureSampler : register(s0);

float DesaturationStrength;

// This comes from the sprite batch vertex shader
struct VertexShaderOutput
{
	float2 Position: TEXCOORD0;
	float4 Color: COLOR0;
	float2 TextureCoordinate: TEXCOORD0;
};

// straight passthrough to turn effects off easily
float4 PassThrough(VertexShaderOutput input) : COLOR0
{
	return tex2D(TextureSampler, input.TextureCoordinate);
}

// simple desat filter for testing
float4 DesaturateFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(TextureSampler, input.TextureCoordinate);

	// desaturate with dot product of color and a desat value
	float desatColor = dot(color, float3(0.3, 0.59, 0.11));
	color.rgb = lerp(color, desatColor, DesaturationStrength);
	return color;
}

// make the technique available for drawing
technique DesaturateTechnique
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 DesaturateFunction();
	}
}
