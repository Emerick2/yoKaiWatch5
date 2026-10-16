# Combat refactor merge review

## Scope

- Unify species, owned instances and combat instances; remove MedaliumYoKai consumers.
- Split combat orchestration into scene, turn, action, progression and result controllers.
- Route gameplay commands and presentation updates through typed events.
- Update Combat2 and its UI assets, model textures, AP scheduling and reserve transitions.

## Latest fixes

- Replace the combat wheel's 1 ms PressAndRepeat interaction with one Press interaction in both the input asset and generated wrapper.
- Subscribe input callbacks only while the component is enabled.
- Enable compact portraits and give zero-sized image roots a visible minimum size.
- Copy reward content through a wrapper with the first template's coordinate system, preserving each background's authored rectangle.
- Preserve the initial team order for rewards independently of combat wheel rotations.

## Validation

Targeted C# compilation succeeded for combat controllers, UI, appearance, generated input and InputManager. Editor test sources compile. Regression coverage includes compact/full portraits, sparse front/reserve ordering, initial reward order, transition cleanup and automatic reserve promotion.

These checks do not execute Unity Play Mode. Before merging, verify single presses and held buttons, six-step rotation in both directions, KO promotion, narrow and wide canvases, reward alignment, saved teams, and scene-only test teams. The result wheel and model/card alignment require visual validation.

## Commit boundaries

Exports, NuGet package caches, IDE files, generated font assets and unrelated project settings are excluded. Existing index entries outside this scope remain staged. No push or merge is performed.
