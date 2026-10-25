using UnityEditor;
using UnityEngine;

[FilePath(
    "ProjectSettings/ImportedMaterialSettings.asset",
    FilePathAttribute.Location.ProjectFolder)]
public class ImportedMaterialSettings :
    ScriptableSingleton<ImportedMaterialSettings>
{
    [SerializeField]
    private bool enabled = true;

    [SerializeField]
    private string shaderName =
        "Universal Render Pipeline/Lit";

    [SerializeField]
    private bool overrideAllShaders = false;

    [SerializeField]
    private bool verboseLogging = false;

    public bool Enabled => enabled;
    public string ShaderName => shaderName;
    public bool OverrideAllShaders => overrideAllShaders;
    public bool VerboseLogging => verboseLogging;

    public Shader GetShader()
    {
        return Shader.Find(shaderName);
    }

    public void SetEnabled(bool value)
    {
        enabled = value;
        Save(true);
    }

    public void SetShader(Shader shader)
    {
        if (shader == null)
            return;

        shaderName = shader.name;
        Save(true);
    }

    public void SetOverrideAllShaders(bool value)
    {
        overrideAllShaders = value;
        Save(true);
    }

    public void SetVerboseLogging(bool value)
    {
        verboseLogging = value;
        Save(true);
    }
}