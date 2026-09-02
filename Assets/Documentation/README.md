# Documentation interne — Yo-kai Watch 5

Cette documentation décrit l'état actuel du projet Unity afin de faciliter son
onboarding, sa maintenance et sa remise à niveau. Elle porte sur le contenu du
dépôt au 1 septembre 2026.

## Démarrage rapide

- Ouvrir le dossier parent `yoKaiWatch5` avec **Unity 2022.3.54f1**.
- La scène de démarrage déclarée dans les Build Settings est
  `7 - SCENNE/menu principale.unity`.
- Les touches/boutons d'action utilisent l'ancien Input Manager : `A`, `B`,
  `X`, `Y`, `L`, `R`, `up`, `down`, `left`, `right`.
- Les scènes de jeu, les données `Resources` et les scripts sont tous sous
  `Assets`.

## Contenu de la documentation

- [Moteur.md](Moteur.md) : référence complète des systèmes du moteur et de
  leurs contrats d’intégration, hors combat.

- [Architecture.md](Architecture.md) : vue synthétique de l’organisation,
  persistance, scènes et conventions.
- [Code-review.md](Code-review.md) : constats de revue, priorisés et reliés
  aux emplacements source.

## Périmètre

Le dépôt contient les scripts du jeu, des prototypes et des exemples TextMesh
Pro/AI Navigation. La documentation du moteur distingue ces catégories et se
concentre sur les systèmes transversaux et hors combat.

Les dossiers `Z-Cours/`, `probablemant plus utile/` et les exemples importés
doivent être considérés comme du code de démonstration ou historique tant
qu'ils ne sont pas explicitement intégrés à une fonctionnalité livrée.

## Règles de contribution proposées

1. Ne pas modifier les assets, scènes ou fichiers déjà changés par un autre
   contributeur sans coordination.
2. Avant d'ajouter une fonctionnalité, identifier son propriétaire : données,
   dialogue, combat, UI ou navigation.
3. Éviter les nouveaux `GameObject.Find`, `Transform.Find` et chaînes de noms
   de scène ; préférer une référence sérialisée, un identifiant centralisé ou
   une API dédiée.
4. Toute correction de sauvegarde, combat ou dialogue doit inclure un scénario
   de reproduction manuel, puis être ajoutée à une future suite de tests Unity.
5. Conserver les fichiers de données dans `Resources` uniquement lorsqu'un
   chargement par chemin est réellement nécessaire ; privilégier ensuite des
   références explicites ou Addressables.
