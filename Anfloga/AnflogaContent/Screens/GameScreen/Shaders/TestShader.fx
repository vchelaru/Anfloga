// This sampler is set automatically based on the render texture source
sampler SpriteBatchTexture : register(s0);
sampler DisplacementTexture : register(s1);



float ViewerX = .5;
float ViewerY = .5;
float BlurStrength = 0.036;
float FocusArea = .25f;

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

float4 DistanceBlurFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
	float ratioX = abs(texCoord.x - ViewerX);
	float ratioY = abs(texCoord.y - ViewerY);

	float blurRatio = max(0, max(ratioX, ratioY) - FocusArea) * BlurStrength;

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

float4 HighPassFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
	float4 returnColor = (float4)0;

	for (int x = 0; x < 4; x++)
	{
		for (int y = 0; y < 4; y++)
		{
			float2 coord;
			coord.x = (texCoord.x - .01) + .02 * (x + .5)/3;
			coord.y = (texCoord.y - .01) + .02* (y + .5)/3;
			float4 color = tex2D(SpriteBatchTexture, coord);
			//float brightness = color.r * 0.299f + color.g * 0.587f + color.b * 0.114f;
	
			//returnColor += color * brightness / 1;
			returnColor += ((color - .45) * 3) / 16;
		}
	}


	return returnColor;

}

technique BloomTechnique
{
	pass Pass1
	{
		//VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_2_0 HighPassFunction();
	}
}

technique DistanceBlurTechnique
{
	pass Pass1
	{
		//VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_2_0 DistanceBlurFunction();
	}
}