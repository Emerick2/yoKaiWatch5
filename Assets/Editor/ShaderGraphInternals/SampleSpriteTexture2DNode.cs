using System;
using System.Linq;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    enum SpriteLocalWrapMode
    {
        None,
        Clamp,
        Repeat,
        Mirror
    }

    enum SpriteTextureSampleType
    {
        Default,
        Normal
    }

    /// <summary>
    /// Sprite-aware alternative to Sample Texture 2D.
    ///
    /// When its Texture input is connected directly to a Sprite Texture property,
    /// this node uses the property's full affine Sprite transform instead of only
    /// UnityTexture2D.scaleTranslate.
    ///
    /// This enables:
    /// - atlas-local Clamp
    /// - atlas-local Repeat
    /// - atlas-local Mirror
    /// - packed affine rotation
    /// - half-texel bleed protection
    ///
    /// If the input is not a direct Sprite Texture property, it falls back to the
    /// normal UnityTexture2D transformed UV behavior.
    /// </summary>
    [Serializable]
    [Title("Input", "Texture", "Sample Sprite Texture 2D")]
    sealed class SampleSpriteTexture2DNode :
        AbstractMaterialNode,
        IGeneratesBodyCode,
        IMayRequireMeshUV,
        IMayRequireTime
    {
        public const int OutputRGBA = 0;
        public const int TextureInput = 1;
        public const int UVInput = 2;
        public const int SamplerInput = 3;
        public const int OutputR = 4;
        public const int OutputG = 5;
        public const int OutputB = 6;
        public const int OutputA = 7;
        public const int ScrollDirectionInput = 8;
        public const int ScrollSpeedInput = 9;

        [SerializeField]
        private SpriteLocalWrapMode m_WrapMode =
            SpriteLocalWrapMode.None;

        [SerializeField]
        private SpriteTextureSampleType m_TextureType =
            SpriteTextureSampleType.Default;

        [SerializeField]
        private bool m_PreventBleeding = true;

        public SampleSpriteTexture2DNode()
        {
            name = "Sample Sprite Texture 2D";
            synonyms = new[]
            {
                "sprite",
                "sample sprite",
                "atlas",
                "texture2d",
                "scroll",
                "panner"
            };

            UpdateNodeAfterDeserialization();
        }

        public override bool hasPreview => true;

        [EnumControl("Wrap Inside Sprite")]
        public SpriteLocalWrapMode wrapMode
        {
            get => m_WrapMode;
            set
            {
                if (m_WrapMode == value)
                    return;

                m_WrapMode = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [EnumControl("Type")]
        public SpriteTextureSampleType textureType
        {
            get => m_TextureType;
            set
            {
                if (m_TextureType == value)
                    return;

                m_TextureType = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [ToggleControl("Prevent Bleeding")]
        public ToggleData preventBleeding
        {
            get => new ToggleData(m_PreventBleeding);
            set
            {
                if (m_PreventBleeding ==
                    value.isOn)
                    return;

                m_PreventBleeding =
                    value.isOn;

                Dirty(ModificationScope.Graph);
            }
        }

        public override void UpdateNodeAfterDeserialization()
        {
            AddSlot(
                new Vector4MaterialSlot(
                    OutputRGBA,
                    "RGBA",
                    "RGBA",
                    SlotType.Output,
                    Vector4.zero
                )
            );

            AddSlot(
                new Vector1MaterialSlot(
                    OutputR,
                    "R",
                    "R",
                    SlotType.Output,
                    0f
                )
            );

            AddSlot(
                new Vector1MaterialSlot(
                    OutputG,
                    "G",
                    "G",
                    SlotType.Output,
                    0f
                )
            );

            AddSlot(
                new Vector1MaterialSlot(
                    OutputB,
                    "B",
                    "B",
                    SlotType.Output,
                    0f
                )
            );

            AddSlot(
                new Vector1MaterialSlot(
                    OutputA,
                    "A",
                    "A",
                    SlotType.Output,
                    0f
                )
            );

            AddSlot(
                new Texture2DInputMaterialSlot(
                    TextureInput,
                    "Texture",
                    "Texture"
                )
            );

            AddSlot(
                new UVMaterialSlot(
                    UVInput,
                    "UV",
                    "UV",
                    UVChannel.UV0
                )
            );

            AddSlot(
                new SamplerStateMaterialSlot(
                    SamplerInput,
                    "Sampler",
                    "Sampler",
                    SlotType.Input
                )
            );

            AddSlot(
                new Vector2MaterialSlot(
                    ScrollDirectionInput,
                    "Direction",
                    "Direction",
                    SlotType.Input,
                    Vector2.zero
                )
            );

            AddSlot(
                new Vector1MaterialSlot(
                    ScrollSpeedInput,
                    "Speed",
                    "Speed",
                    SlotType.Input,
                    1f
                )
            );

            RemoveSlotsNameNotMatching(
                new[]
                {
                    OutputRGBA,
                    OutputR,
                    OutputG,
                    OutputB,
                    OutputA,
                    TextureInput,
                    UVInput,
                    SamplerInput,
                    ScrollDirectionInput,
                    ScrollSpeedInput
                }
            );
        }

        public void GenerateNodeCode(
            ShaderStringBuilder builder,
            GenerationMode generationMode)
        {
            string uv =
                GetSlotValue(
                    UVInput,
                    generationMode
                );

            string scrollDirection =
                GetSlotValue(
                    ScrollDirectionInput,
                    generationMode
                );

            string scrollSpeed =
                GetSlotValue(
                    ScrollSpeedInput,
                    generationMode
                );

            string texture =
                GetSlotValue(
                    TextureInput,
                    generationMode
                );

            MaterialSlot samplerSlot =
                FindInputSlot<MaterialSlot>(
                    SamplerInput
                );

            bool hasSampler =
                owner.GetEdges(
                    samplerSlot.slotReference
                ).Any();

            string sampler =
                hasSampler
                    ? GetSlotValue(
                        SamplerInput,
                        generationMode
                      )
                    : texture;

            SpriteTextureShaderProperty property =
                GetConnectedSpriteProperty();

            string sampleUv;

            if (property != null)
            {
                string localUv =
                    GetVariableNameForSlot(
                        UVInput
                    ) + "_SpriteLocal";

                builder.AppendLine(
                    $"$precision2 {localUv} = {uv};"
                );

                if (property.useTilingAndOffset)
                {
                    builder.AppendLine(
                        $"{localUv} = " +
                        $"{localUv} * " +
                        $"{property.referenceName}_ST.xy + " +
                        $"{property.referenceName}_ST.zw;"
                    );
                }

                builder.AppendLine(
                    $"{localUv} += " +
                    $"({scrollDirection}) * " +
                    $"({scrollSpeed}) * " +
                    $"IN.TimeParameters.x;"
                );

                AppendWrapCode(
                    builder,
                    localUv
                );

                string row0 =
                    property.referenceName +
                    "_SpriteTransform0";

                string row1 =
                    property.referenceName +
                    "_SpriteTransform1";

                if (m_PreventBleeding)
                {
                    string safe =
                        GetVariableNameForSlot(
                            UVInput
                        ) + "_SpriteSafe";

                    builder.AppendLine(
                        $"$precision2 {safe} = " +
                        $"0.5 / max(" +
                        $"$precision2(" +
                            $"length($precision2(" +
                                $"{row0}.x / {texture}.texelSize.x, " +
                                $"{row1}.x / {texture}.texelSize.y" +
                            $")), " +
                            $"length($precision2(" +
                                $"{row0}.y / {texture}.texelSize.x, " +
                                $"{row1}.y / {texture}.texelSize.y" +
                            $"))" +
                        $"), " +
                        $"$precision2(1e-5, 1e-5));"
                    );

                    builder.AppendLine(
                        $"{localUv} = clamp(" +
                        $"{localUv}, {safe}, 1.0 - {safe});"
                    );
                }

                sampleUv =
                    GetVariableNameForSlot(
                        UVInput
                    ) + "_SpriteAtlas";

                builder.AppendLine(
                    $"$precision2 {sampleUv} = " +
                    $"$precision2(" +
                        $"dot({row0}.xy, {localUv}) + {row0}.z, " +
                        $"dot({row1}.xy, {localUv}) + {row1}.z" +
                    $");"
                );
            }
            else
            {
                string scrollingUv =
                    GetVariableNameForSlot(
                        UVInput
                    ) + "_ScrollingUV";

                builder.AppendLine(
                    $"$precision2 {scrollingUv} = " +
                    $"({uv}) + " +
                    $"({scrollDirection}) * " +
                    $"({scrollSpeed}) * " +
                    $"IN.TimeParameters.x;"
                );

                sampleUv =
                    GetVariableNameForSlot(
                        UVInput
                    ) + "_TextureUV";

                builder.AppendLine(
                    $"$precision2 {sampleUv} = " +
                    $"{texture}.GetTransformedUV({scrollingUv});"
                );
            }

            string rgba =
                GetVariableNameForSlot(
                    OutputRGBA
                );

            builder.AppendLine(
                $"$precision4 {rgba} = " +
                $"SAMPLE_TEXTURE2D(" +
                    $"{texture}.tex, " +
                    $"{sampler}.samplerstate, " +
                    $"{sampleUv}" +
                $");"
            );

            if (textureType ==
                SpriteTextureSampleType.Normal)
            {
                builder.AppendLine(
                    $"{rgba}.rgb = UnpackNormal({rgba});"
                );
            }

            builder.AppendLine(
                $"$precision " +
                $"{GetVariableNameForSlot(OutputR)} = " +
                $"{rgba}.r;"
            );

            builder.AppendLine(
                $"$precision " +
                $"{GetVariableNameForSlot(OutputG)} = " +
                $"{rgba}.g;"
            );

            builder.AppendLine(
                $"$precision " +
                $"{GetVariableNameForSlot(OutputB)} = " +
                $"{rgba}.b;"
            );

            builder.AppendLine(
                $"$precision " +
                $"{GetVariableNameForSlot(OutputA)} = " +
                $"{rgba}.a;"
            );
        }

        private void AppendWrapCode(
            ShaderStringBuilder builder,
            string localUv)
        {
            switch (wrapMode)
            {
                case SpriteLocalWrapMode.Clamp:
                    builder.AppendLine(
                        $"{localUv} = saturate({localUv});"
                    );
                    break;

                case SpriteLocalWrapMode.Repeat:
                    builder.AppendLine(
                        $"{localUv} = frac({localUv});"
                    );
                    break;

                case SpriteLocalWrapMode.Mirror:
                    builder.AppendLine(
                        $"{localUv} = 1.0 - abs(" +
                        $"frac({localUv} * 0.5) * 2.0 - 1.0);"
                    );
                    break;
            }
        }

        private SpriteTextureShaderProperty
            GetConnectedSpriteProperty()
        {
            MaterialSlot textureSlot =
                FindInputSlot<MaterialSlot>(
                    TextureInput
                );

            var edge =
                owner.GetEdges(
                    textureSlot.slotReference
                ).FirstOrDefault();

            if (edge == null)
                return null;

            AbstractMaterialNode source =
                edge.outputSlot.node;

            PropertyNode propertyNode =
                source as PropertyNode;

            return propertyNode?.property
                as SpriteTextureShaderProperty;
        }

        public bool RequiresTime()
        {
            return true;
        }

        public bool RequiresMeshUV(
            UVChannel channel,
            ShaderStageCapability stageCapability)
        {
            using (var tempSlots =
                   PooledList<MaterialSlot>.Get())
            {
                GetInputSlots(tempSlots);

                foreach (MaterialSlot slot
                         in tempSlots)
                {
                    if (slot.RequiresMeshUV(channel))
                        return true;
                }
            }

            return false;
        }
    }
}