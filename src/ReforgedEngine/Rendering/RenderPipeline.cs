using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReforgedEngine.Core.Rendering;
using ReforgedEngine.Core.ECS.Components;
using ReforgedEngine.Core.ECS.Entities;

namespace ReforgedEngine.Rendering
{
    public class RenderPipeline
    {
        private readonly Dictionary<RenderLayer, List<Entity>> _layerBatches = new();

        public void BatchAndSort(List<Entity> entities)
        {
            _layerBatches.Clear();
            foreach (var e in entities)
            {
                var rend = e.Get<Renderable>();
                if (!_layerBatches.TryGetValue(rend.RenderLayer, out var batch))
                {
                    batch = new List<Entity>();
                    _layerBatches[rend.RenderLayer] = batch;
                }
                batch.Add(e);
                batch.Sort((a, b) => a.Get<Renderable>().SortKey.CompareTo(b.Get<Renderable>().SortKey));
            }
        }

        public void Draw(SpriteBatch sb, Matrix transform)
        {
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: transform);
            foreach (var kv in _layerBatches.OrderBy(k => k.Key))
            {
                foreach (var e in kv.Value)
                {
                     var pos =  e.Get<Position>();
                     var rend =  e.Get<Renderable>();
                    sb.Draw(rend.Texture, pos.DrawPosition, rend.SourceRect, rend.Tint, 0f, rend.Origin, 1f, SpriteEffects.None, 0f);
                }
            }
            sb.End();
        }
    }
}