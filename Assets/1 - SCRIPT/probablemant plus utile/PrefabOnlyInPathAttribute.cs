using UnityEngine;

public class PrefabOnlyInPathAttribute : PropertyAttribute {
    public string Path;

    public PrefabOnlyInPathAttribute(string path) {
        this.Path = path;
        Debug.Log(path);
    }
}
