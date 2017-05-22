// This sampler is set automatically based on the render texture source
sampler DarknessTexture : register(s0);

float darknessStart;
float alphaPerPixel;
float cameraTop;
float textureHeight;

float4 DarknessFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(DarknessTexture, texCoord);
	//return color;
	float pixelY = cameraTop - (textureHeight * texCoord.y);
	if (pixelY < darknessStart && color.a == 0)
	{
		color.a = abs(darknessStart - pixelY) * alphaPerPixel;
	}

	return color;
}

technique DarknessTechnique
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 DarknessFunction();
	}
}
