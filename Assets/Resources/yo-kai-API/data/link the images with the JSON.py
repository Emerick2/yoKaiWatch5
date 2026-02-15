import json
import os

output_dir = 'yokai_data'
lien = 'https://emerick2.github.io/yo-kai-API/data/'

if os.path.exists(output_dir):
    for filename in os.listdir(output_dir):
        if filename.endswith('.json'):
            file_path = os.path.join(output_dir, filename)
            
            with open(file_path, 'r', encoding='utf-8') as f:
                try:
                    data = json.load(f)
                except json.JSONDecodeError:
                    print(f"Erreur de lecture sur le fichier {filename}")
                    continue

            yokai_id = data.get('ID')
            data['image_url'] = f"{lien}picture/{yokai_id}.png"
            
            if 'Rang_E' in data:
                data['E'] = data.pop('Rang_E')
            if 'Rang_D' in data:
                data['D'] = data.pop('Rang_D')
            if 'Rang_C' in data:
                data['C'] = data.pop('Rang_C')
            if 'Rang_B' in data:
                data['B'] = data.pop('Rang_B')
            if 'Rang_A' in data:
                data['A'] = data.pop('Rang_A')
            if 'Rang_S' in data:
                data['S'] = data.pop('Rang_S')

            with open(file_path, 'w', encoding='utf-8') as f:
                json.dump(data, f, indent=4, ensure_ascii=False)
            
            print(f"Fichier {filename} mis à jour.")
else:
    print(f"Le dossier {output_dir} n'existe pas.")