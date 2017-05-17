// This sampler is set automatically based on the render texture source
sampler SpriteBatchTexture : register(s0);
sampler DisplacementTexture : register(s1);

float ViewerX = .5;
float ViewerY = .5;

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

	// * 2 * .018
	// * 2 to double the half, .018 is the ratio
	// for blurryness

	float blurRatio = max(ratioX, ratioY) * .036;

	float4 color = 0;

	int samples = 4;
	float samplesSquared = 16;

	for (int x = 0; x < samples; x++)
	{
		for (int y = 0; y < samples; y++)
		{
			float2 coord = texCoord;
			coord.x += -blurRatio / 2 + (blurRatio * x / samples);
			coord.y += -blurRatio / 2 + (blurRatio * y / samples);
			color += tex2D(SpriteBatchTexture, coord) / samplesSquared;
		}
	}
	return color;
}

void SpriteVertexShader(inout float4 color    : COLOR0,
	inout float2 texCoord : TEXCOORD0,
	inout float4 position : POSITION0)
{
}

technique BlurTechnique
{
	pass Pass1
	{
		//VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_2_0 BlurFunction();
	}
}