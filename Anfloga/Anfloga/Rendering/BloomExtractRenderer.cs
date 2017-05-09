using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Anfloga.Rendering
{
    public class BloomExtractRenderer : IEffectsRenderer
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public float Threshold { get; set; } = 0.7f;

        SpriteBatch spriteBatch;
        bool initialized = false;
        Effect effect;
        GraphicsDevice device;

        public void Initialize(GraphicsDevice device, Camera camera)
        {
            Width = device.PresentationParameters.BackBufferWidth;
            Height = device.PresentationParameters.BackBufferHeight;
            this.device = device;
            spriteBatch = new SpriteBatch(this.device);
            effect = FlatRedBallServices.Load<Effect>(@"Content\Shaders\BloomExtract");
            effect.Parameters["BloomThreshold"].SetValue(Threshold);
            initialized = true;
        }

        public void Draw(RenderTarget2D src, RenderTarget2D dest = null)
        {
            if(!initialized)
            {
                throw new Exception("Draw called before effect initialized!");
            }

            int destWidth = dest != null ? dest.Width : Width;
            int destHeight = dest != null ? dest.Height : Height;

            this.device.SetRenderTarget(dest);
            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, effect);
            spriteBatch.Draw(src, new Rectangle(0, 0, destWidth, destHeight), Color.White);
            spriteBatch.End();
        }
    }
}
