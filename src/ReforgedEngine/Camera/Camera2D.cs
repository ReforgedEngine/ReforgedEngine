using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReforgedEngine.Core.Camera
{
    /// <summary>
    /// Camera isométrica 2D IDÊNTICA à antiga.
    /// Copiada do modelo antigo para garantir compatibilidade.
    /// </summary>
    public sealed class Camera2D
    {
        public Vector2 Position { get; private set; }
        public float Zoom { get; private set; } = 1f;
        public float Rotation { get; private set; } = 0f;

        private readonly Viewport _viewport;

        // Para compatibilidade
        public int ViewWidth => _viewport.Width;
        public int ViewHeight => _viewport.Height;

        public Camera2D(Viewport viewport)
        {
            _viewport = viewport;
        }

        // Construtor alternativo mantendo compatibilidade
        public Camera2D(int viewW, int viewH)
            : this(new Viewport(0, 0, viewW, viewH))
        {
        }

        // ============================================================
        // TRANSFORM MATRIX IDÊNTICA À ANTIGA
        // ============================================================

        public Matrix Transform
        {
            get
            {
                return
                    Matrix.CreateTranslation(new Vector3(-Position, 0f)) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateScale(Zoom, Zoom, 1f) *
                    Matrix.CreateTranslation(
                        _viewport.Width * 0.5f,
                        _viewport.Height * 0.5f,
                        0f
                    );
            }
        }

        // ============================================================
        // MÉTODOS IDÊNTICOS À ANTIGA
        // ============================================================

        public void Move(Vector2 delta)
        {
            Position += delta;
        }

        public void SetZoom(float value)
        {
            Zoom = MathHelper.Clamp(value, 0.1f, 4f);
        }

        public void Rotate(float delta)
        {
            Rotation += delta;
        }

        public void LookAt(Vector2 worldPos)
        {
            Position = worldPos;
        }

        public Vector2 ScreenToWorld(Vector2 screenPos)
        {
            return Vector2.Transform(screenPos, Matrix.Invert(Transform));
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, Transform);
        }
    }
}