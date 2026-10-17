#ifndef SPRITE_TEXTURE_INPUT_INCLUDED
#define SPRITE_TEXTURE_INPUT_INCLUDED

// Shader Graph Custom Function:
// Function name: SpriteTextureUV
//
// Inputs:
//   UV         Vector2
//   SpriteRect Vector4
//
// Output:
//   Out        Vector2
//
// SpriteRect:
//   XY = normalized offset
//   ZW = normalized size

void SpriteTextureUV_float(float2 UV, float4 SpriteRect, out float2 Out)
{
    Out = UV * SpriteRect.zw + SpriteRect.xy;
}

void SpriteTextureUV_half(half2 UV, half4 SpriteRect, out half2 Out)
{
    Out = UV * SpriteRect.zw + SpriteRect.xy;
}

#endif
