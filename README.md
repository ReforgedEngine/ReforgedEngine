# ReforgedEngine ⚒️ 2.5D TMX Isometric Engine DLL

[![Build](https://img.shields.io/badge/Build-Passing-brightgreen.svg)](https://github.com/ReforgedEngine/ReforgedEngine/actions) [![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.md) [![Chat](https://img.shields.io/badge/Discord-Join-orange.svg)](https://discord.gg/reforgedengine)

**ReforgedEngine** is a pure C# MonoGame library for building **pixel-perfect 2.5D isometric worlds** from Tiled TMX maps. Designed for UO-style RPGs/MMO-likes: multi-floor buildings, directional walls/roofs (NW/NE/SE/SW), infinite chunks, ECS performance (1M+ tiles @60FPS 4K).

**FREE Core (MIT)**: ECS + TMX parser + basic render.
**PRO ($5 Patreon)**: Lighting, FOV, occlusion, streaming.

## 🎮 Features (v0.1 Alpha)
- **TMX Native**: Chunks, groups, layers, properties (`z_base`, `floor`, `RenderLayer`).
- **Isometric Projection**: Feet-based, offsets auto (64x64 tiles).
- **Deferred SortKey**: Front-to-back ulong sort (no depth buffer).
- **Collision**: TMX shapes (ellipse/rect/poly manual - no pixel-perfect Z bugs).
- **PVGames HD**: HiDef LinearWrap + mipmaps.
- **ECS Custom**: SoA archetypes, zero GC, 65k tiles baseline.
- **Infinite Maps**: Chunk streaming ready.

## 🚀 Quick Start (FREE DLL)
### Build DLL
```bash
dotnet restore
dotnet build src/ReforgedEngine --configuration Release
