// Things 2D rendering shader
// Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com

// Vertex input data
struct VertexData
{
	float3 pos		: POSITION;
	float4 color	: COLOR0;
	float2 uv		: TEXCOORD0;
};

// Pixel input data
struct PixelData
{
	float4 pos		: POSITION;
	float4 color	: COLOR0;
	float2 uv		: TEXCOORD0;
};

// Render settings
// w = transparency
float4 rendersettings;

//mxd. solid fill color. used in model wireframe rendering
float4 fillColor;

// Transform settings
float4x4 transformsettings;

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
	MipMapLodBias = -0.9f;
};

//mxd. Texture sampler settings for sprite rendering
sampler2D texture1sprite = sampler_state
{
	Texture = <texture1>;
	MagFilter = Point;
	MinFilter = Point;
	MipFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
	MipMapLodBias = 0.0f;
};

// Transformation
PixelData vs_transform(VertexData vd)
{
	PixelData pd = (PixelData)0;
	pd.pos = mul(float4(vd.pos, 1.0f), transformsettings);
	pd.color = vd.color;
	pd.uv = vd.uv;
	return pd;
}

//mxd. Pixel shader for sprite drawing
float4 ps_sprite(PixelData pd) : COLOR
{
	// Take this pixel's color
	float4 c = tex2D(texture1sprite, pd.uv);
	return float4(c.rgb, c.a * rendersettings.w) * pd.color;
}

//mxd. Pixel shader for thing box and arrow drawing
float4 ps_thing(PixelData pd) : COLOR
{
	// Take this pixel's color
	float4 c = tex2D(texture1samp, pd.uv);
	return float4(c.rgb, c.a * rendersettings.w) * pd.color;
}

//mxd. Pretty darn simple pixel shader for wireframe rendering :)
float4 ps_fill(PixelData pd) : COLOR {
	return fillColor;
}

// Technique for shader model 2.0
technique SM20
{
	pass p0 //mxd
	{
		VertexShader = compile vs_2_0 vs_transform();
		PixelShader = compile ps_2_0 ps_thing();
	}

	pass p1 //mxd
	{
		VertexShader = compile vs_2_0 vs_transform();
		PixelShader = compile ps_2_0 ps_sprite();
	}


        pass p2 //mxd
	{
		VertexShader = compile vs_2_0 vs_transform();
		PixelShader = compile ps_2_0 ps_fill();
	}
}
