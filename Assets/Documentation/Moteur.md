# Documentation interne du moteur — Yo-kai Watch 5

> Document de maintenance basé sur l’état du dépôt au 1 septembre 2026.
> Il décrit les systèmes de jeu, leur branchement dans Unity et leurs contrats
> implicites. Le système de combat est volontairement hors périmètre : cette
> page ne documente ni ses règles, ni sa boucle, ni ses scripts.

## 1. Fiche technique

- Projet Unity **2022.3.54f1** (`ProjectSettings/ProjectVersion.txt`).
- Produit : **Yo-Kai Watch 5**, version `0.0.4`.
- Rendu et interface : scènes 3D, UGUI et TextMesh Pro.
- Entrées : ancien Input Manager Unity (`activeInputHandler: 0`).
- Packages importants : AI Navigation 1.1.7, Cinemachine 2.10.3, Netcode for
  GameObjects 1.11.0, Newtonsoft Json 3.2.2, Timeline 1.7.6, UGUI et TMP.
- Les scripts métier sont dans l’assembly Unity par défaut : il n’y a pas
  d’asmdef ni de namespace global commun.
- Les données chargées dynamiquement utilisent principalement `Resources`.

Le projet mélange code livré, prototypes et exemples importés. Un dossier
`Z-Cours`, les scènes de test, les exemples TextMesh Pro/AI Navigation et les
anciens scripts de dialogue ne doivent pas être pris pour une API stable.

## 2. Lancement et cycle de vie

### Scène d’entrée

La scène active au démarrage d’une build est `Assets/7 - SCENNE/menu
principale.unity`. Les scènes activées dans les Build Settings sont :

- menu principal ; maison du héros ; visé le yo-kai ; combat ; Bingo-kai ;
  DevenirAmis ; plusieurs cartes (`Maison`, `chambre`, `ecole-ourcival`,
  `tout-pour-tous`, `t101`, `t102`, `t103`, `yo-kai-world-1`) ;
- attrape-insecte et `Combat2`.

Les autres scènes présentes dans `Assets/7 - SCENNE` sont des scènes de test ou
des scènes non ajoutées à la build. Ajouter un fichier `.unity` ne suffit pas :
il faut aussi l’ajouter dans `ProjectSettings/EditorBuildSettings.asset`.

### Initialisation globale

Les objets persistants sont créés soit par la scène, soit à la première lecture
de leur propriété `Instance` :

| Service | Rôle | Cycle de vie |
| --- | --- | --- |
| `NewDataCenter` | journal, variables, monnaie, position | singleton + `DontDestroyOnLoad` |
| `MesYoKai` | collection et équipe | singleton + `DontDestroyOnLoad` |
| `Item` | catalogue nourriture/équipement/animaux | singleton ; chargement au `Start` |
| `medaliumDictionnaire` | catalogue Yo-kai et modèles | singleton + `DontDestroyOnLoad` |
| `GestionnaireMusique` / `AudioManager` | musique et effets | objets persistants selon la scène |
| `DialcodePrinted` | afficheur de dialogue courant | singleton utilisé par les PNJ |

Le code dépend encore de hiérarchies existantes : certains scripts recherchent
`NewDataCenter/DICTIONNAIRE`, des objets par nom (`fondu`, `Texte`, `-- BULLE --`)
ou des tags (`Player`, `interfasse`, `Fondu`). Toute nouvelle scène doit
reproduire ces contrats ou fournir des références équivalentes.

## 3. Données et domaines métier

Les classes sérialisables sous `1 - SCRIPT/0-CLASSES` sont des données, pas des
composants de scène.

### Yo-kai

- `YoKai` décrit l’espèce : ID, nom, rang, tribu, personnalité et visuels.
- `YoKaiActif` décrit une instance possédée : `IDUnique`, surnom, niveau, XP,
  position dans la roue, objet tenu, statistiques de base, IV/EV et statistiques
  calculées.
- `MedaliumYoKai` est la fiche encyclopédique chargée depuis l’API locale :
  statistiques, attributs, texte de nourriture, images, son et modèle 3D.
- Les énumérations de référence sont dans `0-ENUM/yo-kai` : `Rang`, `Tribue`,
  `Personalité`, `Talent`, `Technique`, `Type`, `Element`, `PlatPréféré` et
  `StatutAmitié`.

`YoKaiActif` génère ses IV dans le constructeur (40 points répartis
aléatoirement), puis calcule les statistiques selon la formule présente dans le
code : `((2 * Base + IV + EV / 2) * Niveau) / 100 + 5`. Les statistiques de base
doivent donc être renseignées avant un appel à `Initialisation` si l’on crée une
instance manuellement. Le niveau maximal codé pour l’XP est 99.

