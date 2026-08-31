# Revue de code — 31 août 2026

## Méthode et limites

Revue statique des scripts de jeu, des scènes déclarées, des entrées et des
flux sauvegarde/dialogue/combat/navigation. Les exemples TextMesh Pro et AI
Navigation ont été exclus des constats métier. Une compilation batch Unity a
été tentée mais l'éditeur avait déjà le projet ouvert : Unity refuse deux
instances pour un même projet. Les constats ci-dessous sont donc fondés sur le
code source et doivent être validés dans la Console de l'éditeur déjà ouvert.

## Synthèse

La base possède des fonctionnalités importantes déjà reliées entre elles
(sauvegarde, dialogue JSON, collection, combat et mini-jeux), mais elle reste
fragile en raison de l'état global, des références par nom et de plusieurs
erreurs fonctionnelles avérées. Priorité : fiabiliser la sauvegarde et les
calculs de combat avant d'ajouter du contenu.

## Constats à corriger

| Priorité | Emplacement | Constat et impact | Correction recommandée |
| --- | --- | --- | --- |
| P0 | `1 - SCRIPT/DONT DESTROY/NewDataCenter.cs:195` | `MeilleurScoreADéfinir` compare deux valeurs puis appelle `SauvegarderINT`, qui additionne au lieu d'affecter. Un score de 10 remplacé par 20 devient 30. | Ajouter `DefinirInt`/`SetValue` et l'utiliser pour les valeurs absolues. Conserver une méthode `AjouterInt` explicite. |
| P0 | `1 - SCRIPT/DONT DESTROY/MesYoKai.cs:36-41` | Deux Yo-kai de test sont ajoutés à chaque démarrage, y compris après le chargement d'une sauvegarde. La collection et la sauvegarde grossissent à chaque lancement. | Retirer ces appels ou les protéger par une option de développement et une initialisation de nouvelle partie unique. |
| P0 | `1 - SCRIPT/DONT DESTROY/MesYoKai.cs:134-146` | `maListe.Add` est conditionnel à l'existence de l'ID sauvegardé, mais l'écriture utilise ensuite `maListe[i]`. Si une entrée est absente/corrompue, l'index diverge et produit une `ArgumentOutOfRangeException`. | Construire un objet local, l'ajouter, puis travailler sur `maListe[^1]`; conserver un mapping ID unique -> index. |
| P0 | `1 - SCRIPT/COMBATS/DiminuerLaVieScript.cs:79` | Les données du défenseur sont chargées avec `attaquant.ID`, donc faiblesses/résistances et données cible sont celles de l'attaquant. | Charger `ServantYoKai(cible.ID)` et couvrir le cas par un test. |
| P0 | `1 - SCRIPT/COMBATS/DiminuerLaVieScript.cs:74` | Le type d'attaque est toujours `Physique`; le chemin technique/élémentaire ne peut jamais être exécuté. | Porter le type et la technique dans l'action de combat, puis les transmettre à `DiminueLaVie`. |
| P1 | `1 - SCRIPT/DONT DESTROY/NewDataCenter.cs:118-140` | Le paramètre `dossier` est ignoré et remplacé par `journal<N>.json`. Des appelants demandent pourtant `position.json`, `équipe.json` ou `mesData.json`; leur intention n'est donc pas respectée. | Soit supprimer ce paramètre de tous les appels et documenter un journal unique, soit réellement séparer les fichiers. |
| P1 | `1 - SCRIPT/page aceuil/menuPrincipal.cs:82-90` | Une nouvelle partie supprime `mesData.json`, alors que le système actif écrit `journal1.json`. Elle ne réinitialise donc pas forcément la partie courante ; elle désactive/détruit aussi le singleton d'une façon fragile. | Appeler une API `ResetJournal` robuste, recréer l'état sans désactiver le singleton et vérifier la reprise après redémarrage. |
| P1 | `1 - SCRIPT/DONT DESTROY/Item.cs:56-63` | Le parseur concatène l'indice `i` au lieu du caractère lu (`temps2 += i`). Des effets numériques comme `FOR+12` sont mal lus. | Remplacer par `temps2 += temps[i]`; ajouter des cas de test `+1`, `-5`, `+12` et deux effets. |
| P1 | `1 - SCRIPT/DONT DESTROY/Item.cs:178-181` | La condition est inversée : `effetSur_ELSE` ne reçoit jamais l'effet quand il est vide. | Tester `string.IsNullOrEmpty(nouv.effetSur_ELSE)` ou affecter directement le texte non interprété. |
| P1 | `1 - SCRIPT/0-CLASSES/Servant/ServantGrille.cs:167-171` | Avec zéro item, le modulo/diviseur basé sur le nombre de lignes vaut zéro. L'ouverture d'une grille vide peut planter. | Sortir tôt lorsque `totalItems == 0` et désactiver la sélection. |
| P1 | `1 - SCRIPT/DIALCODE/PNJDialcode.cs:45-55` | Si le JSON de dialogue n'existe pas, la liste précédente est conservée. Un PNJ peut alors afficher un dialogue obsolète. | Vider la liste avant le chargement et journaliser clairement le chemin manquant. |
| P1 | `1 - SCRIPT/SAC/fonctionnementMédaillon.cs:231-258` | Les offsets de page sont incohérents (`12 * numéroDeLaPage` au lieu de `(numéroDeLaPage - 1) * 12`) et le dernier écran peut sélectionner une case sans Yo-kai. | Centraliser `indexGlobal = (page - 1) * 12 + index`; désactiver/vider explicitement chaque case absente. |
| P1 | `1 - SCRIPT/DONT DESTROY/NewDataCenter.cs:23-24, 203-212` | Le XOR avec clé en clair ne constitue pas un chiffrement sûr et l'option est active par défaut. Toute sauvegarde reste facilement modifiable. | Le présenter comme obfuscation, ou employer une protection adaptée à la plateforme avec intégrité; ne jamais prétendre protéger des secrets. |
| P2 | `1 - SCRIPT/COMBATS/GestionnaireCombat.cs` (1 107 lignes), `DialcodePrinted.cs` (760), `RemplisLaRoue.cs` (627) | Classes à responsabilités multiples : logique métier, input, instanciation, UI et état dans le même fichier. Les corrections deviennent risquées. | Découper progressivement par responsabilité, sans grand refactoring non testé. |
| P2 | Projet entier | Les singletons créent des objets à la volée et les scripts recherchent aussi des objets/enfants par chaînes (`Find`, tags). Une scène ou hiérarchie renommée provoque des `NullReferenceException` au runtime. | Mettre un bootstrap, des références sérialisées, des validations `OnValidate` et des messages d'erreur contextualisés. |
| P2 | Projet entier | Les noms de scènes et de chemins `Resources` sont éparpillés sous forme de littéraux. | Centraliser les identifiants de scènes/ressources et valider les entrées de Build Settings en CI. |
| P2 | Projet entier | Aucun test Unity n'est présent. Les régressions de sauvegarde et combat ne sont pas détectées automatiquement. | Créer des tests EditMode pour parsing, sauvegarde et dégâts, puis PlayMode pour le flux scène/dialogue. |
| P3 | Projet entier | Pas de namespaces ni d'assembly definitions pour le code du jeu ; le projet dépend de l'assembly par défaut et importe parfois `Unity.VisualScripting` sans besoin explicite. | Introduire un namespace projet et des `.asmdef` par module lorsque les P0/P1 sont stabilisés. |
| P3 | `1 - SCRIPT/probablemant plus utile`, `Z-Cours`, exemples importés | Code historique, prototype et code tiers résident avec le code livré, ce qui augmente la surface de maintenance. | Déplacer/archiver ce qui n'est plus utilisé et conserver une liste d'assets de démonstration exclus des builds. |

## Points positifs

- La version Unity et les dépendances sont verrouillées dans le projet.
- Les scènes de production sont déjà déclarées dans les Build Settings.
- Le dialogue dispose d'un format JSON éditable et d'un outil éditeur dédié.
- Plusieurs accès externes ont des garde-fous (`null` et bornes) utiles, en
  particulier lors de l'instanciation de modèles et de la sélection de cibles.
- Un premier contrôle de qualité Qodana est configuré dans la CI.

## Plan de remise à niveau conseillé

1. Corriger les six P0 et écrire un test EditMode pour chacun.
2. Unifier le contrat de sauvegarde (journal unique ou fichiers distincts), puis
   tester nouvelle partie, reprise et reset sur disque.
3. Sécuriser les écrans vides et les données Resources absentes.
4. Extraire le calcul de combat et les données de combat dans des classes sans
   `MonoBehaviour`; tester les dégâts, la garde et les éléments.
5. Créer une scène de bootstrap et remplacer progressivement les recherches par
   nom/tags par des références ou services injectés.

