using System;
using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers.ShaderInputPropertyDrawer;

namespace UnityEditor.ShaderGraph.Internal
{
    /// <summary>
    /// Texture2D-compatible Shader Graph property with first-class Sprite support.
    ///
    /// It intentionally remains PropertyType.Texture2D so every existing Texture2D
    /// compatible node can consume it.
    ///
    /// Hidden shader data:
    ///   <ref>_ST               Native Unity material Tiling/Offset.
    ///   <ref>_SpriteTransform0 Atlas U transform: (m00, m01, offsetU, unused).
    ///   <ref>_SpriteTransform1 Atlas V transform: (m10, m11, offsetV, unused).
    ///   <ref>_SpriteRect       Compatibility axis-aligned transform.
    ///   <ref>_SpriteMode       1 when a Sprite is selected, otherwise 0.
    ///   <ref>_SpriteInset      Inspector/runtime anti-bleed inset in pixels.
    ///
    /// The normal Sample Texture 2D node uses an affine-compatible _ST
    /// representation. For arbitrary packed rotation and local wrap modes,
    /// use the companion Sample Sprite Texture 2D node.
    /// </summary>
    [Serializable]
    [BlackboardInputInfo(50.5f, "Sprite Texture")]
    public sealed class SpriteTextureShaderProperty :
        AbstractShaderProperty<SerializableTexture>,
        IShaderPropertyDrawer
    {
        [SerializeField]
        private SerializableSprite m_Sprite = new SerializableSprite();

        [SerializeField]
        private Texture2DShaderProperty.DefaultType m_DefaultType =
            Texture2DShaderProperty.DefaultType.White;

        [SerializeField]
        private bool m_UseTilingAndOffset;

        [SerializeField]
        private bool m_IsMainTexture;

        [SerializeField]
        private bool m_Modifiable = true;

        [SerializeField]
        private bool m_PerRendererData;

        [SerializeField]
        private float m_SpriteInsetPixels;

        public SpriteTextureShaderProperty()
        {
            displayName = "Sprite Texture";
            value = new SerializableTexture();
            generatePropertyBlock = true;
        }

        public override PropertyType propertyType => PropertyType.Texture2D;

        internal override bool isExposable => true;
        internal override bool isRenamable => true;

        internal bool useTilingAndOffset
        {
            get => m_UseTilingAndOffset;
            set => m_UseTilingAndOffset = value;
        }

        internal Texture2DShaderProperty.DefaultType defaultType
        {
            get => m_DefaultType;
            set => m_DefaultType = value;
        }

        internal bool isMainTexture
        {
            get => m_IsMainTexture;
            set => m_IsMainTexture = value;
        }

        internal bool modifiable
        {
            get => m_Modifiable;
            set => m_Modifiable = value;
        }

        internal bool perRendererData
        {
            get => m_PerRendererData;
            set => m_PerRendererData = value;
        }

        internal float spriteInsetPixels
        {
            get => Mathf.Max(0f, m_SpriteInsetPixels);
            set => m_SpriteInsetPixels = Mathf.Max(0f, value);
        }

        internal Sprite sprite
        {
            get => m_Sprite != null ? m_Sprite.sprite : null;
            set
            {
                if (m_Sprite == null)
                    m_Sprite = new SerializableSprite();

                m_Sprite.sprite = value;
            }
        }

        private UnityEngine.Object editorObject
        {
            get
            {
                Sprite selectedSprite = sprite;
                return selectedSprite != null
                    ? selectedSprite
                    : value.texture;
            }
        }

        internal SpriteTextureEditorTransform GetSpriteTransform()
        {
            Sprite selectedSprite = sprite;

            return selectedSprite != null
                ? SpriteTextureEditorTransform.FromSprite(
                    selectedSprite,
                    spriteInsetPixels
                )
                : SpriteTextureEditorTransform.Identity;
        }

        internal override void GetPropertyReferenceNames(List<string> result)
        {
            base.GetPropertyReferenceNames(result);

            if (useTilingAndOffset)
                result.Add(referenceName + "_ST");

            result.Add(referenceName + "_SpriteTransform0");
            result.Add(referenceName + "_SpriteTransform1");
            result.Add(referenceName + "_SpriteRect");
            result.Add(referenceName + "_SpriteMode");
            result.Add(referenceName + "_SpriteInset");
        }

        internal override string GetPropertyBlockString()
        {
            string normalTag =
                defaultType == Texture2DShaderProperty.DefaultType.NormalMap
                    ? "[Normal]"
                    : string.Empty;

            string mainTextureTag =
                isMainTexture
                    ? "[MainTexture]"
                    : string.Empty;

            string scaleOffsetTag =
                useTilingAndOffset
                    ? string.Empty
                    : "[NoScaleOffset]";

            string modifiableTag =
                modifiable
                    ? string.Empty
                    : "[NonModifiableTextureData]";

            string perRendererTag =
                perRendererData
                    ? "[PerRendererData]"
                    : string.Empty;

            string fallback =
                Texture2DShaderProperty.ToShaderLabString(defaultType);

            return
                $"{hideTagString}" +
                $"[SpriteTextureInput]" +
                $"{perRendererTag}" +
                $"{modifiableTag}" +
                $"{normalTag}" +
                $"{mainTextureTag}" +
                $"{scaleOffsetTag}" +
                $"{referenceName}(\"{displayName}\", 2D) = " +
                $"\"{fallback}\" {{}}";
        }

        internal override void AppendPropertyBlockStrings(
            ShaderStringBuilder builder)
        {
            builder.AppendLine(GetPropertyBlockString());

            SpriteTextureEditorTransform transform = GetSpriteTransform();
            Vector4 compatibilityRect =
                transform.GetCompatibilityScaleOffset();

            builder.AppendLine(
                $"[HideInInspector]{referenceName}_SpriteTransform0(" +
                $"\"{displayName} Sprite Transform U\", Vector) = " +
                $"{ToShaderVector(transform.row0)}"
            );

            builder.AppendLine(
                $"[HideInInspector]{referenceName}_SpriteTransform1(" +
                $"\"{displayName} Sprite Transform V\", Vector) = " +
                $"{ToShaderVector(transform.row1)}"
            );

            builder.AppendLine(
                $"[HideInInspector]{referenceName}_SpriteRect(" +
                $"\"{displayName} Sprite Rect\", Vector) = " +
                $"{ToShaderVector(compatibilityRect)}"
            );

            builder.AppendLine(
                $"[HideInInspector]{referenceName}_SpriteMode(" +
                $"\"{displayName} Sprite Mode\", Float) = " +
                $"{ToShaderFloat(sprite != null ? 1f : 0f)}"
            );

            builder.AppendLine(
                $"[HideInInspector]{referenceName}_SpriteInset(" +
                $"\"{displayName} Sprite Inset\", Float) = " +
                $"{ToShaderFloat(spriteInsetPixels)}"
            );
        }

        internal override bool AllowHLSLDeclaration(
            HLSLDeclaration declaration)
        {
            // Same practical limitation as a Texture2D property.
            return declaration != HLSLDeclaration.HybridPerInstance &&
                   declaration != HLSLDeclaration.DoNotDeclare;
        }

        internal override void ForeachHLSLProperty(
            Action<HLSLProperty> action)
        {
            HLSLDeclaration declaration = generatePropertyBlock
                ? HLSLDeclaration.UnityPerMaterial
                : HLSLDeclaration.Global;

            action(new HLSLProperty(
                HLSLType._Texture2D,
                referenceName,
                HLSLDeclaration.Global
            ));

            action(new HLSLProperty(
                HLSLType._SamplerState,
                "sampler" + referenceName,
                HLSLDeclaration.Global
            ));

            action(new HLSLProperty(
                HLSLType._float4,
                referenceName + "_TexelSize",
                declaration
            ));

            if (useTilingAndOffset)
            {
                action(new HLSLProperty(
                    HLSLType._float4,
                    referenceName + "_ST",
                    declaration
                ));
            }

            action(new HLSLProperty(
                HLSLType._float4,
                referenceName + "_SpriteTransform0",
                declaration
            ));

            action(new HLSLProperty(
                HLSLType._float4,
                referenceName + "_SpriteTransform1",
                declaration
            ));

            // Keep the legacy-compatible axis-aligned transform available.
            action(new HLSLProperty(
                HLSLType._float4,
                referenceName + "_SpriteRect",
                declaration
            ));
        }

        internal override string GetPropertyAsArgumentString(
            string precisionString)
        {
            return "UnityTexture2D " + referenceName;
        }

        internal override string GetPropertyAsArgumentStringForVFX(
            string precisionString)
        {
            return "TEXTURE2D(" + referenceName + ")";
        }

        internal override string GetHLSLVariableName(
            bool isSubgraphProperty,
            GenerationMode mode)
        {
            if (isSubgraphProperty)
                return referenceName;

            string rect = referenceName + "_SpriteRect";

            if (!useTilingAndOffset)
            {
                return
                    $"UnityBuildTexture2DStructInternal(" +
                    $"TEXTURE2D_ARGS({referenceName}, sampler{referenceName}), " +
                    $"{referenceName}_TexelSize, {rect})";
            }

            string st = referenceName + "_ST";

            // Standard Texture2D consumers only understand scale + translation.
            // This path exactly handles normal, horizontal flip, vertical flip,
            // and 180-degree packed sprites. 90-degree/arbitrary atlas packing is
            // handled by Sample Sprite Texture 2D using the full 2x3 transform.
            return
                $"UnityBuildTexture2DStructInternal(" +
                $"TEXTURE2D_ARGS({referenceName}, sampler{referenceName}), " +
                $"{referenceName}_TexelSize, " +
                $"float4(" +
                    $"{st}.xy * {rect}.xy, " +
                    $"{rect}.zw + {st}.zw * {rect}.xy" +
                $"))";
        }

        internal override AbstractMaterialNode ToConcreteNode()
        {
            // A concrete Texture2D asset node cannot carry Sprite metadata.
            // Keep the backing texture when converting the property.
            return new Texture2DAssetNode
            {
                texture = value.texture
            };
        }

        internal override PreviewProperty GetPreviewMaterialProperty()
        {
            return new PreviewProperty(propertyType)
            {
                name = referenceName,
                textureValue = value.texture,
                texture2DDefaultType = defaultType
            };
        }

        internal override ShaderInput Copy()
        {
            var copy = new SpriteTextureShaderProperty
            {
                displayName = displayName,
                value = value,
                defaultType = defaultType,
                useTilingAndOffset = useTilingAndOffset,
                isMainTexture = isMainTexture,
                modifiable = modifiable,
                perRendererData = perRendererData,
                spriteInsetPixels = spriteInsetPixels
            };

            copy.sprite = sprite;
            return copy;
        }

        void IShaderPropertyDrawer.HandlePropertyField(
            PropertySheet propertySheet,
            PreChangeValueCallback preChangeValueCallback,
            PostChangeValueCallback postChangeValueCallback)
        {
            DrawDefaultField(
                propertySheet,
                preChangeValueCallback,
                postChangeValueCallback
            );

            DrawEnumField(
                propertySheet,
                "Mode",
                defaultType,
                value =>
                {
                    defaultType =
                        (Texture2DShaderProperty.DefaultType)value;
                },
                "Change Sprite Texture Mode",
                preChangeValueCallback,
                postChangeValueCallback
            );

            DrawToggleField(
                propertySheet,
                "Use Tiling and Offset",
                useTilingAndOffset,
                value => useTilingAndOffset = value,
                "Change Use Tiling and Offset",
                preChangeValueCallback,
                postChangeValueCallback
            );

            DrawToggleField(
                propertySheet,
                "Main Texture",
                isMainTexture,
                value => isMainTexture = value,
                "Change Main Texture",
                preChangeValueCallback,
                postChangeValueCallback
            );

            DrawToggleField(
                propertySheet,
                "Modifiable",
                modifiable,
                value => modifiable = value,
                "Change Modifiable",
                preChangeValueCallback,
                postChangeValueCallback
            );

            DrawToggleField(
                propertySheet,
                "Per Renderer Data",
                perRendererData,
                value => perRendererData = value,
                "Change Per Renderer Data",
                preChangeValueCallback,
                postChangeValueCallback
            );

            DrawInsetField(
                propertySheet,
                preChangeValueCallback,
                postChangeValueCallback
            );

            DrawSpriteInfo(propertySheet);
        }

        private void DrawDefaultField(
            PropertySheet propertySheet,
            PreChangeValueCallback preChangeValueCallback,
            PostChangeValueCallback postChangeValueCallback)
        {
            UnityEngine.Object currentObject = editorObject;

            var field = new ObjectField("Default")
            {
                objectType = typeof(UnityEngine.Object),
                allowSceneObjects = false,
                value = currentObject
            };

            field.tooltip =
                "Accepts either a Texture2D or a sliced Sprite.";

            field.RegisterValueChangedCallback(evt =>
            {
                UnityEngine.Object newObject = evt.newValue;

                if (newObject != null &&
                    !(newObject is Texture2D) &&
                    !(newObject is Sprite))
                {
                    field.SetValueWithoutNotify(currentObject);

                    Debug.LogWarning(
                        "Sprite Texture only accepts Texture2D or Sprite assets."
                    );

                    return;
                }

                preChangeValueCallback(
                    "Change Sprite Texture Default"
                );

                if (newObject is Sprite newSprite)
                {
                    sprite = newSprite;
                    value.texture = newSprite.texture;
                }
                else
                {
                    sprite = null;
                    value.texture = newObject as Texture2D;
                }

                currentObject = newObject;

                postChangeValueCallback(
                    true,
                    ModificationScope.Graph
                );
            });

            propertySheet.Add(field);
        }

        private static void DrawEnumField(
            PropertySheet propertySheet,
            string label,
            Enum currentValue,
            Action<Enum> apply,
            string undoName,
            PreChangeValueCallback preChangeValueCallback,
            PostChangeValueCallback postChangeValueCallback)
        {
            var field = new EnumField(label, currentValue);

            field.RegisterValueChangedCallback(evt =>
            {
                if (Equals(evt.newValue, currentValue))
                    return;

                preChangeValueCallback(undoName);
                apply(evt.newValue);

                postChangeValueCallback(
                    true,
                    ModificationScope.Graph
                );
            });

            propertySheet.Add(field);
        }

        private static void DrawToggleField(
            PropertySheet propertySheet,
            string label,
            bool currentValue,
            Action<bool> apply,
            string undoName,
            PreChangeValueCallback preChangeValueCallback,
            PostChangeValueCallback postChangeValueCallback)
        {
            var field = new Toggle(label)
            {
                value = currentValue
            };

            field.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == currentValue)
                    return;

                preChangeValueCallback(undoName);
                apply(evt.newValue);

                postChangeValueCallback(
                    true,
                    ModificationScope.Graph
                );
            });

            propertySheet.Add(field);
        }

        private void DrawInsetField(
            PropertySheet propertySheet,
            PreChangeValueCallback preChangeValueCallback,
            PostChangeValueCallback postChangeValueCallback)
        {
            var field = new UnityEngine.UIElements.FloatField(
                "Sprite Inset (px)"
            )
            {
                value = spriteInsetPixels
            };

            field.tooltip =
                "Shrinks the sampled Sprite region inward to reduce texture " +
                "bleeding from neighbouring cells.";

            field.RegisterValueChangedCallback(evt =>
            {
                float newValue = Mathf.Max(0f, evt.newValue);

                if (Mathf.Approximately(
                    newValue,
                    spriteInsetPixels
                ))
                    return;

                preChangeValueCallback(
                    "Change Sprite Inset"
                );

                spriteInsetPixels = newValue;
                field.SetValueWithoutNotify(newValue);

                postChangeValueCallback(
                    true,
                    ModificationScope.Graph
                );
            });

            propertySheet.Add(field);
        }

        private void DrawSpriteInfo(PropertySheet propertySheet)
        {
            Sprite selected = sprite;

            if (selected == null)
                return;

            string packed =
                selected.packed
                    ? $"Packed {selected.packingMode} / " +
                      $"{selected.packingRotation}"
                    : "Not packed";

            string text =
                $"Sprite: {selected.rect.width:0.#}×" +
                $"{selected.rect.height:0.#} px, " +
                $"PPU {selected.pixelsPerUnit:0.##}, " +
                $"{packed}";

            var label = new Label(text);
            label.style.whiteSpace = WhiteSpace.Normal;
            label.tooltip =
                "Pivot: " + selected.pivot +
                "\nBorder: " + selected.border;

            propertySheet.Add(label);

            SpriteTextureEditorTransform transform = GetSpriteTransform();

            if (transform.hasCrossAxisTerms)
            {
                var warning = new HelpBox(
                    "This packed Sprite uses an affine rotation. " +
                    "Use 'Sample Sprite Texture 2D' for exact sampling. " +
                    "The normal Sample Texture 2D node only supports the " +
                    "axis-aligned compatibility transform.",
                    HelpBoxMessageType.Warning
                );

                propertySheet.Add(warning);
            }
        }

        private static string ToShaderVector(Vector4 value)
        {
            return
                $"({ToShaderFloat(value.x)}, " +
                $"{ToShaderFloat(value.y)}, " +
                $"{ToShaderFloat(value.z)}, " +
                $"{ToShaderFloat(value.w)})";
        }

        private static string ToShaderFloat(float value)
        {
            return NodeUtils.FloatToShaderValueShaderLabSafe(value);
        }
    }

    /// <summary>
    /// Editor-side affine transform helper kept in Unity.ShaderGraph.Editor so
    /// SpriteTextureShaderProperty does not depend on the project's runtime assembly.
    /// </summary>
    internal readonly struct SpriteTextureEditorTransform
    {
        internal readonly Vector4 row0;
        internal readonly Vector4 row1;

        internal static SpriteTextureEditorTransform Identity =>
            new SpriteTextureEditorTransform(
                new Vector4(1f, 0f, 0f, 0f),
                new Vector4(0f, 1f, 0f, 0f)
            );

        internal bool hasCrossAxisTerms =>
            Mathf.Abs(row0.y) > 0.00001f ||
            Mathf.Abs(row1.x) > 0.00001f;

        internal SpriteTextureEditorTransform(
            Vector4 row0,
            Vector4 row1)
        {
            this.row0 = row0;
            this.row1 = row1;
        }

        internal Vector4 GetCompatibilityScaleOffset()
        {
            return new Vector4(
                row0.x,
                row1.y,
                row0.z,
                row1.z
            );
        }

        internal static SpriteTextureEditorTransform FromSprite(
            Sprite sprite,
            float insetPixels)
        {
            if (sprite == null || sprite.texture == null)
                return Identity;

            if (TryFromGeometry(
                sprite,
                insetPixels,
                out SpriteTextureEditorTransform transform
            ))
                return transform;

            return FromTextureRect(sprite, insetPixels);
        }

        private static bool TryFromGeometry(
            Sprite sprite,
            float insetPixels,
            out SpriteTextureEditorTransform transform)
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
                transform = Identity;
                return false;
            }

            Vector2 p0 = Local01(sprite, vertices[0]);

            for (int i = 1; i < vertices.Length - 1; i++)
            {
                Vector2 p1 = Local01(sprite, vertices[i]);

                for (int j = i + 1; j < vertices.Length; j++)
                {
                    Vector2 p2 = Local01(sprite, vertices[j]);

                    Vector2 d1 = p1 - p0;
                    Vector2 d2 = p2 - p0;

                    float det =
                        d1.x * d2.y -
                        d1.y * d2.x;

                    if (Mathf.Abs(det) < 0.000001f)
                        continue;

                    Vector2 q0 = uvs[0];
                    Vector2 q1 = uvs[i] - q0;
                    Vector2 q2 = uvs[j] - q0;

                    float m00 =
                        (q1.x * d2.y -
                         q2.x * d1.y) / det;

                    float m01 =
                        (-q1.x * d2.x +
                          q2.x * d1.x) / det;

                    float m10 =
                        (q1.y * d2.y -
                         q2.y * d1.y) / det;

                    float m11 =
                        (-q1.y * d2.x +
                          q2.y * d1.x) / det;

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
                        new SpriteTextureEditorTransform(
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

            transform = Identity;
            return false;
        }

        private static SpriteTextureEditorTransform FromTextureRect(
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
                Mathf.Max(0f, rect.width * 0.5f - 0.001f)
            );

            float insetY = Mathf.Clamp(
                insetPixels,
                0f,
                Mathf.Max(0f, rect.height * 0.5f - 0.001f)
            );

            float width =
                Mathf.Max(0.001f, rect.width - insetX * 2f);

            float height =
                Mathf.Max(0.001f, rect.height - insetY * 2f);

            return new SpriteTextureEditorTransform(
                new Vector4(
                    width / texture.width,
                    0f,
                    (rect.x + insetX) / texture.width,
                    0f
                ),
                new Vector4(
                    0f,
                    height / texture.height,
                    (rect.y + insetY) / texture.height,
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

            float scaleU = 1f - insetU * 2f;
            float scaleV = 1f - insetV * 2f;

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
}
