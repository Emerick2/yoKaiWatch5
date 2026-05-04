import csv
import json
import os

def AjouterDansJSON(fichier, clef, valeur):
    donnees = []
    if os.path.exists(fichier):
        with open(fichier, 'r', encoding='utf-8') as f:
            try:
                donnees = json.load(f)
            except json.JSONDecodeError:
                donnees = []

    donnees.append({clef: valeur})

    with open(fichier, 'w', encoding='utf-8') as f:
        json.dump(donnees, f, indent=4, ensure_ascii=False)

with open('phrase yo-criminel.csv', 'r', encoding='utf-8') as csvfile:
    reader = csv.reader(csvfile)
    
    for ligne in reader:
        if len(ligne) > 3:
            print(f"Ajout du gain : {ligne[3]}")
            AjouterDansJSON("test.json", "gain", ligne[3])