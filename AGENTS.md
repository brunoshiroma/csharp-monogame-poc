# AGENTS.md

Agent guidance for the `csharp-monogame-poc` repository.

---

## Project Overview

A C# / MonoGame proof-of-concept structured as a Visual Studio solution with two projects:

| Project | Purpose |
|---|---|
| `GameCore/` | Reusable engine wrapper (`Core` class) around `Microsoft.Xna.Framework.Game` |
| `csharp-monogame-poc/` | Runnable game entry-point; currently a Galaxia/space-shooter stub |

Target framework: **net9.0** (both projects).  
MonoGame package: `MonoGame.Framework.DesktopGL 3.8.*`.

---

## Repository Layout

```
csharp-monogame-poc.sln
GameCore/
  Core.cs                  # Singleton Game wrapper; exposes static Graphics, SpriteBatch, Content, GraphicsDevice
  GameCore.csproj
csharp-monogame-poc/
  Game1.cs                 # Concrete game class; extends Core
  Program.cs               # Entry point
  Content/
    Content.mgcb           # MonoGame Content Builder pipeline config
    images/                # Raw source assets (PNG, etc.)
  csharp-monogame-poc.csproj
.devcontainer/
  devcontainer.json        # Universal devcontainer image (mcr.microsoft.com/devcontainers/universal:4.0.1-noble)
.gitignore                 # MonoGame / VS standard ignores
```

---

## Architecture Conventions

- **`GameCore.Core`** is a singleton. Never instantiate it more than once per process.
- Static accessors (`Core.Graphics`, `Core.SpriteBatch`, `Core.Content`, `Core.GraphicsDevice`) are available after `Initialize()` completes.
- New game projects should extend `Core`, not `Microsoft.Xna.Framework.Game` directly.
- Content assets go in `Content/` and must be registered in `Content.mgcb` before use.
- `SpriteBatch.Begin()` / `End()` must wrap all draw calls; call them inside `Draw()` overrides.

---

## Build & Run

```bash
# Restore and build the full solution
dotnet build csharp-monogame-poc.sln

# Run the main game (requires a display / virtual framebuffer on headless systems)
dotnet run --project csharp-monogame-poc/csharp-monogame-poc.csproj
```

> **Headless note:** MonoGame DesktopGL requires SDL2. On headless CI/dev containers, prefix with `DISPLAY=:99 Xvfb :99 -screen 0 1280x720x24 &` or use a virtual framebuffer.

---

## Adding a New Game Project

1. Create a new directory (e.g. `PongGame/`).
2. Add a `.csproj` targeting `net9.0` with references to `MonoGame.Framework.DesktopGL` and `GameCore`.
3. Add the project to `csharp-monogame-poc.sln` via `dotnet sln add`.
4. Create a `Content/Content.mgcb` for any assets.
5. Extend `GameCore.Core` for the main game class.

---

## Code Style

- Namespaces match directory names (PascalCase).
- `internal static` singletons use the `s_` prefix (e.g. `s_instance`).
- `TODO` comments are acceptable stubs; replace before merging feature work.
- No external logging frameworks — use `System.Console` or MonoGame's built-in diagnostics.

---

## What Agents Should NOT Do

- Do not modify `Core.cs` singleton guard logic without updating all consumers.
- Do not commit binary assets (`.xnb`, compiled content) — the MGCB pipeline generates them at build time.
- Do not change `TargetFramework` without updating both projects simultaneously.
- Do not add NuGet packages without checking compatibility with `MonoGame.Framework.DesktopGL 3.8.*`.
