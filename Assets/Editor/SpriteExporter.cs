using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteExporter {
    [MenuItem("Outils/Exporter tous les sprites")]
    public static void ExporterSprites() {
        string path = "Assets/Resources/Sprites/MonSpriteSheet.png";
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

        if (texture == null) {
            Debug.LogError("Texture non trouvée !");
            return;
        }

        string dossierExport = "Assets/Sprites_Exportés";
        Directory.CreateDirectory(dossierExport);

        Sprite[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(path)
                                        as Sprite[];

        int compteur = 0;

        foreach (var obj in AssetDatabase.LoadAllAssetsAtPath(path)) {
            if (obj is Sprite sprite) {
                Texture2D nouvelleImage = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                Color[] pixels = texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height);
                nouvelleImage.SetPixels(pixels);
                nouvelleImage.Apply();

                byte[] png = nouvelleImage.EncodeToPNG();
                File.WriteAllBytes($"{dossierExport}/{sprite.name}_{compteur}.png", png);
                compteur++;
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"✅ {compteur} sprites exportés dans {dossierExport}");
    }
}
