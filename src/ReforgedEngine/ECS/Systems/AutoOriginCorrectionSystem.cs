// AutoOriginCorrectionSystem.cs
using Microsoft.Xna.Framework;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;

namespace ReforgedEngine.Core.ECS.Systems
{
    /// <summary>
    /// Sistema que automaticamente corrige origens para sprites.
    /// Garante que todos os sprites tenham origem no centro da base.
    /// </summary>
    public sealed class AutoOriginCorrectionSystem : SystemBase
    {
        public bool Enabled { get; set; } = true;

        public AutoOriginCorrectionSystem()
        : base(ComponentMask.Empty
            .With<Position>()
            .With<Renderable>())
        {
        }

        protected override void ProcessArchetype(World world, Archetype archetype, object ctx)
        {
            if (!Enabled) return;

            var entities = archetype.Entities;

            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                var pos = entity.Get<Position>();
                var ren = entity.Get<Renderable>();

                if (ren.Texture == null) continue;

                // Origem esperada: centro na base
                Vector2 expectedOrigin = new Vector2(
                    ren.Texture.Width * 0.5f,
                    ren.Texture.Height);

                // Se a origem atual é diferente, corrigir
                if (!AreOriginsEqual(pos.Origin, expectedOrigin))
                {
                    // Calcular o deslocamento causado pela correção
                    Vector2 originDelta = expectedOrigin - pos.Origin;

                    // Ajustar a posição mundial para compensar
                    // (Mantém o sprite no mesmo lugar visual)
                    pos.WorldPos -= originDelta;
                    pos.FeetWorld = pos.WorldPos;

                    // Aplicar nova origem
                    pos.Origin = expectedOrigin;

                    // Recalcular posição isométrica
                    pos.UpdateIso(Vector2.Zero, 64f, 64f);

                    // Salvar de volta
                    entity.Set(pos);

#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"[AutoOrigin] Corrected entity {entity.Id}: " +
                                                     $"origin {pos.Origin}");
#endif
                }
            }
        }

        private bool AreOriginsEqual(Vector2 a, Vector2 b)
        {
            return Vector2.DistanceSquared(a, b) < 0.1f;
        }

        // Método público para forçar correção de uma entidade específica
        public void ForceCorrection(Entity entity)
        {
            if (entity.Has<Position>() && entity.Has<Renderable>())
            {
                var pos = entity.Get<Position>();
                var ren = entity.Get<Renderable>();

                if (ren.Texture != null)
                {
                    pos.Origin = new Vector2(ren.Texture.Width * 0.5f, ren.Texture.Height);
                    entity.Set(pos);
                }
            }
        }
    }
}