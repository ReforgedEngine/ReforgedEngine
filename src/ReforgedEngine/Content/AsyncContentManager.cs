using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ReforgedEngine.Content
{
    public class AsyncContentManager
    {
        private readonly ContentManager _content;
        private readonly GraphicsDevice _graphics;

        public AsyncContentManager(ContentManager content, GraphicsDevice graphics)
        {
            _content = content;
            _graphics = graphics;
        }

        public async Task<Texture2D> LoadAsyncTexture(string path)
        {
            return await Task.Run(() => _content.Load<Texture2D>(path));
        }

        public async Task<SpriteFont> LoadAsyncFont(string path)
        {
            return await Task.Run(() => _content.Load<SpriteFont>(path));
        }
    }
}