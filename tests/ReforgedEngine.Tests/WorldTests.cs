// WorldTests.cs
using Microsoft.Xna.Framework;
using ReforgedEngine.Core.ECS;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;

namespace ReforgedEngine.Tests.Core.ECS
{
    public class WorldTests
    {
        [Fact]
        public void CreateEntity_ReturnsValidEntity()
        {
            // Arrange
            var world = new World();

            // Act
            var entity = world.CreateEntity();

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(1, entity.Id);
            Assert.Equal(ComponentMask.Empty, entity.Mask);
        }

        [Fact]
        public void CreateMultipleEntities_HasUniqueIds()
        {
            // Arrange
            var world = new World();

            // Act
            var entity1 = world.CreateEntity();
            var entity2 = world.CreateEntity();
            var entity3 = world.CreateEntity();

            // Assert
            Assert.Equal(1, entity1.Id);
            Assert.Equal(2, entity2.Id);
            Assert.Equal(3, entity3.Id);
        }

        [Fact]
        public void Entity_CanAddAndRetrieveComponents()
        {
            // Arrange
            var world = new World();
            var entity = world.CreateEntity();
            var position = new Position
            {
                WorldPos = new Vector2(100, 100),
                Floor = 1,
                ZBase = 5
            };

            // Act
            entity.Set(position);
            var retrieved = entity.Get<Position>();

            // Assert
            Assert.Equal(position.WorldPos, retrieved.WorldPos);
            Assert.Equal(position.Floor, retrieved.Floor);
            Assert.Equal(position.ZBase, retrieved.ZBase);
        }

        [Fact]
        public void Entity_HasComponent_ReturnsCorrectly()
        {
            // Arrange
            var world = new World();
            var entity = world.CreateEntity();

            // Act & Assert
            Assert.False(entity.Has<Position>());

            entity.Set(new Position());
            Assert.True(entity.Has<Position>());

            entity.Remove<Position>();
            Assert.False(entity.Has<Position>());
        }

        [Fact]
        public void EntityMask_UpdatesWhenAddingComponents()
        {
            // Arrange
            var world = new World();
            var entity = world.CreateEntity();

            // Act
            entity.Set(new Position());

            // Assert
            Assert.True(entity.Mask.Contains<Position>());
            Assert.False(entity.Mask.Contains<Renderable>());

            entity.Set(new Renderable());
            Assert.True(entity.Mask.Contains<Position>());
            Assert.True(entity.Mask.Contains<Renderable>());
        }
    }
}