// 2D display rendering shader
// Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com

// Pixel input data
struct PixelData
{
    float4 pos		: POSITION;
    float4 color	: COLOR0;
    float2 uv		: TEXCOORD0;
};

// Settings
// x = texel width
// y = texel height
// z = FSAA blend factor
// w = transparency
float4 settings;

// Texture1 input
texture texture1
<
    string UIName = "Texture1";
    string ResourceType = "2D";
>;

// Texture sampler settings
sampler2D texture1samp = sampler_state
{
    Texture = <texture1>;
    MagFilter = Point;
    MinFilter = Point;
    MipFilter = Point;
	AddressU = Wrap;
	AddressV = Wrap;
}; 

// This blends the max of 2 pixels
float4 addcolor(float4 c1, float4 c2)
{
	return float4(max(c1.r, c2.r),
				  max(c1.g, c2.g),
				  max(c1.b, c2.b),
				  saturate(c1.a + c2.a * 0.5f));
}

// Pixel shader
float4 ps_main(PixelData pd) : COLOR
{
	// Take this pixel's color
	float4 c = tex2D(texture1samp, pd.uv);
	
	// If this pixel is not drawn on...
	if(c.a < 0.1f)
	{
		// Mix the colors of nearby pixels
		float4 n = (float4)0;
		n = addcolor(n, tex2D(texture1samp, float2(pd.uv.x + settings.x, pd.uv.y)));
		n = addcolor(n, tex2D(texture1samp, float2(pd.uv.x - settings.x, pd.uv.y)));
		n = addcolor(n, tex2D(texture1samp, float2(pd.uv.x, pd.uv.y + settings.y)));
		n = addcolor(n, tex2D(texture1samp, float2(pd.uv.x, pd.uv.y - settings.y)));
		
		// If any pixels nearby where found, return a blend, otherwise return nothing
		//if(n.a > 0.1f) return float4(n.rgb, n.a * settings.z); else return (float4)0;
		return float4(n.rgb, n.a * settings.z * settings.w);
	}
	else return float4(c.rgb, c.a * settings.w);
}

// Technique for shader model 2.0
technique SM20
{
	pass p0
	{
	    VertexShader = null;
	    PixelShader = compile ps_2_0 ps_main();
	}
}
