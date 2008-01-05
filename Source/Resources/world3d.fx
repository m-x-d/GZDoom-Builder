// 3D world rendering shader
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

// Matrix for final transformation
float4x4 worldviewproj;

// Texture input
texture texture1;

// Texture sampler settings
sampler2D texturesamp = sampler_state
{
    Texture = <texture1>;
    MagFilter = Linear;
    MinFilter = Anisotropic;
    MipFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
}; 

// Vertex shader
PixelData vs_main(VertexData vd)
{
    PixelData pd;
    
    // Fill pixel data input
    pd.pos = mul(float4(vd.pos, 1.0f), worldviewproj);
    pd.color = vd.color;
    pd.uv = vd.uv;
    
    // Return result
    return pd;
}

// Pixel shader
float4 ps_main(PixelData pd) : COLOR
{
	float4 tcolor = tex2D(texturesamp, pd.uv);
	
	// Blend texture color and vertex color
    return float4(tcolor.r * pd.color.r,
				  tcolor.g * pd.color.g,
				  tcolor.b * pd.color.b,
				  tcolor.a * pd.color.a);
}

// Technique for shader model 2.0
technique SM20
{
    pass p0
    {
	    VertexShader = compile vs_2_0 vs_main();
	    PixelShader = compile ps_2_0 ps_main();
    }
}
