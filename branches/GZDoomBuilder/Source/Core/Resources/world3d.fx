// 3D world rendering shader
// Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com

// Vertex input data
struct VertexData
{
    float3 pos		  : POSITION;
    float4 color	  : COLOR0;
    float2 uv		    : TEXCOORD0;
    float3 normal   : NORMAL; //mxd
};

// Pixel input data
struct PixelData
{
    float4 pos		: POSITION;
    float4 color	: COLOR0;
    float2 uv		  : TEXCOORD0;
};

//mxd
// Pixel input data for light pass
struct LitPixelData
{
    float4 pos		  : POSITION;
    float4 color	  : COLOR0;
    float2 uv		    : TEXCOORD0;
    float3 pos_w    : TEXCOORD1; //mxd. pixel position in world space
    float3 normal   : TEXCOORD2; //mxd. normal
};

// Modulation color
float4 modulatecolor;

// Highlight color
float4 highlightcolor;

// Matrix for final transformation
float4x4 worldviewproj;

//mxd
float4x4 world;
float4 vertexColor;
//light
float4 lightPosAndRadius;
float4 lightColor;

// Texture input
texture texture1;

// Filter settings
dword minfiltersettings;
dword magfiltersettings;
dword mipfiltersettings;
float maxanisotropysetting;

// Texture sampler settings
sampler2D texturesamp = sampler_state
{
    Texture = <texture1>;
    MagFilter = magfiltersettings;
    MinFilter = minfiltersettings;
    MipFilter = mipfiltersettings;
    MipMapLodBias = 0.0f;
    MaxAnisotropy = maxanisotropysetting;
};

// Vertex shader
PixelData vs_main(VertexData vd) {
    PixelData pd;
    // Fill pixel data input
    pd.pos = mul(float4(vd.pos, 1.0f), worldviewproj);
    pd.color = vd.color;
    pd.uv = vd.uv;
    // Return result
    return pd;
}

//mxd. same as vs_main, but uses vertexColor var instead of actual vertex color. used in models rendering
PixelData vs_customvertexcolor(VertexData vd) {
    PixelData pd;
    // Fill pixel data input
    pd.pos = mul(float4(vd.pos, 1.0f), worldviewproj);
    pd.color = vertexColor;
    pd.color.a = 1.0f;
    pd.uv = vd.uv;
    // Return result
    return pd;
}

//mxd. light pass vertex shader
LitPixelData vs_lightpass(VertexData vd) {
    LitPixelData pd;
    pd.pos = mul(float4(vd.pos, 1.0f), worldviewproj);
    pd.pos_w = mul(vd.pos, (float3x3)world);
    pd.color = vd.color;
    pd.uv = vd.uv;
    //pd.normal = normalize(mul(vd.normal, (float3x3)world));
    pd.normal = vd.normal;

    // Return result
    return pd;
}

// Normal pixel shader
float4 ps_main(PixelData pd) : COLOR
{
    float4 tcolor = tex2D(texturesamp, pd.uv);
    return tcolor * pd.color * modulatecolor;
}

// Full-bright pixel shader
float4 ps_fullbright(PixelData pd) : COLOR
{
    float4 tcolor = tex2D(texturesamp, pd.uv);
    tcolor.a *= pd.color.a;
	
    // Blend texture color and modulation color
    return tcolor * modulatecolor;
}

// Normal pixel shader with highlight
float4 ps_main_highlight(PixelData pd) : COLOR
{
	float4 tcolor = tex2D(texturesamp, pd.uv);
	
	// Blend texture color, vertex color and modulation color
	float4 ncolor = tcolor * pd.color * modulatecolor;
	float4 hcolor = float4(highlightcolor.rgb, ncolor.a);
	
    //return lerp(ncolor, hcolor, highlightcolor.a);
    return float4(hcolor.rgb * highlightcolor.a + (ncolor.rgb - 0.4f * highlightcolor.a), tcolor.a);
}

