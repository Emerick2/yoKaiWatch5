import os
import random
import time
import pygame

def jouer_musique_aleatoire():
    # 1. Initialiser le mixer de pygame (nécessaire pour le son)
    pygame.mixer.init()
    
    # 2. Obtenir le dossier où se trouve ce script
    dossier_courant = os.path.dirname(os.path.abspath(__file__))
    
    # 3. Trouver tous les fichiers .mp3 dans ce dossier
    chansons = [f for f in os.listdir(dossier_courant) if f.lower().endswith('.mp3')]
    
    # Vérification de sécurité
    if not chansons:
        print("Aucun fichier .mp3 trouvé dans ce dossier !")
        return

    print(f"{len(chansons)} chansons trouvées. Lancement du lecteur (Ctrl+C pour arrêter)...")
    print("-" * 30)

    try:
        while True:
            # 4. Choisir une chanson au hasard
            chanson_choisie = random.choice(chansons)
            chemin_complet = os.path.join(dossier_courant, chanson_choisie)
            
            print(f"Lecture de : {chanson_choisie}")
            
            # 5. Charger et jouer la musique
            try:
                pygame.mixer.music.load(chemin_complet)
                pygame.mixer.music.play()
                
                # 6. Boucle d'attente : tant que la musique joue, on attend
                while pygame.mixer.music.get_busy():
                    time.sleep(1) # Pause d'une seconde pour ne pas surcharger le processeur
                    
            except pygame.error as e:
                print(f"Erreur lors de la lecture de {chanson_choisie}: {e}")

    except KeyboardInterrupt:
        # Permet d'arrêter proprement avec Ctrl+C
        print("\nArrêt du lecteur. À bientôt !")
        pygame.mixer.music.stop()

if __name__ == "__main__":
    jouer_musique_aleatoire()