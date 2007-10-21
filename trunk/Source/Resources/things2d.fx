// Things 2D rendering shader
// Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com

// Pixel input data
struct PixelData
{
    float4 pos		: POSITION;
    float2 uv		: TEXCOORD0;
};

// Colors
float4 thingcolor;

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
	MipMapLodBias = -0.99f;
};

// Pixel shader for colored circle
float4 ps_circle(PixelData pd) : COLOR
{
	float4 c = tex2D(texture1samp, pd.uv);
	float4 s = tex2D(texture1samp, pd.uv + float2(0.25f, 0.0f));
	return float4(lerp(c.rgb * thingcolor.rgb, s.rgb, s.a), c.a * thingcolor.a);
}

// Pixel shader for icon
float4 ps_icon(PixelData pd) : COLOR
{
	return tex2D(texture1samp, pd.uv);
}

// Technique for shader model 2.0
technique SM20
{
	pass p0
	{
	    VertexShader = null;
	    PixelShader = compile ps_2_0 ps_circle();
		CullMode = None;
	    ZEnable = false;
	    AlphaBlendEnable = true;
	    SrcBlend = SrcAlpha;
	    DestBlend = InvSrcAlpha;
	}
	
	pass p1
	{
	    VertexShader = null;
	    PixelShader = compile ps_2_0 ps_icon();
		CullMode = None;
	    ZEnable = false;
	    AlphaBlendEnable = true;
	    SrcBlend = SrcAlpha;
	    DestBlend = InvSrcAlpha;
	}
}
