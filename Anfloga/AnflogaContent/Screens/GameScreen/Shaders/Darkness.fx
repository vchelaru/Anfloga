// This sampler is set automatically based on the render texture source
sampler DarknessTexture : register(s0);

float darknessStart;
float alphaPerPixel;
float cameraTop;
float textureHeight;

float4 DarknessFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(DarknessTexture, texCoord);
	float alpha = 0;
	float pixelY = cameraTop - (textureHeight * texCoord.y);
	if (pixelY < darknessStart)
	{
		alpha = abs(darknessStart - pixelY) * alphaPerPixel;
	}

	return float4(color.rgb, alpha);
}

technique DarknessTechnique
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 DarknessFunction();
	}
}
