//https://github.com/daniel-ilett/smo-shaders/blob/master/Assets/Shaders/Complete/BoxBlurSinglepass.shader
void BoxBlurAlpha_float(in UnityTexture2D MainTex, in float2 MainTex_TexelSize, in float KernelSize, in float2 UV, out float Alpha)
{
	if (tex2D(MainTex, UV.xy).a >= 1.0f) {
		Alpha = 1;
		return;
	}

	float a = 0.0;

	KernelSize = (int)KernelSize;
	int upper = ((KernelSize - 1) / 2);
	// Sum over all pixel colours in the kernel.
	[loop]
	for (int x = -upper; x <= upper; x++)
	{
		[loop]
		for (int y = -upper; y <= upper; y++)
		{
			float2 offset = float2(MainTex_TexelSize.x * x, MainTex_TexelSize.y * y);
			a += tex2D(MainTex, UV.xy + offset).a;
		}
	}

	// Divide through to get an unweighted average pixel colour.
	Alpha = a / (KernelSize * KernelSize);
}