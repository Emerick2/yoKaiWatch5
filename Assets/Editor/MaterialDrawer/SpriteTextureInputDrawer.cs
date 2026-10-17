using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SpriteTextureShaderGraph.Editor
{
    /// <summary>
    /// Material inspector drawer for [SpriteTextureInput].
    ///
    /// Features:
    /// - Texture2D or Sprite object picker.
    /// - Native Unity Tiling/Offset controls.
    /// - Multi-material editing.
    /// - Preserves Tiling/Offset when the selected Sprite changes.
    /// - Resolves a material's hidden Sprite transform back to its Sprite asset.
    /// - Compact Sprite metadata/warning line.
    /// </summary>
    public sealed class SpriteTextureInputDrawer : MaterialPropertyDrawer
    {
        private const float Epsilon = 0.00001f;

        public override void OnGUI(
            Rect position,
            MaterialProperty prop,
            string label,
            MaterialEditor editor)
        {
            float line = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            Rect objectRect = new Rect(
                position.x,
                position.y,
                position.width,
                line
            );

            DrawObjectField(
                objectRect,
                prop,
                label,
                editor
            );

            float y = objectRect.yMax + spacing;

            if (HasNativeTilingAndOffset(prop))
            {
                float nativeHeight =
                    line * 2f + spacing;

                Rect scaleOffsetRect = new Rect(
                    position.x,
                    y,
                    position.width,
                    nativeHeight
                );

                // Use Unity's own MaterialEditor control instead of recreating it.
                editor.TextureScaleOffsetProperty(
                    scaleOffsetRect,
                    prop
                );

                y = scaleOffsetRect.yMax + spacing;
            }

            Sprite sprite = GetFirstResolvedSprite(prop);

            if (sprite != null)
            {
                Rect infoRect = new Rect(
                    position.x,
                    y,
                    position.width,
                    line
                );

                DrawSpriteInfo(infoRect, prop, sprite);
            }
        }

        public override float GetPropertyHeight(
            MaterialProperty prop,
            string label,
            MaterialEditor editor)
        {
            float line = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            float height = line;

            if (HasNativeTilingAndOffset(prop))
                height += spacing + line * 2f + spacing;

            if (GetFirstResolvedSprite(prop) != null)
                height += spacing + line;

            return height;
        }

        private static void DrawObjectField(
            Rect position,
            MaterialProperty prop,
            string label,
            MaterialEditor editor)
        {
            UnityEngine.Object current =
                GetCommonDisplayedObject(
                    prop,
                    out bool mixed
                );

            EditorGUI.showMixedValue = mixed;
            EditorGUI.BeginChangeCheck();

            UnityEngine.Object selected =
                EditorGUI.ObjectField(
                    position,
                    new GUIContent(
                        label,
                        "Texture2D or sliced Sprite"
                    ),
                    current,
                    typeof(UnityEngine.Object),
                    false
                );

            if (EditorGUI.EndChangeCheck())
            {
                if (selected != null &&
                    !(selected is Texture2D) &&
                    !(selected is Sprite))
                {
                    Debug.LogWarning(
                        "Sprite Texture only accepts " +
                        "Texture2D or Sprite assets."
                    );
                }
                else
                {
                    editor.RegisterPropertyChangeUndo(
                        "Change " + label
                    );

                    foreach (UnityEngine.Object target
                             in prop.targets)
                    {
                        Material material =
                            target as Material;

                        if (material == null)
                            continue;

                        ApplyObject(
                            material,
                            prop.name,
                            selected
                        );

                        EditorUtility.SetDirty(material);
                    }

                    GUI.changed = true;
                    editor.Repaint();
                }
            }

            EditorGUI.showMixedValue = false;
        }

        private static void DrawSpriteInfo(
            Rect position,
            MaterialProperty prop,
            Sprite sprite)
        {
            float inset = GetInset(
                prop.targets
                    .OfType<Material>()
                    .FirstOrDefault(),
                prop.name
            );

            SpriteTextureTransform transform =
                SpriteTextureRuntime.GetSpriteTransform(
                    sprite,
                    inset
                );

            string packed = sprite.packed
                ? $"{sprite.packingMode}/{sprite.packingRotation}"
                : "No";

            string text =
                $"{sprite.rect.width:0.#}×" +
                $"{sprite.rect.height:0.#} px  •  " +
                $"PPU {sprite.pixelsPerUnit:0.##}  •  " +
                $"Packed {packed}";

            if (transform.HasCrossAxisTerms)
                text += "  •  Use Sample Sprite Texture 2D";

            EditorGUI.LabelField(
                position,
                GUIContent.none,
                new GUIContent(
                    text,
                    "Pivot: " + sprite.pivot +
                    "\nBorder: " + sprite.border
                ),
                EditorStyles.miniLabel
            );
        }

        private static bool HasNativeTilingAndOffset(
            MaterialProperty prop)
        {
            return
                (prop.flags &
                 MaterialProperty.PropFlags.NoScaleOffset) == 0;
        }

        private static UnityEngine.Object GetCommonDisplayedObject(
            MaterialProperty prop,
            out bool mixed)
        {
            mixed = false;

            UnityEngine.Object common = null;
            bool first = true;

            foreach (UnityEngine.Object target in prop.targets)
            {
                Material material = target as Material;

                if (material == null)
                    continue;

                UnityEngine.Object current =
                    ResolveDisplayedObject(
                        material,
                        prop.name
                    );

                if (first)
                {
                    common = current;
                    first = false;
                }
                else if (common != current)
                {
                    mixed = true;
                    return common;
                }
            }

            return common;
        }

        private static Sprite GetFirstResolvedSprite(
            MaterialProperty prop)
        {
            Material material =
                prop.targets
                    .OfType<Material>()
                    .FirstOrDefault();

            return material != null
                ? ResolveDisplayedObject(
                    material,
                    prop.name
                  ) as Sprite
                : null;
        }

        private static UnityEngine.Object ResolveDisplayedObject(
            Material material,
            string textureProperty)
        {
            Texture2D texture =
                material.GetTexture(textureProperty)
                as Texture2D;

            if (texture == null)
                return null;

            string modeProperty =
                textureProperty + "_SpriteMode";

            bool isSprite =
                material.HasProperty(modeProperty) &&
                material.GetFloat(modeProperty) > 0.5f;

            if (!isSprite)
                return texture;

            string row0Property =
                textureProperty + "_SpriteTransform0";

            string row1Property =
                textureProperty + "_SpriteTransform1";

            if (!material.HasProperty(row0Property) ||
                !material.HasProperty(row1Property))
            {
                return ResolveLegacySprite(
                    material,
                    textureProperty,
                    texture
                );
            }

            Vector4 wantedRow0 =
                material.GetVector(row0Property);

            Vector4 wantedRow1 =
                material.GetVector(row1Property);

            float inset =
                GetInset(material, textureProperty);

            // The Sprite may live under a different source texture when packed,
            // so first try all Sprite assets in the backing texture's path.
            string texturePath =
                AssetDatabase.GetAssetPath(texture);

            Sprite found =
                FindMatchingSpriteInPath(
                    texturePath,
                    wantedRow0,
                    wantedRow1,
                    inset
                );

            if (found != null)
                return found;

            // Fallback: global project search by texture name is deliberately
            // avoided because it is expensive and ambiguous.
            return texture;
        }

        private static Sprite FindMatchingSpriteInPath(
            string path,
            Vector4 wantedRow0,
            Vector4 wantedRow1,
            float inset)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            foreach (Sprite sprite in AssetDatabase
                         .LoadAllAssetsAtPath(path)
                         .OfType<Sprite>())
            {
                if (sprite == null)
                    continue;

                SpriteTextureTransform transform =
                    SpriteTextureRuntime.GetSpriteTransform(
                        sprite,
                        inset
                    );

                if (Approximately(
                        transform.Row0,
                        wantedRow0
                    ) &&
                    Approximately(
                        transform.Row1,
                        wantedRow1
                    ))
                {
                    return sprite;
                }
            }

            return null;
        }

        private static UnityEngine.Object ResolveLegacySprite(
            Material material,
            string textureProperty,
            Texture2D texture)
        {
            string legacyRectProperty =
                textureProperty + "_SpriteRect";

            if (!material.HasProperty(legacyRectProperty))
                return texture;

            Vector4 wanted =
                material.GetVector(legacyRectProperty);

            string path =
                AssetDatabase.GetAssetPath(texture);

            if (string.IsNullOrEmpty(path))
                return texture;

            foreach (Sprite sprite in AssetDatabase
                         .LoadAllAssetsAtPath(path)
                         .OfType<Sprite>())
            {
                SpriteTextureTransform transform =
                    SpriteTextureRuntime.GetSpriteTransform(
                        sprite,
                        0f
                    );

                if (Approximately(
                    transform.CompatibilityScaleOffset,
                    wanted
                ))
                    return sprite;
            }

            return texture;
        }

        private static void ApplyObject(
            Material material,
            string textureProperty,
            UnityEngine.Object selected)
        {
            if (selected == null)
            {
                SpriteTextureRuntime.SetTexture(
                    material,
                    textureProperty,
                    null
                );

                return;
            }

            if (selected is Sprite sprite)
            {
                float inset =
                    GetInset(material, textureProperty);

                SpriteTextureRuntime.SetSprite(
                    material,
                    textureProperty,
                    sprite,
                    inset
                );

                return;
            }

            SpriteTextureRuntime.SetTexture(
                material,
                textureProperty,
                selected as Texture2D
            );
        }

        private static float GetInset(
            Material material,
            string textureProperty)
        {
            if (material == null)
                return 0f;

            string property =
                textureProperty + "_SpriteInset";

            return material.HasProperty(property)
                ? Mathf.Max(
                    0f,
                    material.GetFloat(property)
                  )
                : 0f;
        }

        private static bool Approximately(
            Vector4 a,
            Vector4 b)
        {
            return
                Mathf.Abs(a.x - b.x) <= Epsilon &&
                Mathf.Abs(a.y - b.y) <= Epsilon &&
                Mathf.Abs(a.z - b.z) <= Epsilon &&
                Mathf.Abs(a.w - b.w) <= Epsilon;
        }
    }
}
