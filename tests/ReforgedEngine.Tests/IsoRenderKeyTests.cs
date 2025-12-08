// IsoRenderKeyTests.cs
using Microsoft.Xna.Framework;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.Rendering;

namespace ReforgedEngine.Tests.Isometric
{
    public class IsoRenderKeyTests
    {
        [Fact]
        public void Compute_SortsByRenderLayerFirst()
        {
            // Arrange
            var pos1 = new Position { FeetIso = new Vector2(0, 0), ZBase = 0 };
            var pos2 = new Position { FeetIso = new Vector2(0, 0), ZBase = 0 };

            var ren1 = new Renderable { RenderLayer = RenderLayer.TerrainBase, SortKey = 0 };
            var ren2 = new Renderable { RenderLayer = RenderLayer.ObjectsMedium, SortKey = 0 };

            // Act
            var key1 = IsoRenderKey.From(pos1, ren1);
            var key2 = IsoRenderKey.From(pos2, ren2);

            // Assert - ObjectsMedium deve vir depois (maior número)
            Assert.True(key1.Raw < key2.Raw);
        }

        [Fact]
        public void Compute_SortsByZBaseSecond()
        {
            // Arrange
            var pos1 = new Position { FeetIso = new Vector2(0, 0), ZBase = 0 };
            var pos2 = new Position { FeetIso = new Vector2(0, 0), ZBase = 10 };

            var ren1 = new Renderable { RenderLayer = RenderLayer.Floor, SortKey = 0 };
            var ren2 = new Renderable { RenderLayer = RenderLayer.Floor, SortKey = 0 };

            // Act
            var key1 = IsoRenderKey.From(pos1, ren1);
            var key2 = IsoRenderKey.From(pos2, ren2);

            // Assert - ZBase maior deve vir depois
            Assert.True(key1.Raw < key2.Raw);
        }

        [Fact]
        public void Compute_SortsByFeetIsoYThird()
        {
            // Arrange
            var pos1 = new Position { FeetIso = new Vector2(0, 0), ZBase = 0 };
            var pos2 = new Position { FeetIso = new Vector2(0, 50), ZBase = 0 };

            var ren1 = new Renderable { RenderLayer = RenderLayer.Floor, SortKey = 0 };
            var ren2 = new Renderable { RenderLayer = RenderLayer.Floor, SortKey = 0 };

            // Act
            var key1 = IsoRenderKey.From(pos1, ren1);
            var key2 = IsoRenderKey.From(pos2, ren2);

            // Assert - Y maior deve vir depois
            Assert.True(key1.Raw < key2.Raw);
        }

        [Fact]
        public void FromPosition_IsAliasForCompute()
        {
            // Arrange
            var pos = new Position { FeetIso = new Vector2(100, 200), ZBase = 15 };
            var ren = new Renderable { RenderLayer = RenderLayer.WallsNW, SortKey = 0 };

            // Act
            var key1 = IsoRenderKey.From(pos, ren);
            var key2 = IsoRenderKey.From(pos, ren);

            // Assert
            Assert.Equal(key1, key2);
        }
    }
}