### Objets

Les modèles `Item`, `Nouriture`, `Équipement` et `Annimal` se trouvent sous
`0-CLASSES/item`. Les catégories et effets sont des enums sous `0-ENUM/Item`.
`Food_tampons` représente la forme JSON intermédiaire.

`Item` charge et trie les fichiers `TextAsset` par ID depuis :

```text
Resources/yo-kai-API/data/food_data/
Resources/yo-kai-API/data/item-equipement_data/
Resources/yo-kai-API/data/bug_data/
```

Les images sont résolues à partir de `imageUrl` en remplaçant la racine de
l’API GitHub par `yo-kai-API/`. Les modèles d’animaux sont cherchés dans
`Resources/annimal/<nom>`. Les prix invalides prennent une valeur par défaut.
Les effets d’équipement sont extraits des marqueurs `FOR`, `VIT`, `ESP` et
`DÉF`; les descriptions de nourriture sont normalisées (minuscules et accents
retirés) avant analyse.

### Quêtes

`Quête` contient un nom, une description et une `CatégorieQuête`. La liste
globale est portée par `LesQuêtesQuiExistent`, généralement enfant de
`NewDataCenter/DICTIONNAIRE`. L’interface `LesQuête` instancie un prefab par
entrée, affiche l’icône de catégorie et considère la quête terminée quand la
clé portant son nom vaut `"1"`.

Les informations attachées à un fragment de dialogue sont dans
`QuestMenuInformation`; c’est le dialogue qui déclenche l’ajout ou la fin d’une
quête, puis écrit l’état dans `NewDataCenter`.

## 4. Persistance : `NewDataCenter`

### API d’usage

```csharp
NewDataCenter.Instance.SauvegarderSTR("maCle", "maValeur");
NewDataCenter.Instance.SauvegarderINT("compteur", 1);
string valeur = NewDataCenter.Instance.Lire("maCle");
NewDataCenter.Instance.Sauvgarder();
```

Le service conserve en mémoire un `Dictionary<string, string>`. Le fichier
physique du journal courant est toujours :

```text
Application.persistentDataPath/journal<N>.json
```

Le JSON contient deux tableaux parallèles `keys` et `values`, encapsulés dans
`DataContainer`. Les paramètres `dossier` de `Lire` et `SauvegarderINT` sont
historiques : le code force le nom `journal<N>.json`; ils ne permettent pas de
choisir un fichier indépendant.

### Journaux et chargement

`NewJournalNumber(n)` change de journal, recharge les données, met à jour le
meilleur score et rafraîchit les PNJ de la scène. `ResetJournal(n)` archive le
fichier sous un nom suffixé `_supr-<date>` au lieu de l’effacer directement.
`Sauvgarder()` capture la scène active et la position du joueur (`scenne`,
`pos1`, `pos2`, `pos3`) avant d’appeler `EcrireSurDisque()`.

Clés importantes observées :

| Clé | Contenu |
| --- | --- |
| `nomDuJoueur`, `thisIsAGirl` | profil et avatar |
| `scenne`, `pos1`, `pos2`, `pos3` | reprise de partie |
| `€` | monnaie stockée en centimes |
| `tempsDeJeu` | minutes ajoutées automatiquement |
| `IDdeMesYoKai` | nombre/compteur d’instances possédées |
| `<i>_ID`, `<i>_niveau`, `<i>_xp`, `<i>_personalité`, `<i>_surnom` | données d’un Yo-kai |
| `yoKaiDansEquipe`, `yoKaiDansEquipePosition` | composition et positions |
| nom d’une quête | valeur `1` quand terminée |
| variables utilisées dans les dialogues | progression et choix |

La monnaie est convertie en centimes avec `decimal`; une modification négative
ne peut pas faire passer le solde sous zéro. `SauvegarderINT` additionne la
valeur à la valeur existante : il ne s’agit pas d’un setter.

### Chiffrement actuel

Le flag `lesDonnéesDoiventÊtreChiffré` est activé. Les valeurs sont transformées
par XOR avec une clé codée en dur lors du chargement et avant/après l’écriture.
C’est une obfuscation réversible, pas une protection anti-triche. Ne jamais y
stocker un secret. En cas de diagnostic, désactiver le flag localement et
supprimer les journaux de test cohérents avec le mode utilisé.

## 5. Collection et médalium

`MesYoKai` charge les instances possédées à partir des clés indexées du journal,
puis charge l’équipe et ses positions. La chaîne d’équipe est séparée par des
caractères non numériques ; la position est validée dans l’intervalle 0–5 et
les doublons sont remplacés par les positions manquantes.

