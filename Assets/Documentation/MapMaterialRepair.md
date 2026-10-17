# Map material texture recovery

Scanned 101 Collada models and 2,924 map material files, including the nested Assets copy.

- Verified 1,454 textured material bindings; 1450 material files changed.
- Assigned valid Texture2D references to both `_BaseMap` and `_MainTex`.
- Preserved shaders, material GUIDs, renderer assignments and existing valid textures.
- Used Collada image/sampler references, CRC32 image-name IDs, and material bindings from Git before the broken conversion (`0f1bfe436^`).
- Kept 1,288 materials with no source texture (image ID zero or untextured color/shadow surfaces) unchanged.
- Left 182 ambiguous variants unchanged across the original and nested copy; their source names are absent and historical mappings disagree.
- Checked 1457 model-to-material GUID references; 0 missing references.

See MapMaterialBindings.csv for verified bindings and MapMaterialUnresolved.csv for outstanding variants. Refresh/reimport the modified material assets in Unity. Visual scene rendering has not been tested here.
