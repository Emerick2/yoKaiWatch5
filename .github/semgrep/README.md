# C# quality checks

The `C# quality` GitHub Actions workflow uses Semgrep Community Edition (LGPL-2.1), pinned to version 1.181.0. It runs without a Semgrep account, token, paid rules or Unity license. Rules are maintained here in the repository; scans do not use the hosted platform and metrics are disabled.

Current checks detect exception rethrows that discard the original stack trace and Unity random calls inside constructors. Rule regression fixtures cover both findings and accepted code. This is a targeted source check, not a replacement for Qodana's full inspection catalogue, C# compilation or Unity tests. The Unity rule uses syntactic patterns and does not follow calls into helper methods; it is not a complete serialization-safety check.

Pull requests report new findings relative to their base commit. Pushes to main and manual runs scan all maintained C# sources. Imported plugins, TextMesh Pro and the nested Assets/Assets copy are excluded. JSON and SARIF reports are saved as workflow artifacts; the job only needs read access to repository contents.

Run from the repository root:

```sh
python -m pip install semgrep==1.181.0
semgrep --test .github/semgrep --metrics=off
semgrep scan --config .github/semgrep/rules.yml --metrics=off --error --strict --include '*.cs' --exclude 'Assets/Assets/**' --exclude 'Assets/Plugins/**' --exclude 'Assets/TextMesh Pro/**' Assets
```

Qodana's workflow and configuration were removed. The unused `QODANA_TOKEN` repository secret can be deleted by a repository administrator.