`medaliumDictionnaire.ServantYoKai(id)` utilise d’abord sa liste en mémoire,
puis charge `Resources/yo-kai-API/data/yokai_data/<id>.json`. Les images et le
son sont dérivés du nom normalisé par `MakeAnUrl`. Les valeurs de tribu et de
rang textuelles sont converties en enums avec des corrections de variantes.

`InstantiateYoKai(id)` lit
`Resources/yo-kai-API/data/_3D/yokai_assets_mapping.json`, instancie le modèle
FBX et applique ses textures, puis ajoute `MakeATexture` et `MakeAShader`.
L’ID passé doit correspondre à une clé du mapping et les fichiers référencés
doivent être sous `Resources/yo-kai-API/data/_3D/`.

Les scripts `fonctionnementMédalium`, `fonctionnementMédaillon`,
`RemplisLaRoue`, `ChangementEquipe` et `Page1DeChangeréquipe` ne possèdent pas
les données : ils mettent en page et modifient les listes du service persistant.

## 6. Dialogues « Dialcode »

### Fichiers

Un PNJ `PNJDialcode` porte un `jsonPath`. Il charge :

```text
Resources/texte/<jsonPath>-listePage.json
Resources/texte/<jsonPath>-page<ID>.json
```

Le premier fichier décrit les conditions et la visibilité des pages; le second
contient `{ "fragments": [...] }`. Les chemins d’images d’un fragment sont des
chemins `Resources` sans extension.

`FragmentDeDialogue` regroupe le texte, l’auteur, le type, les choix, une image,
un cadeau, une boutique, des conditions et des métadonnées de quête. Les enums
`TypeDialogue`, `QuiJeSuis`, `Opérateur` et `OpérateurEtape2` définissent le
format attendu par l’éditeur Dialcode.

### Exécution

1. `PNJDialcode` ajoute des colliders à ses enfants, attend
   `DialcodePrinted.Instance`, charge ses pages puis appelle `Refresh()`.
2. Quand le joueur entre dans la zone, le PNJ choisit la dernière page dont les
   conditions sont satisfaites.
3. `DialcodePrinted` affiche les phrases progressivement, remplace `<PNAME>`
   par le nom du joueur et traite les choix.
4. Les effets de fragment peuvent modifier une variable, donner un objet,
   ouvrir une boutique ou une quête, puis rafraîchir les PNJ.

Les conditions simples comparent des clés du dictionnaire global et peuvent
être combinées avec `&&`, `||` et des parenthèses. Toute nouvelle clé utilisée
dans un dialogue doit être initialisée ou accepter la valeur par défaut `"0"`.

La fenêtre de dialogue dépend de noms de hiérarchie et de composants UI :
préserver notamment le panneau de fondu, la bulle, le texte, les images et les
emplacements de choix lors de la création d’une scène.

`GestionnaireMenu` est un ancien interpréteur de dialogues/menus encore présent
dans le sac. Il ne faut pas le confondre avec le format JSON Dialcode ; avant de
modifier un dialogue, vérifier quel composant est réellement référencé par la
scène.

## 7. Joueur, caméra et déplacement

`mouvementDuJoueur` est placé sur le conteneur joueur. Au `Start`, il sélectionne
`Katie` ou `Nathan` selon `thisIsAGirl`, détruit l’autre, expose le personnage
actif via `mouvementDuJoueur.player` et ajoute/configure un `Rigidbody` si besoin.
Le Rigidbody conserve la gravité et bloque les rotations X/Z.

Le déplacement lit `Horizontal` et `Vertical` dans `FixedUpdate`; la direction
est normalisée, la vitesse normale est 5 et la vitesse de course 10 tant que le
bouton `B` est maintenu. La rotation utilise un `Slerp`. La variable statique
`droitDeMarcher` est le verrou global : elle arrête le déplacement et masque
l’interface taguée `interfasse` quand elle vaut `false`.

`CameraFollow` suit une cible avec un décalage sérialisé `pX`, `pY`, `pZ` et un
`SmoothDamp` de 0,25 seconde. Le script joueur recopie aussi la position X/Z du
joueur sur la caméra référencée. La référence `cameraFollow.target` doit donc
être valide après la sélection Katie/Nathan.

## 8. Transitions, portes et téléportation

- `Porte` attend le joueur dans son trigger, affiche l’aide via `InfoUI`, puis
  crée un `LivraisonTp` avant de charger la scène cible avec `A`.
