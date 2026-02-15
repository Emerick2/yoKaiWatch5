import json
import os

script_dir = os.path.dirname(os.path.abspath(__file__))
# Le JSON est dans le même dossier que le script
json_path = os.path.join(script_dir, 'yokai_assets_mapping.json')
erreur = False

try:
    with open(json_path, 'r', encoding='utf-8') as f:
        mapping_data = json.load(f)
except (FileNotFoundError, json.JSONDecodeError) as e:
    print(f"Erreur: Impossible de lire le fichier {json_path}. ({e})")
    erreur = True

while not erreur:
    id_cible = input("Entrez l'ID du modèle à modifier (ou 'q' pour quitter): ")
    if id_cible.lower() == 'q':
        break

    id_source = input(f"Entrez l'ID du modèle à utiliser comme source pour '{id_cible}': ")
    if id_source.lower() == 'q':
        break

    if id_cible not in mapping_data:
        print(f"Erreur: L'ID cible '{id_cible}' n'a pas été trouvé dans {json_path}.")
        continue
    
    if id_source not in mapping_data:
        print(f"Erreur: L'ID source '{id_source}' n'a pas été trouvé dans {json_path}.")
        continue

    # confirm = input(f"\nConfirmez-vous cette opération ? (o/n): ").lower()
    # if confirm != 'o':
    #     print("Opération annulée.")
    #     continue
    
    # Sauvegarde de l'ancien nom de fichier modèle pour une suppression éventuelle
    old_model_file = mapping_data[id_cible].get("modelFile")

    # Remplacement des données
    if "modelFile" in mapping_data[id_source]:
        mapping_data[id_cible]["modelFile"] = mapping_data[id_source]["modelFile"]
    else:
        # Si la source n'a pas de modelFile, on peut choisir de le supprimer de la cible ou de ne rien faire
        if "modelFile" in mapping_data[id_cible]:
            del mapping_data[id_cible]["modelFile"]

    try:
        with open(json_path, 'w', encoding='utf-8') as f:
            json.dump(mapping_data, f, indent=2)
        print(f"Succès : Le 'modelFile' de l'ID '{id_cible}' a été mis à jour avec celui de '{id_source}'.")

        # Suppression de l'ancien modèle s'il n'est plus utilisé
        if old_model_file:
            is_used = any(entry.get("modelFile") == old_model_file for entry in mapping_data.values())
            if not is_used:
                models_dir = os.path.join(script_dir, 'models')
                file_to_delete = os.path.join(models_dir, old_model_file)
                if os.path.exists(file_to_delete):
                    os.remove(file_to_delete)
                    print(f"Fichier modèle non utilisé '{old_model_file}' a été supprimé.")
                else:
                    print(f"Avertissement: L'ancien fichier modèle '{old_model_file}' n'a pas été trouvé pour suppression.")

    except IOError as e:
        print(f"Erreur lors de l'écriture ou de la suppression du fichier : {e}")


    print("\n--- Récapitulatif ---")
    print(f"Vous avez remplacer les données de '{id_cible}':")
    print(json.dumps(mapping_data[id_cible], indent=2))
    print(f"Par les données de '{id_source}':")
    print(json.dumps(mapping_data[id_source], indent=2))
    

    print("-" * 30)

# Objectif : demande à l'utilisateur le numéro de corp à suprimer et celui à mettre à la place
# Suprimé l'ancien corp et mettre à jours le JSON.