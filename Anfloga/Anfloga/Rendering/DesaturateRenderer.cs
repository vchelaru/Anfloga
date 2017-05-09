using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Anfloga.Rendering
{
    public class DesaturateRenderer : IEffectsRenderer
    {
        Effect effect;
        SpriteBatch spriteBatch;
        bool initialized = false;
        GraphicsDevice device;

        public int Width { get; set; }
        public int Height { get; set; }
        public float DesaturationStrength { get; set; } = 1.0f;

        public DesaturateRenderer() { }

        // shortcut to construct and intialize
        public DesaturateRenderer(GraphicsDevice device, Camera camera)
        {
            this.Initialize(device, camera);
        }

        public void Initialize(GraphicsDevice device, Camera camera)
        {
            Width = device.PresentationParameters.BackBufferWidth;
            Height = device.PresentationParameters.BackBufferHeight;
            this.device = device;
            spriteBatch = new SpriteBatch(this.device);

            effect = FlatRedBallServices.Load<Effect>(@"Content\Shaders\Desaturation");
            effect.Parameters["DesaturationStrength"].SetValue(DesaturationStrength);

            initialized = true;
        }

        public void Draw(RenderTarget2D src, RenderTarget2D dest = null)
        {
            if (!initialized)
            {
                throw new Exception("Draw called before renderer was initialized.");
            }

            int destWidth = dest != null ? dest.Width : Width;
            int destHeight = dest != null ? dest.Height : Height;

            this.device.SetRenderTarget(dest);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, effect);
            spriteBatch.Draw(src, new Rectangle(0, 0, destWidth, destHeight), Color.White);
            spriteBatch.End();
        }
    }
}
