import os
import shutil
import hashlib
import re
import json

# === CONFIGURATION ===
# On ajoute un 'r' devant les guillemets pour éviter l'erreur Unicode
start = r"C:\Users\pacau\Desktop\MES PROJET\yo-kai-API\yo-kai-API\data"

# Utiliser os.path.join est encore plus propre pour éviter les problèmes de slashs
SOURCE_DIR = os.path.join(start, "3D")

# Chemins vers les nouveaux dossiers de sortie
OUTPUT_MODELS_DIR = os.path.join(start, "_3D", "models")
OUTPUT_TEXTURES_DIR = os.path.join(start, "_3D", "textures")
OUTPUT_JSON_MAPPING = os.path.join(start, "_3D", "yokai_assets_mapping.json")
# =====================

# Regex pour trouver l'ID dans le nom du fichier.
# Cherche 'y' suivi de chiffres. Ex: y104000 -> ID 104000
ID_PATTERN = re.compile(r"y(\d+)")

def calculate_file_hash(filepath):
    """Calcule le hash MD5 d'un fichier pour vérifier s'il est unique."""
    hasher = hashlib.md5()
    with open(filepath, 'rb') as f:
        buf = f.read(65536)
        while len(buf) > 0:
            hasher.update(buf)
            buf = f.read(65536)
    return hasher.hexdigest()

def ensure_dir(directory):
    if not os.path.exists(directory):
        os.makedirs(directory)
        print(f"Dossier créé : {directory}")

def main():
    ensure_dir(OUTPUT_MODELS_DIR)
    ensure_dir(OUTPUT_TEXTURES_DIR)

    # Dictionnaires pour suivre les fichiers uniques (Hash -> Nom du fichier final)
    unique_model_hashes = {}
    unique_texture_hashes = {}

    # Dictionnaire pour stocker le résultat final qui ira dans le JSON
    # Structure : { "ID_YOKAI": { "model": "nom.fbx", "textures": ["t1.png", "t2.png"] } }
    yokai_mapping = {}

    print("Début de l'analyse des fichiers...")

    # 1. Regrouper les fichiers par ID Yo-kai
    files_by_id = {}
    for filename in os.listdir(SOURCE_DIR):
        match = ID_PATTERN.search(filename)
        if match:
            yokai_id = match.group(1) # On récupère juste les chiffres (ex: 104000)
            if yokai_id not in files_by_id:
                files_by_id[yokai_id] = []
            files_by_id[yokai_id].append(os.path.join(SOURCE_DIR, filename))
        else:
            print(f"Attention : Impossible de trouver un ID dans {filename}, fichier ignoré.")

    print(f"Trouvé {len(files_by_id)} identifiants de Yo-kai différents.")
    print("Début du traitement et de la déduplication...\n")

    # 2. Traiter chaque groupe
    for yokai_id, files in files_by_id.items():
        current_model_final_name = None
        current_textures_final_names = []

        for filepath in files:
            filename = os.path.basename(filepath)
            file_hash = calculate_file_hash(filepath)

            # --- Traitement des MODÈLES (.fbx) ---
            if filename.lower().endswith('.fbx'):
                if file_hash in unique_model_hashes:
                    # Le modèle existe déjà, on réutilise son nom
                    current_model_final_name = unique_model_hashes[file_hash]
                    # print(f"  [Modèle doublon] ID {yokai_id} utilise le modèle existant {current_model_final_name}")
                else:
                    # Nouveau modèle unique trouvé
                    # On le renomme en utilisant le premier ID qui l'utilise pour que ce soit propre
                    new_model_name = f"model_y{yokai_id}.fbx"
                    target_path = os.path.join(OUTPUT_MODELS_DIR, new_model_name)
                    shutil.copy2(filepath, target_path)
                    
                    unique_model_hashes[file_hash] = new_model_name
                    current_model_final_name = new_model_name
                    print(f"  [Nouveau Modèle] {new_model_name} créé (Hash: {file_hash[:8]}...)")

            # --- Traitement des TEXTURES (.png) ---
            elif filename.lower().endswith('.png'):
                # Pour les textures, c'est plus simple de garder le nom d'origine
                # car il contient souvent des infos utiles (_01, _02)
                # On vérifie quand même les doublons exacts.
                if file_hash not in unique_texture_hashes:
                     target_path = os.path.join(OUTPUT_TEXTURES_DIR, filename)
                     # Attention : si deux textures différentes ont le même nom par erreur, ceci écrasera.
                     # Dans votre cas, les noms semblent uniques grâce à l'ID dedans.
                     shutil.copy2(filepath, target_path)
                     unique_texture_hashes[file_hash] = filename
                
                current_textures_final_names.append(filename)

        # 3. Sauvegarder le mapping pour cet ID
        if current_model_final_name:
             # On enlève les doublons de textures s'il y en a pour un même modèle
            unique_textures_list = sorted(list(set(current_textures_final_names)))
            
            yokai_mapping[yokai_id] = {
                "modelFile": current_model_final_name,
                "textureFiles": unique_textures_list
            }

    # 4. Exporter le résultat JSON
    print(f"\nTerminé. Écriture du fichier de mapping : {OUTPUT_JSON_MAPPING}")
    with open(OUTPUT_JSON_MAPPING, 'w', encoding='utf-8') as f:
        json.dump(yokai_mapping, f, indent=2, ensure_ascii=False)

    print("\n--- Résumé ---")
    print(f"Modèles uniques conservés : {len(unique_model_hashes)}")
    print(f"Textures uniques conservées : {len(unique_texture_hashes)}")
    print(f"Mapping généré pour {len(yokai_mapping)} Yo-kai.")
    print("Vérifiez le dossier 'output'.")

if __name__ == "__main__":
    main()