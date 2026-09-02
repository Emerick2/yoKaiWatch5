# Architecture du projet

La documentation de référence est [Moteur.md](Moteur.md). Cette page conserve
une vue synthétique de l’architecture; le combat n’y est pas décrit car il est
en cours de modification.

## Vue d'ensemble

Yo-kai Watch 5 est un projet Unity 3D construit avec Unity 2022.3.54f1. Il
utilise UGUI/TextMesh Pro, AI Navigation, Cinemachine, Netcode for GameObjects
et Newtonsoft Json. Le code n'est actuellement pas découpé en assembly
definitions ni en espaces de noms : les scripts partagent donc l'assembly Unity
par défaut et de nombreuses dépendances sont implicites.

```text
Menu principal
  -> création/chargement d'un journal
  -> scènes de carte
       -> dialogue / quête / inventaire / médalium / mini-jeux
  -> sauvegarde dans Application.persistentDataPath
```

## Organisation des assets

| Emplacement | Rôle |
| --- | --- |
| `1 - SCRIPT/0-CLASSES`, `0-ENUM` | Modèles de données et énumérations métier. |
| `1 - SCRIPT/DONT DESTROY` | Services et état persistants : sauvegarde, équipe, items, médalium, audio. |
| `1 - SCRIPT/DIALCODE` | Chargement et rendu des dialogues JSON, boutiques et conditions. |
| `1 - SCRIPT/COMBATS` | Système séparé, volontairement non documenté ici. |
| `1 - SCRIPT/SAC` | Menu sac, équipe, médalium et journal. |
| `1 - SCRIPT/PLAYER`, `portes`, `livraison` | Joueur, téléportation et chargement de scènes. |
| `1 - SCRIPT/Interface UI en jeux` | HUD, minimap et indicateurs. |
| `7 - SCENNE` | Scènes Unity ; les scènes activées sont listées dans `ProjectSettings/EditorBuildSettings.asset`. |
| `Resources` | Dialogues et données chargées dynamiquement par chemin. |
| `Editor` | Outils d'import/export et éditeur Dialcode. |

## Services persistants et état global

Les objets suivants appliquent un singleton et/ou `DontDestroyOnLoad` :

| Service | Responsabilité |
| --- | --- |
| `NewDataCenter` | Journal de sauvegarde, monnaie, position et variables de jeu. |
| `MesYoKai` | Liste des Yo-kai possédés et composition de l'équipe. |
| `medaliumDictionnaire` | Chargement à la demande des métadonnées et visuels Yo-kai. |
| `Item` | Chargement des nourritures, équipements et animaux depuis `Resources`. |
| `PlayerPositionLocally` | Position temporaire du joueur et retour depuis les scènes annexes. |
| `DialcodePrinted` | UI de dialogue courante. |

Attention : l'ordre de création de ces services est important aujourd'hui.
Certains scripts recherchent un enfant par son nom sous `NewDataCenter`, alors
que d'autres accèdent directement à un singleton. Une scène de bootstrap unique
doit à terme créer et initialiser ces services dans un ordre maîtrisé.

## Sauvegarde

`NewDataCenter` conserve un dictionnaire `string -> string`, sérialisé en deux
listes parallèles (`keys`, `values`) dans :

```text
Application.persistentDataPath/journal<N>.json
```

Les principales clés observées sont :

| Clé | Usage |
| --- | --- |
| `nomDuJoueur` | Nom affiché dans les dialogues. |
| `scenne`, `pos1`, `pos2`, `pos3` | Reprise de scène et position du joueur. |
| `€` | Monnaie en centimes. |
| `IDdeMesYoKai`, `<id>_ID`, `<id>_niveau`, `<id>_xp` | Collection des Yo-kai. |
| `yoKaiDansEquipe`, `yoKaiDansEquipePosition` | Équipe et positions de roue. |
| `choix-numéro-<n>` | Choix effectués dans les dialogues. |

L'option de « chiffrement » actuelle est un XOR réversible, avec une clé dans
le code. Elle évite seulement une lecture immédiate de la sauvegarde ; elle ne
protège pas les données contre une modification. Voir les actions P0/P1 dans la
revue avant de l'utiliser comme mécanisme de sécurité.

## Dialogue et quêtes

1. `PNJDialcode` charge `Resources/texte/<jsonPath>-listePage.json`.
2. Il choisit la dernière page dont les conditions sont satisfaites.
3. `DialcodePrinted` affiche les `FragmentDeDialogue`, traite les choix,
   cadeaux, boutiques et quêtes.
4. Les effets sont inscrits dans `NewDataCenter`.

Les chemins Resource et la hiérarchie UI sont contractuels : les noms tels que
`-- BULLE --`, `Texte`, `icone` ou `fondu` ne doivent pas être renommés sans
mettre à jour les appels correspondants.

## Chargement des scènes

Trois mécanismes coexistent : chargement direct via `SceneManager`,
`LivraisonTp` pour placer le joueur à l'arrivée, et `PlayerPositionLocally`
pour les téléportations/retours additifs. Les scènes nécessaires à une build
doivent figurer dans `EditorBuildSettings.asset`; ne pas seulement créer le
fichier `.unity`.

## Vérification locale

La configuration de qualité existante est `qodana.yaml` (profil .NET starter).
Il n'existe pas de tests Unity dans le dépôt. Avant une livraison :

1. Ouvrir le projet dans l'éditeur de la version indiquée et vérifier la Console.
2. Tester menu -> nouvelle partie -> sauvegarde -> rechargement.
3. Tester un dialogue avec choix, un achat et une quête.
4. Tester une activité hors combat et le retour à la carte.
5. Lancer Qodana dans la CI et corriger les nouvelles alertes.