- `TeleporteurAutomatique` réagit à un trigger ou une collision. Il peut
  téléporter immédiatement ou attendre `A`; il transmet `nameOfScene` et
  `IDTeleport` via `DataOfTeleport`.
- `LivraisonTp` reste persistant jusqu’à l’arrivée dans la scène cible, place le
  joueur à `positionPersonnage`, puis se détruit sauf pour un aller-retour.
- `LivraisonDébutUnePartie` restaure la position sauvegardée au lancement d’une
  scène, puis se détruit.

Les noms de scènes sont des chaînes sensibles aux accents et à la casse. Une
transition qui ne fonctionne pas doit être vérifiée dans cet ordre : scène dans
les Build Settings, nom exact, tag `Player`, objet de fondu et point de spawn.

## 9. Sac, inventaire et interface

`GestionnaireMenu`, `sacMouvement`, `mouvementSacObjet` et `PlaceLesApplicationsSac`
gèrent l’ouverture et la navigation visuelle du sac. `InvantaireSac` prépare
les catégories d’objets via `ServantGrille` et utilise `ServantImage` pour les
icônes de rang, tribu, élément et type.

`écrireLesPetitesValeursDuMenu` initialise les textes/valeurs du menu et
`EnregistreParamètre` sauvegarde les paramètres exposés par cette interface.
`LesLivres` change de page, tandis que `complétéLeJournal` affiche les éléments
de progression. Les composants UI héritent souvent de `Text`, `Image`,
`CanvasGroup` ou `RectTransform` directement : leurs références sérialisées et
leurs enfants nommés sont des dépendances fonctionnelles.

## 10. HUD, minimap et objectifs

- `InfoUI` est le point d’affichage des consignes contextuelles.
- `nomVille` affiche le nom de la zone.
- `montre` pilote l’interface de montre et ses boîtes.
- `iconnePNJMiniMap` instancie/positionne les icônes PNJ.
- `PointDeDestinationQuête` et `ArrowUIIndicator` affichent une destination,
  y compris lorsque la cible sort du viewport.
- `trouverUnYoKai` active un état quand un collider entre/sort de sa zone.

Les scripts de HUD ne calculent pas la progression métier : ils la lisent et la
représentent. La progression doit être écrite dans le journal ou le service
approprié avant de demander un rafraîchissement d’UI.

## 11. Audio, vidéo et présentation

`GestionnaireMusique` maintient les sources de musique et expose le changement
de thème ou la lecture d’un effet. `AudioManager.PlayClipAt` joue un clip à une
position. `SystèmeMusiqueLecteurSac` gère le lecteur musical du sac.

`ServantMovie` utilise une `VideoPlayer` temporaire pour lire une vidéo depuis
`Resources`; les render textures et vidéos du dossier `Assets/ici` doivent
rester référencées par les scènes qui les utilisent. `MakeATexture` et
`MakeAShader` appliquent les traitements visuels aux modèles. `fonducirculaire`
est une image UI personnalisée pour les transitions circulaires.

Les scripts racine `AnimationManager`, `AddAnimator`, `CacheAnimator`,
`AdjustModelScale`, `BackgroundMouve`, `MouvementDash`, `Tree`, `Dedur` et
`MakeATexture` sont des composants de présentation ou de prototype. Leur
présence dans `Assets/` ne signifie pas qu’ils sont requis par toutes les
scènes. `AnimationManager` utilise un `PlayableGraph` quand sa bibliothèque
d’animations est initialisée; son initialisation est actuellement en grande
partie commentée, donc vérifier la scène avant de l’utiliser comme système
générique.

## 11 bis. Réseau expérimental

Le package Netcode for GameObjects est installé, mais le seul point d’entrée
identifiable est `1 - SCRIPT/serveur/NetworkStartUI.cs`. Il affiche trois
boutons IMGUI (`Host`, `Client`, `Serveur`) et appelle le `NetworkManager`
singleton correspondant. Ce flux suppose qu’un `NetworkManager` existe déjà
dans la scène; il n’y a pas de transport, de lobby, de synchronisation du
joueur ou de persistance réseau documenté dans le projet. Le réseau doit donc
être considéré comme expérimental et être testé séparément.

## 12. Activités hors combat

### Bingo-kai

`BingoKai` orchestre les boules, les boutons `E`/`B`, les possibilités de prix et
le choix du cadeau. `ChoisirSaPièceDeBingoKai` filtre les pièces disponibles et
retourne la pièce sélectionnée. Les données de prix associent des noms d’objets
et des IDs de Yo-kai; l’attribution finale doit passer par les services de
données persistants.

### Capture d’insectes

