using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixerParameter : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private VolumeMixerUI volumeMixerPrefab;
    [SerializeField] private Transform container;

    [SerializeField]
    private string parameterSuffix = "Volume";

    private readonly List<VolumeMixerUI> instances = new();

    private void Start()
    {
        Rebuild();
    }

    public void Rebuild()
    {
        foreach (var instance in instances)
        {
            if (instance != null)
                Destroy(instance.gameObject);
        }

        instances.Clear();

        if (mixer == null || volumeMixerPrefab == null)
        {
            Debug.LogError("MixerParameter: configuration manquante.");
            return;
        }

        Transform parent = container != null
            ? container
            : transform;

        // Récupération automatique des groupes du mixer
        AudioMixerGroup[] groups =
            mixer.FindMatchingGroups(string.Empty);

        foreach (AudioMixerGroup group in groups)
        {
            string parameterName = group.name + parameterSuffix;

            // Ne créer que les volumes réellement exposés
            if (!mixer.GetFloat(parameterName, out _))
                continue;

            VolumeMixerUI instance = Instantiate(
                volumeMixerPrefab,
                parent
            );

            instance.name = $"VolumeMixer_{group.name}";

            instance.Initialize(
                mixer,
                parameterName,
                group.name
            );

            instances.Add(instance);
        }
    }
}