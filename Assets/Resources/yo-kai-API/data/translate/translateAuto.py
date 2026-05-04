import os
import json

def translate(output_filename, input_key, output_key):
    source_dir = r"C:\Users\pacau\Desktop\MES PROJET\PROJET\API\yo-kai-API\data\yokai_data"
    translation_map = {}

    if not os.path.exists(source_dir):
        print(f"Erreur : Le dossier {source_dir} est introuvable.")
        return

    for filename in os.listdir(source_dir):
        if filename.endswith(".json"):
            file_path = os.path.join(source_dir, filename)
            
            try:
                with open(file_path, 'r', encoding='utf-8') as f:
                    data = json.load(f)
                    
                    if input_key in data and output_key in data:
                        cle_primaire = data[input_key]
                        valeur_traduction = data[output_key]
                        
                        if cle_primaire not in translation_map:
                            translation_map[cle_primaire] = valeur_traduction
            
            except (json.JSONDecodeError, IOError) as e:
                print(f"Erreur lors de la lecture du fichier {filename} : {e}")

    script_dir = os.path.dirname(os.path.abspath(__file__))
    output_path = os.path.join(script_dir, output_filename)

    try:
        with open(output_path, 'w', encoding='utf-8') as f_out:
            json.dump(translation_map, f_out, ensure_ascii=False, indent=4)
        print(f"Succès : Le fichier '{output_filename}' a été créé avec {len(translation_map)} entrées.")
    except IOError as e:
        print(f"Erreur lors de la création du fichier de sortie : {e}")



if __name__ == "__main__":
    # FR
    translate("translateEnToFr.json", "nom_anglais", "Nom")
    translate("translateJpToFr.json", "nom_original", "Nom")

    # EN
    translate("translateFrToEn.json", "Nom", "nom_anglais")
    translate("translateJpToEn.json", "nom_original", "nom_anglais")

    # JP
    translate("translateEnToJp.json", "nom_anglais", "nom_original")
    translate("translateFrToJp.json", "Nom", "nom_original")