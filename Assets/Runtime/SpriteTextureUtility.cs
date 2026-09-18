using UnityEngine;

/// <summary>
/// Runtime helper for the Texture2D + Sprite UV rectangle representation used by
/// SpriteTextureShaderGUI.
/// </summary>
public static class SpriteTextureUtility
{
    public static readonly Vector4 FullRect = new Vector4(0f, 0f, 1f, 1f);

    /// <summary>
    /// Returns XY = normalized offset, ZW = normalized size.
    /// </summary>
    public static Vector4 GetUvRect(Sprite sprite)
    {
        if (sprite == null || sprite.texture == null)
            return FullRect;

        Texture2D texture = sprite.texture;

        Rect rect;
        try
        {
            // textureRect is preferable for packed sprites and is identical to
            // rect for ordinary sliced sprite sheets.
            rect = sprite.textureRect;
        }
        catch
        {
            rect = sprite.rect;
        }

        return new Vector4(
            rect.x / texture.width,
            rect.y / texture.height,
            rect.width / texture.width,
            rect.height / texture.height
        );
    }

    public static Vector2 RemapUv(Vector2 uv, Vector4 spriteRect)
    {
        return new Vector2(
            uv.x * spriteRect.z + spriteRect.x,
            uv.y * spriteRect.w + spriteRect.y
        );
    }

    public static void SetSprite(
        Material material,
        string textureProperty,
        string rectProperty,
        Sprite sprite)
    {
        if (material == null)
            return;

        if (sprite == null)
        {
            SetTexture(material, textureProperty, rectProperty, null);
            return;
        }

        material.SetTexture(textureProperty, sprite.texture);
        material.SetVector(rectProperty, GetUvRect(sprite));
    }

    public static void SetTexture(
        Material material,
        string textureProperty,
        string rectProperty,
        Texture2D texture)
    {
        if (material == null)
            return;

        material.SetTexture(textureProperty, texture);
        material.SetVector(rectProperty, FullRect);
    }

    public static void SetSprite(
        MaterialPropertyBlock block,
        string textureProperty,
        string rectProperty,
        Sprite sprite)
    {
        if (block == null)
            return;

        if (sprite == null)
        {
            SetTexture(block, textureProperty, rectProperty, null);
            return;
        }

        block.SetTexture(textureProperty, sprite.texture);
        block.SetVector(rectProperty, GetUvRect(sprite));
    }

    public static void SetTexture(
        MaterialPropertyBlock block,
        string textureProperty,
        string rectProperty,
        Texture2D texture)
    {
        if (block == null)
            return;

        block.SetTexture(textureProperty, texture);
        block.SetVector(rectProperty, FullRect);
    }
}
