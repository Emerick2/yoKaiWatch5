import csv
import json
import os

input_file = 'Médalium.csv'
output_dir = 'yokai_data'

if not os.path.exists(output_dir):
    os.makedirs(output_dir)

def convert_csv_to_json():
    try:
        with open(input_file, mode='r', encoding='cp1252') as csv_file:
            reader = csv.DictReader(csv_file, delimiter=';')
            
            for row in reader:
                if not row.get('ID'):
                    continue

                yokai_id = row['ID']
                
                yokai_data = {k.strip(): v.strip() for k, v in row.items() if k}
                
                file_path = os.path.join(output_dir, f"{yokai_id}.json")
                
                with open(file_path, 'w', encoding='utf-8') as json_file:
                    json.dump(yokai_data, json_file, indent=4, ensure_ascii=False)
                
                print(f"Fichier {yokai_id}.json créé.")

    except FileNotFoundError:
        print(f"Erreur : Le fichier {input_file} est introuvable.")
    except Exception as e:
        print(f"Une erreur est survenue : {e}")

if __name__ == "__main__":
    convert_csv_to_json()