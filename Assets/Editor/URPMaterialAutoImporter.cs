
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

public class URPMaterialAutoImporter : AssetPostprocessor
{
    public override int GetPostprocessOrder()
    {
        // Après les postprocesseurs URP natifs.
        return 10000;
    }

    private void OnPreprocessMaterialDescription(
        MaterialDescription description,
        Material material,
        AnimationClip[] animations)
    {
        var settings = ImportedMaterialSettings.instance;

        if (!settings.Enabled)
            return;

        // Tous les formats gérés par ModelImporter.
        if (!(assetImporter is ModelImporter))
            return;

        Shader shader = settings.GetShader();

        if (shader == null)
        {
            Debug.LogWarning(
                "[Material Import] Shader introuvable : " +
                settings.ShaderName);
            return;
        }

        if (material.shader == shader)
            return;

        // Conserver les shaders personnalisés si souhaité.
        if (!settings.OverrideAllShaders)
        {
            if (material.shader != null)
            {
                string name = material.shader.name;

                if (name != "Standard" &&
                    name != "Standard (Specular setup)" &&
                    !name.StartsWith("Legacy Shaders/"))
                    return;
            }
        }

        // Récupération des propriétés existantes.
        Texture albedo = GetTexture(
            material, "_BaseMap", "_MainTex");

        Color color = GetColor(
            material, "_BaseColor", "_Color");

        Texture normal = GetTexture(
            material, "_BumpMap");

        Texture emission = GetTexture(
            material, "_EmissionMap");

        Color emissionColor = GetColor(
            material, "_EmissionColor");

        float metallic = GetFloat(
            material, "_Metallic", 0f);

        float smoothness = GetFloat(
            material, "_Smoothness",
            GetFloat(material, "_Glossiness", 0.5f));

        float bumpScale = GetFloat(
            material, "_BumpScale", 1f);

        // Fallback : texture provenant de la description.
        TexturePropertyDescription diffuse;

        if (albedo == null &&
            description.TryGetProperty(
                "DiffuseColor", out diffuse))
        {
            albedo = diffuse.texture;
        }

        // Appliquer le shader sélectionné.
        material.shader = shader;

        SetTexture(material, "_BaseMap", albedo);
        SetTexture(material, "_MainTex", albedo);

        SetColor(material, "_BaseColor", color);
        SetColor(material, "_Color", color);

        SetFloat(material, "_Metallic", metallic);
        SetFloat(material, "_Smoothness", smoothness);
        SetFloat(material, "_Glossiness", smoothness);

        if (normal != null)
        {
            SetTexture(material, "_BumpMap", normal);
            SetFloat(material, "_BumpScale", bumpScale);

            if (material.HasProperty("_BumpMap"))
                material.EnableKeyword("_NORMALMAP");
        }

        if (emission != null)
        {
            SetTexture(material, "_EmissionMap", emission);
            SetColor(material, "_EmissionColor", emissionColor);

            material.EnableKeyword("_EMISSION");
        }

        if (settings.VerboseLogging)
        {
            Debug.Log(
                $"[Material Import] {assetPath}\n" +
                $"{material.name} -> {shader.name}");
        }
    }

    private static Texture GetTexture(
        Material mat, params string[] names)
    {
        foreach (string name in names)
        {
            if (mat.HasProperty(name))
            {
                Texture texture = mat.GetTexture(name);
                if (texture != null)
                    return texture;
            }
        }

        return null;
    }

    private static Color GetColor(
        Material mat, params string[] names)
    {
        foreach (string name in names)
        {
            if (mat.HasProperty(name))
                return mat.GetColor(name);
        }

        return Color.white;
    }

    private static float GetFloat(
        Material mat, string name, float fallback)
    {
        return mat.HasProperty(name)
            ? mat.GetFloat(name)
            : fallback;
    }

    private static void SetTexture(
        Material mat, string name, Texture value)
    {
        if (value != null && mat.HasProperty(name))
            mat.SetTexture(name, value);
    }

    private static void SetColor(
        Material mat, string name, Color value)
    {
        if (mat.HasProperty(name))
            mat.SetColor(name, value);
    }

    private static void SetFloat(
        Material mat, string name, float value)
    {
        if (mat.HasProperty(name))
            mat.SetFloat(name, value);
    }
}
