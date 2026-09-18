using System;
using UnityEngine;

/// <summary>
/// Full affine Sprite UV transform.
/// Atlas UV:
///
///     U = Row0.x * localU + Row0.y * localV + Row0.z
///     V = Row1.x * localU + Row1.y * localV + Row1.z
///
/// This representation supports ordinary sliced sprites, flips, 180-degree
/// rotation, and affine atlas packing that swaps the U/V axes.
/// </summary>
[Serializable]
public readonly struct SpriteTextureTransform
{
    public readonly Vector4 Row0;
    public readonly Vector4 Row1;

    public SpriteTextureTransform(
        Vector4 row0,
        Vector4 row1)
    {
        Row0 = row0;
        Row1 = row1;
    }

    public static SpriteTextureTransform Identity =>
        new SpriteTextureTransform(
            new Vector4(1f, 0f, 0f, 0f),
            new Vector4(0f, 1f, 0f, 0f)
        );

    public bool HasCrossAxisTerms =>
        Mathf.Abs(Row0.y) > 0.00001f ||
        Mathf.Abs(Row1.x) > 0.00001f;

    public Vector4 CompatibilityScaleOffset =>
        new Vector4(
            Row0.x,
            Row1.y,
            Row0.z,
            Row1.z
        );

    public Vector2 TransformUv(Vector2 localUv)
    {
        return new Vector2(
            Row0.x * localUv.x +
            Row0.y * localUv.y +
            Row0.z,

            Row1.x * localUv.x +
            Row1.y * localUv.y +
            Row1.z
        );
    }
}

/// <summary>
/// Runtime API for the custom Shader Graph Sprite Texture property.
/// </summary>
public static class SpriteTextureRuntime
{
    public const string Transform0Suffix =
        "_SpriteTransform0";

    public const string Transform1Suffix =
        "_SpriteTransform1";

    public const string LegacyRectSuffix =
        "_SpriteRect";

    public const string SpriteModeSuffix =
        "_SpriteMode";

    public const string SpriteInsetSuffix =
        "_SpriteInset";

    public const string TilingOffsetSuffix =
        "_ST";

    public static SpriteTextureTransform GetSpriteTransform(
        Sprite sprite,
        float insetPixels = 0f)
    {
        if (sprite == null || sprite.texture == null)
            return SpriteTextureTransform.Identity;

        if (TryGetAffineFromGeometry(
            sprite,
            insetPixels,
            out SpriteTextureTransform transform
        ))
            return transform;

        return GetTransformFromTextureRect(
            sprite,
            insetPixels
        );
    }

    public static void SetSprite(
        Material material,
        string textureProperty,
        Sprite sprite)
    {
        if (material == null)
            return;

        float inset =
            GetConfiguredInset(
                material,
                textureProperty
            );

        SetSprite(
            material,
            textureProperty,
            sprite,
            inset
        );
    }

    public static void SetSprite(
        Material material,
        string textureProperty,
        Sprite sprite,
        float insetPixels)
    {
        if (material == null)
            return;

        if (sprite == null)
        {
            SetTexture(
                material,
                textureProperty,
                null
            );

            return;
        }

        SpriteTextureTransform transform =
            GetSpriteTransform(
                sprite,
                insetPixels
            );

        material.SetTexture(
            textureProperty,
            sprite.texture
        );

        ApplyTransform(
            material,
            textureProperty,
            transform
        );

        material.SetFloat(
            textureProperty + SpriteModeSuffix,
            1f
        );
    }

    /// <summary>
    /// Assigns one of a Sprite's Secondary Sprite Textures while preserving the
    /// Sprite's UV transform. Useful for normal/mask secondary textures.
    /// Returns false if the named secondary texture does not exist.
    /// </summary>
    public static bool TrySetSecondarySpriteTexture(
        Material material,
        string textureProperty,
        Sprite sprite,
        string secondaryTextureName,
        float insetPixels = 0f)
    {
        if (material == null ||
            sprite == null ||
            string.IsNullOrEmpty(secondaryTextureName))
            return false;

        int count = sprite.GetSecondaryTextureCount();

        if (count <= 0)
            return false;

        var secondary =
            new SecondarySpriteTexture[count];

        int written =
            sprite.GetSecondaryTextures(secondary);

        for (int i = 0; i < written; i++)
        {
            if (!string.Equals(
                secondary[i].name,
                secondaryTextureName,
                StringComparison.Ordinal
            ))
                continue;

            Texture2D texture =
                secondary[i].texture;

            if (texture == null)
                return false;

            SpriteTextureTransform transform =
                GetSpriteTransform(
                    sprite,
                    insetPixels
                );

            material.SetTexture(
                textureProperty,
                texture
            );

            ApplyTransform(
                material,
                textureProperty,
                transform
            );

            material.SetFloat(
                textureProperty + SpriteModeSuffix,
                1f
            );

            return true;
        }

        return false;
    }

