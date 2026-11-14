
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public sealed class UnityIconExporter :
    IPostprocessBuildWithReport
{
    public int callbackOrder => 100;

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform !=
            BuildTarget.StandaloneLinux64)
            return;

        // Fetch configured Desktop application icons.
        var icons = PlayerSettings.GetIcons(
            NamedBuildTarget.Standalone,
            IconKind.Application
        );

        // Prefer the largest available icon.
        var icon = icons
            .Where(x => x != null)
            .OrderByDescending(x => x.width * x.height)
            .FirstOrDefault();

        if (icon == null)
            throw new BuildFailedException(
                "No Desktop application icon configured " +
                "in Unity Player Settings."
            );

        string assetPath = AssetDatabase.GetAssetPath(icon);

        // Directly reuse the source asset.
        // A PNG source avoids GPU rendering in headless CI.
        if (!string.Equals(
            Path.GetExtension(assetPath),
            ".png",
            StringComparison.OrdinalIgnoreCase))
        {
            throw new BuildFailedException(
                "The Desktop icon source must be a PNG: " +
                assetPath
            );
        }

        string projectRoot = Directory.GetParent(
            Application.dataPath
        ).FullName;

        string source = Path.Combine(
            projectRoot, assetPath
        );

        string destination = Path.Combine(
            projectRoot, "Build", "ci-icon.png"
        );

        Directory.CreateDirectory(
            Path.GetDirectoryName(destination)
        );

        File.Copy(source, destination, true);

        Debug.Log(
            $"[CI] Exported Unity icon to {destination}"
        );
    }
}
