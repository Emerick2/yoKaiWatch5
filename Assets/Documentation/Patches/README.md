# Correctifs locaux de dépendances

Le fichier `UnityMeta-target-instance-object.patch` est conservé comme historique du correctif initial `[TargetInstance] object`.

Ce correctif a été intégré à UnityMeta et publié dans UPM. Ne l’applique pas au package actuel : le jeu utilise maintenant `upm-v0.2.0-alpha.8`, qui inclut également les templates génériques de changement de champ et l’émission compatible avec le core library de Unity pour les arguments `typeof(...)`.

`Mono.Cecil` reste fourni par UPM en version `1.11.4`; aucun package NuGet n’est utilisé par le projet Unity.
