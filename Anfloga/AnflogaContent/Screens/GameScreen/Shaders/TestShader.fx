// This sampler is set automatically based on the render texture source
sampler SpriteBatchTexture : register(s0);
sampler DisplacementTexture : register(s1);

float ViewerX = .5;
float ViewerY = .5;
float BlurStrength = 0.036;

float4 DisplacementFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(DisplacementTexture, texCoord);

	float2 modifiedTexCoord = texCoord;
	float range = .02;
	modifiedTexCoord.y = modifiedTexCoord.y - range/2 + (range * color.r);


	return tex2D(SpriteBatchTexture, modifiedTexCoord);
}

technique DisplacementTechnique
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 DisplacementFunction();
	}
}

float4 BlurFunction(float2 texCoord : TEXCOORD0) : COLOR0
{

	float ratioX = abs(texCoord.x - ViewerX);
	float ratioY = abs(texCoord.y - ViewerY);

	float blurRatio = max(ratioX, ratioY) * BlurStrength;

	float4 color = 0;

	int samples = 4;
	float samplesSquared = 16;
	float blurOver2 = blurRatio / 2;
	float blurOverSamples = blurRatio / samples;

	for (int x = 0; x < samples; x++)
	{
		for (int y = 0; y < samples; y++)
		{
			float2 coord = texCoord;
			coord.x += -blurOver2 + (blurOverSamples * x);
			coord.y += -blurOver2 + (blurOverSamples * y);
			color += tex2D(SpriteBatchTexture, coord) / samplesSquared;
		}
	}
	return color;
}

technique BlurTechnique
{
	pass Pass1
	{
		//VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_2_0 BlurFunction();
	}
}