// Full-bright pixel shader with highlight
float4 ps_fullbright_highlight(PixelData pd) : COLOR
{
	float4 tcolor = tex2D(texturesamp, pd.uv);
	
	// Blend texture color and modulation color
	float4 ncolor = tcolor * modulatecolor;
	float4 hcolor = float4(highlightcolor.rgb, ncolor.a);
	
    //return lerp(ncolor, hcolor, highlightcolor.a);
    return float4(hcolor.rgb * highlightcolor.a + (ncolor.rgb - 0.4f * highlightcolor.a), tcolor.a);
}

//mxd: used to draw bounding boxes
float4 ps_constant_color(PixelData pd) : COLOR {
    return vertexColor;
}

//mxd. dynamic light pixel shader pass, dood!
float4 ps_lightpass(LitPixelData pd) : COLOR {
      //is face facing away from light source?
      if(dot(pd.normal, (lightPosAndRadius.xyz - pd.pos_w)) < -0.1f) // (lightPosAndRadius.xyz - pd.pos_w) == direction from light to current pixel
          clip(-1);

      //is pixel in light range?
      float dist = distance(pd.pos_w, lightPosAndRadius.xyz);
      if(dist > lightPosAndRadius.w)
          clip(-1);
      
      //is pixel tranparent?
      float4 tcolor = tex2D(texturesamp, pd.uv);
      if(tcolor.a == 0.0f)
          clip(-1);

      //if it is - calculate color at current pixel
      float4 lightColorMod = float4(0.0f, 0.0f, 0.0f, 0.0f);

      lightColorMod.rgb = lightColor.rgb * max(lightPosAndRadius.w - dist, 0.0f) / lightPosAndRadius.w;
      if(lightColorMod.r > 0.0f || lightColorMod.g > 0.0f || lightColorMod.b > 0.0f){
          lightColorMod.rgb *= lightColor.a;
          if(lightColor.a > 0.4f) //Normal, vavoom or negative light
              return tcolor * lightColorMod;
          return lightColorMod; //Additive light
      }
      clip(-1);
      return lightColorMod; //should never get here
}

// Technique for shader model 2.0
technique SM20 {
	// Normal
    pass p0 {
	    VertexShader = compile vs_2_0 vs_main();
	    PixelShader = compile ps_2_0 ps_main();
    }
    
    // Full brightness mode
    pass p1 {
	    VertexShader = compile vs_2_0 vs_main();
	    PixelShader = compile ps_2_0 ps_fullbright();
    }

	// Normal with highlight
    pass p2 {
	    VertexShader = compile vs_2_0 vs_main();
	    PixelShader = compile ps_2_0 ps_main_highlight();
    }
    
    // Full brightness mode with highlight
    pass p3 {
	    VertexShader = compile vs_2_0 vs_main();
	    PixelShader = compile ps_2_0 ps_fullbright_highlight();
    }
    
    //mxd. same as p0-p3, but using vertexColor variable
    // Normal
    pass p4 {
	    VertexShader = compile vs_2_0 vs_customvertexcolor();
	    PixelShader = compile ps_2_0 ps_main();
    }
    
    // Full brightness mode
    pass p5 {
	    VertexShader = compile vs_2_0 vs_customvertexcolor();
	    PixelShader = compile ps_2_0 ps_fullbright();
    }
	
	// Normal with highlight
    pass p6 {
	    VertexShader = compile vs_2_0 vs_customvertexcolor();
	    PixelShader = compile ps_2_0 ps_main_highlight();
    }

    // Full brightness mode with highlight
    pass p7 {
	    VertexShader = compile vs_2_0 vs_customvertexcolor();
	    PixelShader = compile ps_2_0 ps_fullbright_highlight();
    }
    
    //mxd. Just fills everything with vertexColor. Used in ThingCage rendering.
    pass p8 {
	    VertexShader = compile vs_2_0 vs_customvertexcolor();
	    PixelShader  = compile ps_2_0 ps_constant_color();
    }
    
    //mxd. Light pass
    pass p9 {
        VertexShader = compile vs_2_0 vs_lightpass();
        PixelShader  = compile ps_2_0 ps_lightpass();
        AlphaBlendEnable = true;
    }
}
