# UnityMeta

UnityMeta is an experimental metaprogramming/aspect framework for **Unity 2022.3 / C# 9**.
It aims to recover the useful authoring model of tools such as Metalama without replacing
Unity's Roslyn compiler.

UnityMeta itself is intentionally **not a catalog of gameplay attributes**. `Clamp`,
`OnChange`, `Log`, and similar features are samples showing how a Unity project or another
package can author its own metacode.

> Current version: **0.2.0-alpha.2**. The architecture is usable; the API is still allowed
> to evolve before 1.0.

## Architecture

UnityMeta combines three layers:

1. a tiny C# 9 authoring API (`FieldGetAspectAttribute`, `FieldSetAspectAttribute`,
   `FieldChangeAspectAttribute`, `MethodAspectAttribute`, template/binding attributes),
2. a Unity `ILPostProcessor` + Mono.Cecil backend for behavior rewriting,
3. a Roslyn 3.8 analyzer/source-generator companion for early diagnostics and future
   source-visible introductions.

This lets ordinary Unity fields remain real fields while aspects can be defined inside the
same project that consumes them.

## Implemented authoring powers

- user-defined field-read transformations with `[GetTemplate]`;
- user-defined field-write transformations with `[SetTemplate]`;
- user-defined field-change hooks with `[ChangeTemplate]`;
- `[OldValue]` / `[NewValue]` and real-change filtering using
  `EqualityComparer<T>.Default`;
- multiple ordered aspects on the same member;
- constructor and named attribute argument bindings;
- primitive, string, enum, `typeof(...)`, and attribute-array metadata emission;
- `nameof(...)` sibling-field value bindings without runtime reflection;
- method `[BeforeTemplate]` / `[AfterTemplate]`;
- `[ReturnValue]` observation from after templates;
- target instance, target method argument, member name and type name bindings;
- Roslyn diagnostics for malformed templates;
- generated aspect manifest;
- Unity-independent Cecil/compiler smoke tests;
- Unity 2022.3 integration-test fixture;
- UPM release packaging and an optional NuGet authoring package.

## Example: user-defined clamp

```csharp
using UnityMeta;

public sealed class ClampAttribute : FieldSetAspectAttribute
{
    public ClampAttribute(int min, int max) { }
    public ClampAttribute(int min, string maxField) { }

    [SetTemplate]
    public static int Constant(
        [Value] int value,
        [AspectArgument(0)] int min,
        [AspectArgument(1)] int max)
        => value < min ? min : value > max ? max : value;

    [SetTemplate]
    public static int DynamicMax(
        [Value] int value,
        [AspectArgument(0)] int min,
        [FieldValueFromAspectArgument(1)] int max)
        => value < min ? min : value > max ? max : value;
}
```

Usage remains ordinary Unity C#:

```csharp
[Clamp(0, nameof(hpMax))]
public int hp;

public int hpMax = 100;
```

`hp = 900;` is rewritten after compilation so the selected template runs before `stfld`.
`hp` remains a real field.

## Example: OnChange-style metacode

`FieldChangeAspectAttribute` is a generic framework primitive. An aspect can observe a real
transition without UnityMeta knowing anything about gameplay:

```csharp
public sealed class HealthChangedAttribute : FieldChangeAspectAttribute
{
    [ChangeTemplate]
    public static void Changed(
        [TargetInstance] Combat target,
        [OldValue] int oldValue,
        [NewValue] int newValue)
    {
        target.OnHealthChanged(oldValue, newValue);
    }
}
```

```csharp
[HealthChanged]
public int hp;
```

If another `[SetTemplate]` clamps/transforms the assignment first, `newValue` is the final
stored value. Reassigning the same final value does not trigger the change template.

## Installation

### Development checkout

Add the package from disk, or from your project's `Packages/manifest.json`:

```json
"com.drapnard.unitymeta": "file:../../UnityMeta/Packages/com.drapnard.unitymeta"
```

For a source checkout, build the optional Roslyn companion with:

```bash
./build.sh
./tools/install-compiler.sh
```

### Published release

A tagged release builds a precompiled UPM branch. Install an exact release with:

```text
https://github.com/DrapNard/UnityMeta.git#upm-v0.2.0-alpha.2
```

The source-repository package is also addressable with Unity's Git `path` query, but the
`upm-v*` tag is preferred because it includes the precompiled Roslyn companion.

GitHub Releases additionally contain a `.tgz`, checksums, and `UnityMeta.Authoring.nupkg`.
The NuGet package is for tooling/authoring scenarios; **Unity weaving still requires UPM**.

## Build and verification

```bash
./build.sh
./tools/verify-repository.sh
./tools/package-release.sh
```

The smoke suite compiles C# 9 fixtures, runs the real Cecil weaver, executes transformed
assemblies, tests the Roslyn generator/analyzer, and covers field reads, clamp, real-change
hooks, metadata bindings and return-value observation.

A manual GitHub Action also runs a real Unity 2022.3 EditMode project when Unity/GameCI
license secrets are configured.

## Not implemented yet

UnityMeta does **not** claim Metalama parity yet. Major remaining work includes generalized
`Around`/`meta.Proceed()`, template inlining, source-visible member introduction, project-wide
selectors/fabrics, exception-safe finally semantics, dependency graphs, property aspects,
ref/address field access interception, and async/iterator state-machine semantics.

See [feature matrix](docs/feature-matrix.md), [limitations](docs/limitations.md), and the
[roadmap](docs/roadmap.md).

## Documentation

- [Architecture](docs/architecture.md)
- [Writing aspects](docs/aspect-authoring.md)
- [Unity 2022 setup](docs/unity-2022-setup.md)
- [Publishing](docs/publishing.md)
- [PolySharp and language compatibility](docs/polysharp.md)
- [Testing](docs/testing.md)
- [Known limitations](docs/limitations.md)
- [Feature matrix](docs/feature-matrix.md)
- [Architecture decision: Unity-native backend](docs/decisions/0001-unity-native-backend.md)
- [Roadmap](docs/roadmap.md)
- [Contributing](CONTRIBUTING.md)

## License

MIT. See [LICENSE.md](LICENSE.md).