    public static void SetTexture(
        Material material,
        string textureProperty,
        Texture2D texture)
    {
        if (material == null)
            return;

        material.SetTexture(
            textureProperty,
            texture
        );

        ApplyTransform(
            material,
            textureProperty,
            SpriteTextureTransform.Identity
        );

        material.SetFloat(
            textureProperty + SpriteModeSuffix,
            0f
        );
    }

    public static void Clear(
        Material material,
        string textureProperty)
    {
        SetTexture(
            material,
            textureProperty,
            null
        );
    }

    public static Vector2 GetTiling(
        Material material,
        string textureProperty)
    {
        return material != null
            ? material.GetTextureScale(textureProperty)
            : Vector2.one;
    }

    public static Vector2 GetOffset(
        Material material,
        string textureProperty)
    {
        return material != null
            ? material.GetTextureOffset(textureProperty)
            : Vector2.zero;
    }

    public static void SetTiling(
        Material material,
        string textureProperty,
        Vector2 tiling)
    {
        if (material != null)
            material.SetTextureScale(
                textureProperty,
                tiling
            );
    }

    public static void SetOffset(
        Material material,
        string textureProperty,
        Vector2 offset)
    {
        if (material != null)
            material.SetTextureOffset(
                textureProperty,
                offset
            );
    }

    public static void SetTilingAndOffset(
        Material material,
        string textureProperty,
        Vector2 tiling,
        Vector2 offset)
    {
        if (material == null)
            return;

        material.SetTextureScale(
            textureProperty,
            tiling
        );

        material.SetTextureOffset(
            textureProperty,
            offset
        );
    }

    public static void ResetTilingAndOffset(
        Material material,
        string textureProperty)
    {
        SetTilingAndOffset(
            material,
            textureProperty,
            Vector2.one,
            Vector2.zero
        );
    }

    public static void SetSprite(
        MaterialPropertyBlock block,
        string textureProperty,
        Sprite sprite,
        float insetPixels = 0f)
    {
        if (block == null)
            return;

        if (sprite == null)
        {
            SetTexture(
                block,
                textureProperty,
                null
            );

            return;
        }

        SpriteTextureTransform transform =
            GetSpriteTransform(
                sprite,
                insetPixels
            );

        block.SetTexture(
            textureProperty,
            sprite.texture
        );

        ApplyTransform(
            block,
            textureProperty,
            transform
        );

        block.SetFloat(
            textureProperty + SpriteModeSuffix,
            1f
        );
    }

    public static void SetTexture(
        MaterialPropertyBlock block,
        string textureProperty,
        Texture2D texture)
    {
        if (block == null)
            return;

        block.SetTexture(
            textureProperty,
            texture
        );

        ApplyTransform(
            block,
            textureProperty,
            SpriteTextureTransform.Identity
        );

        block.SetFloat(
            textureProperty + SpriteModeSuffix,
            0f
        );
    }

    public static void SetTilingAndOffset(
        MaterialPropertyBlock block,
        string textureProperty,
        Vector2 tiling,
        Vector2 offset)
    {
        if (block == null)
            return;

        block.SetVector(
            textureProperty + TilingOffsetSuffix,
            new Vector4(
                tiling.x,
                tiling.y,
                offset.x,
                offset.y
            )
        );
    }

    private static void ApplyTransform(
        Material material,
        string textureProperty,
        SpriteTextureTransform transform)
    {
        material.SetVector(
            textureProperty + Transform0Suffix,
            transform.Row0
        );

        material.SetVector(
            textureProperty + Transform1Suffix,
            transform.Row1
        );

        material.SetVector(
            textureProperty + LegacyRectSuffix,
            transform.CompatibilityScaleOffset
        );
    }

    private static void ApplyTransform(
        MaterialPropertyBlock block,
        string textureProperty,
        SpriteTextureTransform transform)
    {
        block.SetVector(
            textureProperty + Transform0Suffix,
            transform.Row0
        );

        block.SetVector(
            textureProperty + Transform1Suffix,
            transform.Row1
        );

        block.SetVector(
            textureProperty + LegacyRectSuffix,
            transform.CompatibilityScaleOffset
        );
    }

    private static float GetConfiguredInset(
        Material material,
        string textureProperty)
    {
        string property =
            textureProperty + SpriteInsetSuffix;

        return material.HasProperty(property)
            ? Mathf.Max(
                0f,
                material.GetFloat(property)
              )
            : 0f;
    }