La scène `attrape-insecte` et `GestionnaireJeuInsecte` utilisent les données
d’animaux chargées par `Item`. `Coffre`, `colliderItem`, `lesObjet` et
`lesObjetsQuiExistes` servent aux interactions et au placement d’objets de map.
`PersonnageAléatoireDeLaZonne` gère des personnages décoratifs aléatoires.

`MiniMir` contient le modèle d’un point de téléportation avec ID, nom, scène et
image. La navigation de sélection est encore vide dans le script actuel : ne
pas le traiter comme une téléportation fonctionnelle sans compléter le
câblage.

### Prototypes IA et objets de test

`Z-Cours/Emerick/test IA` contient une behavior tree minimale (`Sequence`,
`Selector`, états `Success/Failure/Running`) et un `NavMeshAgent`. C’est un
prototype de navigation/détection, pas un service global du moteur. Les scripts
`Z-Cours`, `Mohand`, `test*`, ainsi que les scènes `zone-de-test`, sont à isoler
avant toute réutilisation en production.

## 13. Navigation, physique et génération d’assets

AI Navigation fournit les `NavMeshSurface` et assets de NavMesh présents dans
`2 - map 3D/IA yo-kai`. Les colliders peuvent être générés par
`AutoColliderGenerator` pour les meshes skinnés. `tousEstDéplasable` permet des
objets physiques déplaçables; `ServantBoxCollider3D` crée des colliders autour
des enfants d’un PNJ ou d’un modèle.

Les fichiers `Exports/t101`, `t102` et `t103` sont des exports de map (GLB/OBJ)
et `Tools/export_collada_map.py` est un outil de traitement, pas une dépendance
runtime Unity.

`MouvePNJ` est le déplacement waypoint d’un PNJ : il suit une liste de
`StopMouveStruct`, attend à chaque point, peut boucler et évite les obstacles
par `SphereCast`. Les deux `LayerMask` distinguent les obstacles généraux et
le joueur. `TousEstAparaisable`, `tousEstDéplasable` et les petits scripts dans
`Z-Cours` sont des comportements de scène ou de test, non des services de
navigation centralisés.

## 14. Contrats de scène et checklist d’intégration

Avant de valider une nouvelle scène :

1. Ajouter la scène aux Build Settings si elle est destinée à la build.
2. Vérifier la présence d’un joueur tagué `Player`, d’un collider et d’un sol
   physique/navigation adapté.
3. Vérifier le service `NewDataCenter` et, si nécessaire, les objets enfants
   attendus (`DICTIONNAIRE`, données de quêtes, catalogue).
4. Vérifier `interfasse`, `Fondu`, `fondu` et les références de caméra.
5. Vérifier les points de téléportation et la position de retour.
6. Vérifier les chemins `Resources` utilisés par les dialogues, objets et
   modèles; les extensions ne doivent pas être ajoutées aux appels
   `Resources.Load` qui les retirent déjà.
7. Tester changement de scène, sauvegarde/reprise, UI et destruction des objets
   temporaires.

## 15. Débogage et limites connues

- Une valeur absente de `NewDataCenter.Lire` revient à `"0"`; cela peut masquer
  une clé oubliée. Loguer explicitement les nouvelles clés.
- Les recherches `FindObjectOfType`, `FindWithTag`, `Transform.Find` et les
  singletons rendent l’ordre d’initialisation important.
- Les noms de fichiers et de classes contiennent des accents, espaces et
  fautes historiques : conserver les chemins exacts lors d’un déplacement.
- `Resources.LoadAll` charge tout un dossier et plusieurs catalogues restent
  en mémoire; surveiller le coût lors de l’ajout de contenu.
- Le dépôt ne contient pas de tests Unity automatisés. La vérification minimale
  est donc une exécution dans l’éditeur avec Console sans erreur, suivie d’un
  parcours menu → nouvelle partie → déplacement → dialogue → sac → activité →
  sauvegarde → reprise.

## 16. Règles de maintenance

- Ne pas utiliser la sauvegarde comme stockage de référence pour un catalogue
  statique : les catalogues restent dans `Resources`/assets et le journal ne
  conserve que la progression.
- Lorsqu’une fonctionnalité modifie une progression, définir d’abord sa clé,
  sa valeur initiale et son moment de sauvegarde.
- Préférer des références sérialisées pour les nouveaux composants, tout en
  conservant les noms contractuels nécessaires aux scènes existantes.
- Documenter dans cette page tout nouveau singleton, chemin `Resources`, tag,
  nom d’enfant ou clé de journal.
- Garder le combat dans sa documentation dédiée pendant sa refonte; cette page
  doit rester centrée sur les systèmes transversaux et hors combat.
