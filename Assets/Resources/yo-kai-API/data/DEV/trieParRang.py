import json
import os
import sys
import re
import unicodedata

# --- CONFIGURATION LA REQUÊTE ---
listeNouritureFavorite = ["Riz", "Pizza", "Tempura", "Pain", "Ramen", "Chocolat", "Hot dog", "Sukiyaki", "Légumes", "Viande", "Bonbons", "Cuisine chinoise", "Douceurs", "Hamburger", "Jus", "Soba", "Oden", "Curry", "Donut", "Fruits de Mer", "Encas", "Super méga barre", "Lait", "Pâtes", "Sushi", "Crème Glacée"]
listeNomTribue = ["Mignon", "Mystérieux", "Bienveillant", "Costaud", "Vaillant", "Insaisissable", "Sombre", "Sinistre"]
listeRang = ["S", "A", "B", "C", "D", "E"]

def make_an_url(valeur):
    valeur = unicodedata.normalize('NFD', str(valeur))
    valeur = re.sub(r'[\u0300-\u036f]', '', valeur)
    valeur = valeur.lower().strip()
    valeur = re.sub(r'\s+', '-', valeur)
    valeur = re.sub(r'[^\w\-]+', '', valeur)
    valeur = re.sub(r'--+', '-', valeur)
    return valeur


def Ecrire(message):
    """Affiche un message sur une seule ligne en effaçant la précédente."""
    sys.stdout.write(f"\r{message}")
    sys.stdout.flush()

def filtrer_yokai(dossier_source, fichier_destination, cle_critere, valeur_attendue):
    resultats = []
    
    fichiers = [f for f in os.listdir(dossier_source) if f.endswith('.json')]
    total = len(fichiers)

    for index, nom_fichier in enumerate(fichiers, 1):
        Ecrire(f"Progression : {index}/{total} (Traitement de {nom_fichier})...")
        
        chemin_complet = os.path.join(dossier_source, nom_fichier)
        try:
            with open(chemin_complet, 'r', encoding='utf-8') as f:
                donnees = json.load(f)
                
                if donnees.get(cle_critere) == valeur_attendue:
                    identifiant = donnees.get("ID")
                    if identifiant:
                        resultats.append(identifiant)
        except Exception:
            pass


    with open(fichier_destination, 'w', encoding='utf-8') as f_dest:
        json.dump(resultats, f_dest, indent=4, ensure_ascii=False)
    
    print(f"Total : {len(resultats)} IDs de Yo-kai (Rang {valeur_attendue}) enregistrés dans {fichier_destination}.")


#C:/Users/pacau/Desktop/MES PROJET/PROJET/API/yo-kai-API/data/DED/

def La_MainTrieCategorie():
    DOSSIER_DATA = "C:/Users/pacau/Desktop/MES PROJET/PROJET/API/yo-kai-API/data/yokai_data"
    DOSSIER_CIBLE = "C:/Users/pacau/Desktop/MES PROJET/PROJET/API/yo-kai-API/data/DED/"
    if not os.path.exists(DOSSIER_CIBLE):
        os.makedirs(DOSSIER_CIBLE)

    for i in range(0,len(listeNomTribue)):
        NOM_FICHIER_CIBLE = "yo-kai-tribue-"+listeNomTribue[i]+".json"
        FICHIER_DESTINATION = os.path.join(DOSSIER_CIBLE, NOM_FICHIER_CIBLE)
        CRITERE = "Tribue"
        VALEUR = listeNomTribue[i]
        filtrer_yokai(DOSSIER_DATA, FICHIER_DESTINATION, CRITERE, VALEUR)

    for i in range(0,len(listeRang)):
        NOM_FICHIER_CIBLE = "yo-kai-rang-"+listeRang[i]+".json"
        FICHIER_DESTINATION = os.path.join(DOSSIER_CIBLE, NOM_FICHIER_CIBLE)
        CRITERE = "Rang"
        VALEUR = listeRang[i]
        filtrer_yokai(DOSSIER_DATA, NOM_FICHIER_CIBLE, CRITERE, VALEUR)

    for i in range(0,len(listeNouritureFavorite)):
        NOM_FICHIER_CIBLE = "yo-kai-food-"+make_an_url(listeNouritureFavorite[i])+".json"
        FICHIER_DESTINATION = os.path.join(DOSSIER_CIBLE, NOM_FICHIER_CIBLE)
        CRITERE = "nourriture"
        VALEUR = listeNouritureFavorite[i]
        filtrer_yokai(DOSSIER_DATA, NOM_FICHIER_CIBLE, CRITERE, VALEUR)


if __name__ == "__main__":
    La_MainTrieCategorie()