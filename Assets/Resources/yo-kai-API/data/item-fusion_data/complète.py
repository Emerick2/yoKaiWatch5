import json
from pathlib import Path

# Configuration
BASE_URL = "https://raw.githubusercontent.com/Emerick2/yo-kai-API/refs/heads/main/img/item-img/item-fusion/"
# Remplace '.' par le chemin de ton dossier contenant les JSON (ex: 'data/items')
FOLDER_PATH = Path('.') 

# Ton dictionnaire de correspondance
MAPPINGS = {
    "Moule à takoyaki": "takoyaki_mold.webp",
    "Sable des dunes": "dune_sand.webp",
    "Statue terre cuite": "terracotta_figure.webp",
    "Hibiscus rouge": "red_hibiscus.webp",
    "Âme tourbill.": "swirling_soul.webp",
    "Clé de voûte": "keystone.webp",
    "Placard confortable": "comfortable_closet.webp",
    "Perle périlleuse": "perilous_pearl.webp",
    "Céramiq. ancienne": "antique_ceramics.webp",
    "Boule barbare": "barbarion_ball.webp",
    "Huile bain parfumée": "scented_bath_oil.webp",
    "Mauvaise nouvelle": "letter_of_postponed.webp",
    "ADN de dinosaure": "adn_de_dinosaure.webp",
    "Essence maléfique": "essence_malefique.webp",
    "Croix ankh dorée": "golden_ankh_cross.webp",
    "Sabre déchaîné": "unleashed_sabre.webp",
    "Costume sable": "sand_suit.webp",
    "Eau d'outre-monde": "spiritual_water.webp",
    "Amnéspirateur": "amnespirateur.webp"
}

def update_json_files():
    count = 0
    # Parcourir tous les fichiers .json du dossier
    for file_path in FOLDER_PATH.glob('*.json'):
        try:
            with open(file_path, 'r', encoding='utf-8') as f:
                data = json.load(f)

            # Vérifier si la clé "name" existe et est dans notre dictionnaire
            name_val = data.get("name")
            if name_val in MAPPINGS:
                # Mise à jour de l'URL
                new_url = f"{BASE_URL}{MAPPINGS[name_val]}"
                data["imageUrl"] = new_url
                
                # Sauvegarde du fichier modifié
                with open(file_path, 'w', encoding='utf-8') as f:
                    json.dump(data, f, indent=4, ensure_ascii=False)
                
                print(f"✅ Mis à jour : {file_path.name} ({name_val})")
                count += 1
        
        except Exception as e:
            print(f"❌ Erreur sur le fichier {file_path.name}: {e}")

    print(f"\nTerminé ! {count} fichiers ont été mis à jour.")

if __name__ == "__main__":
    update_json_files()