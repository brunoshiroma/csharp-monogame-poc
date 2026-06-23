# AGENTS Improvement Spec

Audit of the repository's agent-guidance state and a concrete plan to improve it.

---

## What's Good

| Area | Finding |
|---|---|
| `.gitignore` | Thorough MonoGame/VS ignore rules; covers binaries, IDE files, NuGet packages, and platform artifacts. |
| Solution structure | Two-project split (engine vs. game) is clean and extensible. |
| `Core.cs` singleton guard | Explicit `InvalidOperationException` prevents accidental double-instantiation. |
| Static accessors | `Core.Graphics`, `Core.SpriteBatch`, etc. reduce boilerplate in subclasses. |
| `Content.mgcb` | Pipeline config is present and correctly configured for DesktopGL. |
| `devcontainer.json` | Devcontainer exists; universal image means .NET 8 SDK is available out of the box. |

---

## What's Missing

| Gap | Impact |
|---|---|
| **No AGENTS.md** | Agents have no project context, conventions, or build instructions. High risk of incorrect edits. |
| **No build/run instructions anywhere** | Agents and new contributors must reverse-engineer the build from `.csproj` files. |
| **No headless/CI guidance** | MonoGame DesktopGL requires SDL2 and a display; no documentation on how to handle headless environments. |
| **No content pipeline documentation** | Agents don't know that assets must be registered in `Content.mgcb` before use, or that `.xnb` files are generated artifacts. |
| **No architecture decision record** | The singleton pattern in `Core` is non-obvious; no explanation of why it exists or its constraints. |
| **`devcontainer.json` has no automations** | No `postCreateCommand` to restore NuGet packages or install `dotnet-mgcb` tooling. |
| **No `.ona/skills/` content** | The `.ona/review` file is empty; no agent skills are defined. |
| **No test project** | No unit or integration tests; agents cannot verify correctness of changes. |
| **`Game1.cs` has stale TODOs** | Three `// TODO` stubs remain in the main game class with no tracking. |

---

## What's Wrong

| Issue | Severity | Detail |
|---|---|---|
| `devcontainer.json` uses universal image | Medium | `mcr.microsoft.com/devcontainers/universal:4.0.1-noble` is ~10 GB. A .NET-specific image would start faster and be more reproducible. |
| `dotnet` version mismatch | Medium | Projects target `net9.0` but the installed SDK is `8.0.413`. Builds will fail unless .NET 9 SDK is installed. |
| `Core.cs` `Initialize()` ordering bug | Low | `GraphicsDevice` and `SpriteBatch` are assigned *after* `base.Initialize()`, but subclasses calling `base.Initialize()` first will have `null` references if they try to use them in their own `Initialize()` body before the base call returns. |
| `Game1.Draw()` calls `base.Draw()` before `SpriteBatch.Begin()` | Low | `base.Draw()` is called mid-method; if `Core` ever adds draw logic it will execute before the subclass's sprite batch, causing ordering issues. |
| No `OutputType` in `GameCore.csproj` | Low | Missing `<OutputType>Library</OutputType>` is implicit but should be explicit for clarity and tooling. |
| `.ona/review` file is empty | Low | Placeholder file with no content; misleading to agents that inspect it. |

---

## Improvement Plan

### 1. Fix SDK / Target Framework Mismatch (Priority: High)

**Option A** — Downgrade projects to `net8.0` (matches installed SDK).  
**Option B** — Install .NET 9 SDK in the devcontainer.

Recommended: Option B. Add to `devcontainer.json`:

```json
"features": {
  "ghcr.io/devcontainers/features/dotnet:2": {
    "version": "9.0"
  }
}
```

And add a `postCreateCommand`:

```json
"postCreateCommand": "dotnet restore csharp-monogame-poc.sln"
```

### 2. Switch to a Lighter Devcontainer Image (Priority: Medium)

Replace the universal image with a .NET-specific one:

```json
"image": "mcr.microsoft.com/devcontainers/dotnet:9.0"
```

Add SDL2 and virtual framebuffer support for headless MonoGame:

```json
"postCreateCommand": "sudo apt-get install -y libsdl2-dev xvfb && dotnet restore csharp-monogame-poc.sln"
```

### 3. Fix `Core.cs` Initialization Ordering (Priority: Low)

Move `SpriteBatch` and `GraphicsDevice` assignment to a `LoadContent` override or document the constraint explicitly so subclasses know not to use them before `base.Initialize()` returns.

### 4. Fix `Game1.Draw()` Call Order (Priority: Low)

Move `base.Draw(gameTime)` to the end of the method, after `SpriteBatch.End()`.

### 5. Add `OutputType` to `GameCore.csproj` (Priority: Low)

```xml
<OutputType>Library</OutputType>
```

### 6. Add a Test Project (Priority: Medium)

Create `GameCore.Tests/` with xUnit. Test the singleton guard and any pure logic in `Core`.

### 7. Populate `.ona/review` or Remove It (Priority: Low)

Either add a meaningful code-review checklist or delete the empty file.

### 8. Track and Resolve TODOs (Priority: Low)

Replace `// TODO` stubs in `Game1.cs` with real implementations or open tracked issues.

---

## AGENTS.md Spec

The `AGENTS.md` created alongside this file covers:

- Project overview and layout
- Architecture conventions (singleton, static accessors, content pipeline)
- Build and run commands
- Headless environment guidance
- Steps for adding new game projects
- Code style rules
- Explicit "do not" list for agents

**Future additions to AGENTS.md should include:**

- Devcontainer setup steps once the image is updated
- Test commands once a test project exists
- Asset pipeline workflow (how to add new textures/sounds)
- Branching and PR conventions if the project grows
