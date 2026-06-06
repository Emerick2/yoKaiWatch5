import os
import json
import re
import unicodedata

def make_an_url(valeur):
    """Normalise et slugifie le nom pour l'URL."""
    valeur = unicodedata.normalize('NFD', str(valeur))
    valeur = re.sub(r'[\u0300-\u036f]', '', valeur)
    valeur = valeur.lower().strip()
    valeur = re.sub(r'\s+', '-', valeur)
    valeur = re.sub(r'[^\w\-]+', '', valeur)
    valeur = re.sub(r'--+', '-', valeur)
    return valeur

def get_new_url(old_url, name):
    """Remplace la fin de l'URL par le nom formaté."""
    # On récupère tout ce qui précède le dernier slash
    base_path = old_url.rsplit('/', 1)[0]
    slug = make_an_url(name)
    return f"{base_path}/{slug}.png"

def update_json_files():
    # Définir le dossier de travail sur celui du script
    folder_path = os.path.dirname(os.path.abspath(__file__))
    
    for filename in os.listdir(folder_path):
        if filename.endswith(".json"):
            file_path = os.path.join(folder_path, filename)
            
            with open(file_path, 'r', encoding='utf-8') as f:
                try:
                    data = json.load(f)
                except json.JSONDecodeError:
                    continue

            modified = False

            # --- CAS 1 : Item individuel (ex: Papillon paon) ---
            if isinstance(data, dict) and "name" in data and "imageUrl" in data:
                data["imageUrl"] = get_new_url(data["imageUrl"], data["name"].replace(" ★","_"))
                modified = True

            # --- CAS 2 : L'Annuaire (ex: "Cigale verte": "URL") ---
            elif isinstance(data, dict):
                # On détecte si c'est l'annuaire en vérifiant si les valeurs sont des URLs
                is_annuaire = any(isinstance(v, str) and v.startswith("http") for v in data.values())
                
                if is_annuaire:
                    new_annuaire = {}
                    for name, url in data.items():
                        if isinstance(url, str) and url.startswith("http"):
                            new_annuaire[name] = get_new_url(url, name)
                        else:
                            new_annuaire[name] = url
                    data = new_annuaire
                    modified = True

            # Sauvegarde du fichier si modification
            if modified:
                with open(file_path, 'w', encoding='utf-8') as f:
                    json.dump(data, f, indent=4, ensure_ascii=False)
                print(f"✅ Mis à jour : {filename}")

if __name__ == "__main__":
    update_json_files()