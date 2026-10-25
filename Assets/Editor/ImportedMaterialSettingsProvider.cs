
using UnityEditor;
using UnityEngine;

public static class ImportedMaterialSettingsProvider
{
    [SettingsProvider]
    public static SettingsProvider CreateProvider()
    {
        return new SettingsProvider(
            "Project/Imported Materials",
            SettingsScope.Project)
        {
            label = "Imported Materials",
            guiHandler = DrawSettings,
            keywords = new System.Collections.Generic.HashSet<string>
            {
                "Shader", "URP", "Material", "Import",
                "FBX", "DAE", "OBJ"
            }
        };
    }

    private static void DrawSettings(string searchContext)
    {
        var settings = ImportedMaterialSettings.instance;

        EditorGUILayout.Space(12);

        EditorGUILayout.LabelField(
            "Automatic Material Import",
            EditorStyles.boldLabel);

        EditorGUILayout.HelpBox(
            "Automatically assigns a selected shader to " +
            "materials created by Unity's model importer.",
            MessageType.Info);

        EditorGUI.BeginChangeCheck();

        bool enabled = EditorGUILayout.Toggle(
            "Enable Auto Conversion",
            settings.Enabled);

        if (EditorGUI.EndChangeCheck())
            settings.SetEnabled(enabled);

        using (new EditorGUI.DisabledScope(!settings.Enabled))
        {
            Shader current = settings.GetShader();

            EditorGUI.BeginChangeCheck();

            Shader selected = (Shader)EditorGUILayout.ObjectField(
                "Default Shader",
                current,
                typeof(Shader),
                false);

            if (EditorGUI.EndChangeCheck() && selected != null)
                settings.SetShader(selected);

            EditorGUI.BeginChangeCheck();

            bool overrideAll = EditorGUILayout.Toggle(
                "Override All Shaders",
                settings.OverrideAllShaders);

            if (EditorGUI.EndChangeCheck())
                settings.SetOverrideAllShaders(overrideAll);

            EditorGUI.BeginChangeCheck();

            bool verbose = EditorGUILayout.Toggle(
                "Verbose Logging",
                settings.VerboseLogging);

            if (EditorGUI.EndChangeCheck())
                settings.SetVerboseLogging(verbose);
        }

        EditorGUILayout.Space(12);

        if (settings.GetShader() == null)
        {
            EditorGUILayout.HelpBox(
                "The configured shader was not found. " +
                "Check that its package is installed.",
                MessageType.Warning);
        }

        EditorGUILayout.HelpBox(
            "These settings apply to future model imports " +
            "and reimports. Existing models are not " +
            "automatically reimported.",
            MessageType.None);
    }
}
