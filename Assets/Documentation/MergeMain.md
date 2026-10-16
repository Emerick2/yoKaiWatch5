# Rework combat et main

Intégration de `main` (`6e0c376e2`) dans `Rework-CombatManager`.

| Branche | Grandes différences avant fusion |
| --- | --- |
| Rework | Données Yo-kai unifiées, contrôleurs de combat séparés, event bus, AP, roue animée, curseur, sac, résultats et retour de scène avec victoire/défaite. |
| Rework | Réassociation des textures de maps, scène Combat2 et intégration UnityMeta par UPM. |
| Main | Amitié via Dialcode, récompenses Bingo-kai et données d’objets actualisées, nouvelles cartes et modifications de scènes. |

La fusion conserve ces fonctionnalités. Les anciens types MedaliumNonPersonel/ListeDeMesYoKai ne sont pas réintroduits : les nouveaux appels utilisent YoKai/YoKaiActif. Les noms anglais nécessaires au Bingo-kai sont chargés dans les modèles unifiés.

Les 13 fichiers en conflit ont été résolus. Les objets des scènes ont été fusionnés par fileID ; les ajouts de main sont conservés avec les shaders URP du rework. Les affectations explicites de matériaux de main ont priorité sur les anciennes associations récupérées. Le verrou Burst de main et UnityMeta UPM du rework sont conservés.

## Vérifications

- Compilation séparée de l’event bus, des 237 sources runtime et des 13 sources Editor : OK.
- Compilation des sources des tests Editor : OK (tests Unity non exécutés ici).
- JSON des packages valide ; aucun marqueur de conflit restant.
- Aucun fileID dupliqué ni nouvelle référence locale manquante dans les trois scènes fusionnées.

## Test Unity avant la PR

1. Charger Combat2 : portraits, modèles, deux alliés simultanés et premier ennemi centré.
2. Tester les boutons, le sac vide/rempli, le curseur souris/manette, la roue et le remplacement du front K.-O.
3. Tester victoire et défaite : message de cinq secondes, passage du message, résultats puis retour à la scène avec CombatReturnedEvent.
4. Recevoir un Yo-kai via Dialcode et tirer des objets/Yo-kai au Bingo-kai.
5. Vérifier t101, yo-kai-world-1, espace-emerick et les nouvelles cartes de main, dont le restaurant.

Les exports, caches et modifications locales hors périmètre restent exclus. La PR sera préparée après ce test.
