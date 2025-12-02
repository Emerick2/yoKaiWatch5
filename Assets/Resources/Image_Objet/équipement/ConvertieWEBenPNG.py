import os
from PIL import Image

def convertir_webp_en_png():
    """
    Parcourt le dossier actuel, trouve tous les fichiers .webp
    et les convertit en fichiers .png.
    """
    
    # 🖼️ Configuration
    # Le chemin du dossier est le dossier où se trouve le script
    dossier_actuel = os.path.dirname(os.path.abspath(__file__))
    
    print(f"Démarrage de la conversion dans le dossier : {dossier_actuel}")
    print("-" * 30)

    # Compteurs pour le résumé
    fichiers_convertis = 0
    fichiers_echoues = 0

    # 🔎 Parcours de tous les fichiers dans le dossier
    for nom_fichier in os.listdir(dossier_actuel):
        # Vérifie si le fichier se termine par .webp (insensible à la casse)
        if nom_fichier.lower().endswith(".webp"):
            
            chemin_entree = os.path.join(dossier_actuel, nom_fichier)
            
            # ⚙️ Détermine le nom du fichier de sortie
            # Remplace l'extension .webp (ou .WEBP) par .png
            nom_base = os.path.splitext(nom_fichier)[0]
            chemin_sortie = os.path.join(dossier_actuel, f"{nom_base}.png")

            print(f"Tentative de conversion de : **{nom_fichier}**...")

            try:
                # Ouvre le fichier webp
                image = Image.open(chemin_entree)
                
                # Sauvegarde l'image au format PNG
                # Le format est inféré par l'extension .png
                image.save(chemin_sortie)
                
                print(f"   -> Conversion réussie : **{nom_base}.png**")
                fichiers_convertis += 1
                
            except Exception as e:
                print(f"   -> ⚠️ **Échec de la conversion** pour {nom_fichier}. Erreur: {e}")
                fichiers_echoues += 1
    
    # --- Résumé ---
    print("-" * 30)
    print("✨ **Conversion terminée !**")
    print(f"Nombre de fichiers convertis : **{fichiers_convertis}**")
    print(f"Nombre d'échecs : **{fichiers_echoues}**")


if __name__ == "__main__":
    convertir_webp_en_png()