    private static bool TryGetAffineFromGeometry(
        Sprite sprite,
        float insetPixels,
        out SpriteTextureTransform transform)
    {
        Vector2[] vertices = sprite.vertices;
        Vector2[] uvs = sprite.uv;

        if (vertices == null ||
            uvs == null ||
            vertices.Length != uvs.Length ||
            vertices.Length < 3 ||
            sprite.rect.width <= 0f ||
            sprite.rect.height <= 0f ||
            sprite.pixelsPerUnit <= 0f)
        {
            transform = SpriteTextureTransform.Identity;
            return false;
        }

        Vector2 p0 = Local01(sprite, vertices[0]);

        for (int i = 1; i < vertices.Length - 1; i++)
        {
            Vector2 p1 =
                Local01(sprite, vertices[i]);

            for (int j = i + 1;
                 j < vertices.Length;
                 j++)
            {
                Vector2 p2 =
                    Local01(sprite, vertices[j]);

                Vector2 d1 = p1 - p0;
                Vector2 d2 = p2 - p0;

                float determinant =
                    d1.x * d2.y -
                    d1.y * d2.x;

                if (Mathf.Abs(determinant) <
                    0.000001f)
                    continue;

                Vector2 q0 = uvs[0];
                Vector2 q1 = uvs[i] - q0;
                Vector2 q2 = uvs[j] - q0;

                float m00 =
                    (q1.x * d2.y -
                     q2.x * d1.y) /
                    determinant;

                float m01 =
                    (-q1.x * d2.x +
                      q2.x * d1.x) /
                    determinant;

                float m10 =
                    (q1.y * d2.y -
                     q2.y * d1.y) /
                    determinant;

                float m11 =
                    (-q1.y * d2.x +
                      q2.y * d1.x) /
                    determinant;

                float offsetU =
                    q0.x -
                    m00 * p0.x -
                    m01 * p0.y;

                float offsetV =
                    q0.y -
                    m10 * p0.x -
                    m11 * p0.y;

                ApplyInset(
                    sprite,
                    insetPixels,
                    ref m00,
                    ref m01,
                    ref m10,
                    ref m11,
                    ref offsetU,
                    ref offsetV
                );

                transform =
                    new SpriteTextureTransform(
                        new Vector4(
                            m00,
                            m01,
                            offsetU,
                            0f
                        ),
                        new Vector4(
                            m10,
                            m11,
                            offsetV,
                            0f
                        )
                    );

                return true;
            }
        }

        transform = SpriteTextureTransform.Identity;
        return false;
    }

    private static SpriteTextureTransform
        GetTransformFromTextureRect(
            Sprite sprite,
            float insetPixels)
    {
        Rect rect;

        try
        {
            rect = sprite.textureRect;
        }
        catch
        {
            rect = sprite.rect;
        }

        Texture2D texture = sprite.texture;

        float insetX = Mathf.Clamp(
            insetPixels,
            0f,
            Mathf.Max(
                0f,
                rect.width * 0.5f - 0.001f
            )
        );

        float insetY = Mathf.Clamp(
            insetPixels,
            0f,
            Mathf.Max(
                0f,
                rect.height * 0.5f - 0.001f
            )
        );

        float width =
            Mathf.Max(
                0.001f,
                rect.width - insetX * 2f
            );

        float height =
            Mathf.Max(
                0.001f,
                rect.height - insetY * 2f
            );

        return new SpriteTextureTransform(
            new Vector4(
                width / texture.width,
                0f,
                (rect.x + insetX) /
                texture.width,
                0f
            ),
            new Vector4(
                0f,
                height / texture.height,
                (rect.y + insetY) /
                texture.height,
                0f
            )
        );
    }

    private static Vector2 Local01(
        Sprite sprite,
        Vector2 vertex)
    {
        return new Vector2(
            (vertex.x * sprite.pixelsPerUnit +
             sprite.pivot.x) /
            sprite.rect.width,

            (vertex.y * sprite.pixelsPerUnit +
             sprite.pivot.y) /
            sprite.rect.height
        );
    }

    private static void ApplyInset(
        Sprite sprite,
        float insetPixels,
        ref float m00,
        ref float m01,
        ref float m10,
        ref float m11,
        ref float offsetU,
        ref float offsetV)
    {
        float insetU = Mathf.Clamp(
            insetPixels / sprite.rect.width,
            0f,
            0.4999f
        );

        float insetV = Mathf.Clamp(
            insetPixels / sprite.rect.height,
            0f,
            0.4999f
        );

        float scaleU =
            1f - insetU * 2f;

        float scaleV =
            1f - insetV * 2f;

        offsetU +=
            m00 * insetU +
            m01 * insetV;

        offsetV +=
            m10 * insetU +
            m11 * insetV;

        m00 *= scaleU;
        m10 *= scaleU;

        m01 *= scaleV;
        m11 *= scaleV;
    }
}
