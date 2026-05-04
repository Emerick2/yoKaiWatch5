import os
import json

# --- CONFIGURATION ---
# Dossier où se trouvent tes fichiers JSON
JSON_DIR = '.' 

def patch_missing_names():
    updated_count = 0
    warning_count = 0

    # On liste tous les fichiers .json qui sont nommés par un nombre
    files = [f for f in os.listdir(JSON_DIR) if f.endswith('.json') and f[:-5].isdigit()]
    
    for filename in files:
        file_path = os.path.join(JSON_DIR, filename)
        
        with open(file_path, 'r', encoding='utf-8') as f:
            try:
                data = json.load(f)
            except json.JSONDecodeError:
                print(f"Erreur de lecture : {filename}")
                continue

        # Récupération des valeurs (on utilise .get pour éviter les erreurs si la clé n'existe pas)
        nom_fr = data.get("Nom", "").strip()
        nom_en = data.get("nom_anglais", "").strip()

        # LOGIQUE :
        # Si le nom FR est vide
        if not nom_fr:
            # On vérifie si on a un nom anglais pour compenser
            if nom_en:
                data["Nom"] = nom_en
                # On sauvegarde la modification
                with open(file_path, 'w', encoding='utf-8') as f:
                    json.dump(data, f, ensure_ascii=False, indent=4)
                
                print(f"[{filename}] Nom FR manquant -> Remplacé par le nom anglais : '{nom_en}'")
                updated_count += 1
            else:
                # Si les deux sont vides, on alerte
                print(f"⚠️  ALERTE : Le fichier {filename} n'a ni 'Nom' (FR) ni 'nom_anglais' !")
                warning_count += 1

    print(f"\n--- Terminé ---")
    print(f"Fichiers mis à jour : {updated_count}")
    print(f"Fichiers toujours sans nom : {warning_count}")

if __name__ == "__main__":
    patch_missing_names()