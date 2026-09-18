using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SpriteTextureShaderGraph.Editor
{
    internal static class SpriteTextureDiagnostics
    {
        [MenuItem(
            "Tools/Sprite Texture/Validate Selected Materials"
        )]
        private static void ValidateSelectedMaterials()
        {
            Material[] materials =
                Selection.objects
                    .OfType<Material>()
                    .ToArray();

            if (materials.Length == 0)
            {
                Debug.Log(
                    "Sprite Texture: select one or more Materials first."
                );
                return;
            }

            int warnings = 0;

            foreach (Material material in materials)
            {
                Shader shader = material.shader;

                if (shader == null)
                    continue;

                int count = shader.GetPropertyCount();

                for (int i = 0; i < count; i++)
                {
                    string name =
                        shader.GetPropertyName(i);

                    if (!name.EndsWith(
                        SpriteTextureRuntime.SpriteModeSuffix
                    ))
                        continue;

                    string baseName =
                        name.Substring(
                            0,
                            name.Length -
                            SpriteTextureRuntime
                                .SpriteModeSuffix.Length
                        );

                    string row0 =
                        baseName +
                        SpriteTextureRuntime.Transform0Suffix;

                    string row1 =
                        baseName +
                        SpriteTextureRuntime.Transform1Suffix;

                    if (!material.HasProperty(row0) ||
                        !material.HasProperty(row1))
                    {
                        warnings++;

                        Debug.LogWarning(
                            $"Sprite Texture: '{material.name}' " +
                            $"is missing transform properties for " +
                            $"'{baseName}'. Re-save/recompile the Shader Graph.",
                            material
                        );
                    }
                }
            }

            Debug.Log(
                warnings == 0
                    ? $"Sprite Texture: {materials.Length} selected " +
                      $"material(s) validated successfully."
                    : $"Sprite Texture: validation finished with " +
                      $"{warnings} warning(s)."
            );
        }
    }
}
