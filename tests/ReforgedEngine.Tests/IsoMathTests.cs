// IsoMathTests.cs
using Microsoft.Xna.Framework;
using ReforgedEngine.Isometric;

namespace ReforgedEngine.Tests.Isometric
{
    public class IsoMathTests
    {
        [Theory]
        [InlineData(0, 0, 64, 32, 0, 0)]
        [InlineData(1, 0, 64, 32, 32, 16)]
        [InlineData(0, 1, 64, 32, -32, 16)]
        [InlineData(1, 1, 64, 32, 0, 32)]
        [InlineData(2, 3, 64, 32, -32, 80)]
        public void WorldToIso_ConvertsCorrectly(
            float worldX, float worldY,
            float tileWidth, float tileHeight,
            float expectedIsoX, float expectedIsoY)
        {
            // Arrange
            var worldPos = new Vector2(worldX, worldY);

            // Act
            var result = IsoMath.WorldToIso(worldPos, tileWidth, tileHeight);

            // Assert
            Assert.Equal(expectedIsoX, result.X, 0.01f);
            Assert.Equal(expectedIsoY, result.Y, 0.01f);
        }

        [Theory]
        [InlineData(0, 0, 64, 32, 0, 0)]
        [InlineData(32, 16, 64, 32, 1, 0)]
        [InlineData(-32, 16, 64, 32, 0, 1)]
        [InlineData(0, 32, 64, 32, 1, 1)]
        public void IsoToWorld_ConvertsCorrectly(
            float isoX, float isoY,
            float tileWidth, float tileHeight,
            float expectedWorldX, float expectedWorldY)
        {
            // Arrange
            var isoPos = new Vector2(isoX, isoY);

            // Act
            var result = IsoMath.WorldToIso(isoPos, tileWidth, tileHeight);

            // Assert
            Assert.Equal(expectedWorldX, result.X, 0.01f);
            Assert.Equal(expectedWorldY, result.Y, 0.01f);
        }

        [Fact]
        public void WorldToIso_And_IsoToWorld_AreInverses()
        {
            // Arrange
            var originalWorld = new Vector2(5.5f, 3.25f);
            var tileWidth = 64f;
            var tileHeight = 32f;

            // Act
            var iso = IsoMath.WorldToIso(originalWorld, tileWidth, tileHeight);
            var backToWorld = IsoMath.WorldToIso(iso, tileWidth, tileHeight);

            // Assert
            Assert.Equal(originalWorld.X, backToWorld.X, 0.01f);
            Assert.Equal(originalWorld.Y, backToWorld.Y, 0.01f);
        }

        // IsoMathTests.cs - Adicionar testes específicos para 64x64
        [Theory]
        [InlineData(0, 0, 64, 64, 0, 0)]
        [InlineData(1, 0, 64, 64, 32, 32)]      // Direita: X aumenta
        [InlineData(0, 1, 64, 64, -32, 32)]     // Baixo: Y aumenta
        [InlineData(1, 1, 64, 64, 0, 64)]       // Diagonal
        [InlineData(2, 3, 64, 64, -32, 160)]    // Combinação
        public void WorldToIso_64x64_ConvertsCorrectly(
            float worldX, float worldY,
            float tileWidth, float tileHeight,
            float expectedIsoX, float expectedIsoY)
        {
            // Arrange
            var worldPos = new Vector2(worldX * tileWidth, worldY * tileHeight);

            // Act
            var result = IsoMath.WorldToIso(worldPos, tileWidth, tileHeight);

            // Assert
            Assert.Equal(expectedIsoX, result.X, 0.01f);
            Assert.Equal(expectedIsoY, result.Y, 0.01f);
        }

        [Fact]
        public void ProjectFromPixels_Uses64x64Default()
        {
            // Arrange
            var pixelPos = new Vector2(64, 64); // Um tile para a direita e baixo

            // Act
            var result = IsoMath.ProjectFromPixels(pixelPos);

            // Assert - Deve ser igual a WorldToIso com 64x64
            var expected = IsoMath.WorldToIso(pixelPos, 64f, 64f);
            Assert.Equal(expected.X, result.X, 0.01f);
            Assert.Equal(expected.Y, result.Y, 0.01f);
        }
    }
}