// ComponentMaskTests.cs
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;

namespace ReforgedEngine.Tests.Core.ECS
{
    public class ComponentMaskTests
    {
        [Fact]
        public void EmptyMask_ContainsNothing()
        {
            // Arrange
            var mask = ComponentMask.Empty;

            // Act & Assert
            Assert.False(mask.Contains<Position>());
            Assert.False(mask.Contains<Renderable>());
            Assert.False(mask.Contains<Collider>());
        }

        [Fact]
        public void With_AddsComponentType()
        {
            // Arrange
            var mask = ComponentMask.Empty;

            // Act
            mask = mask.With<Position>();

            // Assert
            Assert.True(mask.Contains<Position>());
            Assert.False(mask.Contains<Renderable>());
        }

        [Fact]
        public void With_CanAddMultipleTypes()
        {
            // Arrange
            var mask = ComponentMask.Empty;

            // Act
            mask = mask.With<Position>().With<Renderable>().With<Collider>();

            // Assert
            Assert.True(mask.Contains<Position>());
            Assert.True(mask.Contains<Renderable>());
            Assert.True(mask.Contains<Collider>());
        }

        [Fact]
        public void Without_RemovesComponentType()
        {
            // Arrange
            var mask = ComponentMask.Empty
                .With<Position>()
                .With<Renderable>()
                .With<Collider>();

            // Act
            mask = mask.Without<Renderable>();

            // Assert
            Assert.True(mask.Contains<Position>());
            Assert.False(mask.Contains<Renderable>());
            Assert.True(mask.Contains<Collider>());
        }

        [Fact]
        public void Contains_Mask_ChecksAllComponents()
        {
            // Arrange
            var mask1 = ComponentMask.Empty
                .With<Position>()
                .With<Renderable>()
                .With<Collider>();

            var mask2 = ComponentMask.Empty
                .With<Position>()
                .With<Renderable>();

            // Act & Assert
            Assert.True(mask1.Contains(mask2));
            Assert.False(mask2.Contains(mask1));
        }
    }
}