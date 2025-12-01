# ReforgedEngine ⚒️ 2.5D TMX Iso Engine

Pixel-perfect isometric engine for Tiled TMX maps. UO-style multi-floor, roofs (NW/NE/SE/SW), infinite chunks, ECS 1M tiles @60FPS.

## 🎮 Status: v0.1.0 Alpha
- ECS Core (custom SoA)
- TMX Parser (chunks/groups/props)
- Deferred SortKey render
- Collision TMX (ellipse/rect/poly)

**FREE Core**: MIT (main branch).
**PRO ($5 Patreon)**: Lighting/FOV/Streaming (pro branch).

## 🚀 Quick Demo
```csharp
var world = new EcsWorld();
world.LoadTmx("forgedlands.tmx");
world.Draw(spriteBatch, camera.Matrix);  // 65k tiles 60FPS!