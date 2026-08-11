import os
import re

dossier = "."

fichiers = [f for f in os.listdir(dossier) if f.endswith('.json')]

fichiers_numerotes = []
for f in fichiers:
    match = re.search(r'\d+', f)
    if match:
        fichiers_numerotes.append((int(match.group()), f))

fichiers_numerotes.sort(key=lambda x: x[0])

if fichiers_numerotes:
    ancien_debut = fichiers_numerotes[0][0]
    nouveau_debut = 904
    decalage = nouveau_debut - ancien_debut

    for ancien_num, nom_origine in fichiers_numerotes:
        nouveau_num = ancien_num + decalage
        nouveau_nom = re.sub(r'\d+', str(nouveau_num), nom_origine)
        
        chemin_origine = os.path.join(dossier, nom_origine)
        chemin_nouveau = os.path.join(dossier, nouveau_nom)
        
        os.rename(chemin_origine, chemin_nouveau)