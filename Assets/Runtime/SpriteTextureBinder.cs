using UnityEngine;

/// <summary>
/// Convenience component for assigning a Sprite Texture through a
/// MaterialPropertyBlock without cloning the Renderer material.
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
public sealed class SpriteTextureBinder : MonoBehaviour
{
    [SerializeField]
    private Renderer targetRenderer;

    [SerializeField]
    private string propertyName = "_MainTex";

    [SerializeField]
    private Sprite sprite;

    [SerializeField]
    [Min(0f)]
    private float insetPixels;

    [SerializeField]
    private bool overrideTilingAndOffset;

    [SerializeField]
    private Vector2 tiling = Vector2.one;

    [SerializeField]
    private Vector2 offset = Vector2.zero;

    private MaterialPropertyBlock propertyBlock;

    private void Reset()
    {
        targetRenderer = GetComponent<Renderer>();
        Apply();
    }

    private void OnEnable()
    {
        Apply();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Apply();
    }
#endif

    public void Apply()
    {
        if (targetRenderer == null ||
            string.IsNullOrEmpty(propertyName))
            return;

        if (propertyBlock == null)
            propertyBlock =
                new MaterialPropertyBlock();

        targetRenderer.GetPropertyBlock(
            propertyBlock
        );

        SpriteTextureRuntime.SetSprite(
            propertyBlock,
            propertyName,
            sprite,
            insetPixels
        );

        if (overrideTilingAndOffset)
        {
            SpriteTextureRuntime
                .SetTilingAndOffset(
                    propertyBlock,
                    propertyName,
                    tiling,
                    offset
                );
        }

        targetRenderer.SetPropertyBlock(
            propertyBlock
        );
    }

    public void SetSprite(Sprite value)
    {
        sprite = value;
        Apply();
    }

    public void SetTilingAndOffset(
        Vector2 newTiling,
        Vector2 newOffset)
    {
        tiling = newTiling;
        offset = newOffset;
        overrideTilingAndOffset = true;
        Apply();
    }
}
