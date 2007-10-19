// Base 2D rendering shader
// Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com

// Pixel input data
struct PixelData
{
    float4 pos		: POSITION;
    float2 uv		: TEXCOORD0;
};

// Settings
// x = texel width
// y = texel height
// z = blend factor
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
    MagFilter = Linear;
    MinFilter = Linear;
    MipFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
}; 

// This blends the max of 2 pixels
float4 addcolor(float4 c1, float4 c2)
{
	return float4(max(c1.r, c2.r),
				  max(c1.g, c2.g),
				  max(c1.b, c2.b),
				  max(c1.a, c2.a));
}

// Pixel shader
float4 ps20_main(PixelData pd) : COLOR
{
	float4 c = tex2D(texture1samp, pd.uv);
	float4 neightbourblend = 
			tex2D(texture1samp, float2(pd.uv.x + settings.x, pd.uv.y)) * settings.z +
			tex2D(texture1samp, float2(pd.uv.x - settings.x, pd.uv.y)) * settings.z +
			tex2D(texture1samp, float2(pd.uv.x, pd.uv.y + settings.y)) * settings.z +
			tex2D(texture1samp, float2(pd.uv.x, pd.uv.y - settings.y)) * settings.z;
	return lerp(neightbourblend, c, c.a);
}

// Technique for shader model 2.0
technique SM20
{
	pass p0
	{
	    VertexShader = null;
	    PixelShader = compile ps_2_0 ps20_main();
		CullMode = None;
	    ZEnable = false;
	    AlphaBlendEnable = true;
	    SrcBlend = SrcAlpha;
	    DestBlend = InvSrcAlpha;
	}
}
