// 2D textured rendering shader
// Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com

// Pixel input data
struct PixelData
{
    float4 pos		: POSITION;
    float4 color	: COLOR0;
    float2 uv		: TEXCOORD0;
};

// Texture input
texture texture1;

// Texture sampler settings
sampler2D texturesamp = sampler_state
{
    Texture = <texture1>;
    MagFilter = Linear;
    MinFilter = Linear;
    MipFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
}; 

// Pixel shader
float4 ps_normal(PixelData pd) : COLOR
{
	return pd.color * tex2D(texturesamp, pd.uv);
}

// Technique for shader model 2.0
technique SM20
{
	pass p0
	{
	    VertexShader = null;
	    PixelShader = compile ps_2_0 ps_normal();
	}
}
