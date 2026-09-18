using UnityEngine;

/// <summary>
/// Runtime helper for assigning a Sprite to the Texture2D + Vector4 pair used
/// by SpriteShaderGUI.
///
/// The UV vector convention is:
///     XY = normalized offset
///     ZW = normalized size
///
/// Example:
///     SpriteShaderUtility.SetSprite(
///         material,
///         "_IconSpriteTexture",
///         "_IconSpriteUV",
///         iconSprite
///     );
/// </summary>
public static class SpriteShaderUtility
{
    public static Vector4 GetUvRect(Sprite sprite)
    {
        if (sprite == null || sprite.texture == null)
            return new Vector4(0f, 0f, 1f, 1f);

        Texture2D texture = sprite.texture;
        Rect rect = sprite.rect;

        return new Vector4(
            rect.x / texture.width,
            rect.y / texture.height,
            rect.width / texture.width,
            rect.height / texture.height
        );
    }

    public static void SetSprite(
        Material material,
        string texturePropertyName,
        string uvPropertyName,
        Sprite sprite)
    {
        if (material == null)
            return;

        if (sprite == null)
        {
            material.SetTexture(texturePropertyName, null);
            material.SetVector(
                uvPropertyName,
                new Vector4(0f, 0f, 1f, 1f)
            );
            return;
        }

        material.SetTexture(texturePropertyName, sprite.texture);
        material.SetVector(uvPropertyName, GetUvRect(sprite));
    }

    public static void SetSprite(
        MaterialPropertyBlock block,
        string texturePropertyName,
        string uvPropertyName,
        Sprite sprite)
    {
        if (block == null)
            return;

        if (sprite == null)
        {
            block.SetTexture(texturePropertyName, null);
            block.SetVector(
                uvPropertyName,
                new Vector4(0f, 0f, 1f, 1f)
            );
            return;
        }

        block.SetTexture(texturePropertyName, sprite.texture);
        block.SetVector(uvPropertyName, GetUvRect(sprite));
    }

    public static Vector2 RemapUv(Vector2 uv, Vector4 spriteUv)
    {
        return new Vector2(
            uv.x * spriteUv.z + spriteUv.x,
            uv.y * spriteUv.w + spriteUv.y
        );
    }
}
